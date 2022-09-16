using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Gameplay
{

    public enum ActionResult
    {
        Cancel = -1,
        Pending = 0,
        Succeed = 1,
        Failed = 2,
    }
    public class CardAction : iFreeze
    {
        #region Action Types
        public static readonly string AttackType = "attack";
        public static readonly string DrawType = "draw";
        public static readonly string EnchantType = "enchant";
        public static readonly string ModeType = "mode";
        public static readonly string NexusType = "nexus";
        public static readonly string ShuffleType = "shuffle";

        #endregion
        

        public string id;
        public GameCard sourceCard;
        public Player player;
        private bool _isRunning = false;
        public bool isRunning { get { return _isRunning; } }
        private bool _isResolved = false;
        public bool isResolved { get { return _isResolved; } }

        public virtual void ForceCompleteAction()
        {
            _isResolved = true;
        }

        protected float actionTime;
        protected bool IsCounterable = true;

        private List<CardAction> _responses = null;
        public List<CardAction> Resposnes { get { _responses ??= new List<CardAction>(); return _responses; } }
        public ActionResult actionResult;

        private List<IEnumerator> _Movements = null;
        public List<IEnumerator> Movements { get { _Movements ??= new List<IEnumerator>(); return _Movements; } }

        protected string _actionMessage = "";
        public string ActionMessage { get { return _actionMessage; } }

        protected string _declaredMessage = "";
        public string DeclaredMessage { get { return _declaredMessage; } }

        protected virtual CardActionData GetActionData()
        {
            return null;
        }
        protected CardActionData _ActionData = null;
        public  CardActionData ActionData
        {
            get
            {
                if (_ActionData == null)
                {
                    if (!_isResolved || _isRunning) { return null; }
                    _ActionData = GetActionData();
                }
                return _ActionData;
            }
        }

        #region Initialization
        protected CardAction(CardActionData data)
        {
            _isResolved = false;
            _ActionData = data;
            ParseData(data);

        }
        protected virtual void ParseData(CardActionData data)
        {
            id = data.Value<string>("actionKey");
        }

        CardAction()
        {
            id = UniqueString.GetShortId($"ca", 5);
            _isResolved = false;
        }
        
        public CardAction(Player p) : this()
        {
            player = p;
        }
        public CardAction(Player p, GameCard source) : this(p)
        {
            sourceCard = source;
        }
        public CardAction(Player p, GameCard source, ActionResult result) : this(p, source)
        {
            actionResult = result;
            if (actionResult == ActionResult.Succeed)
            {
                IsCounterable = false;
            }
        }
        #endregion




        public IEnumerator DeclareAction()
        {
            if (!IsCounterable || actionResult != ActionResult.Pending)
            {
                _isRunning = true;
                actionResult = ActionResult.Succeed;
                yield return null;
            }
            else
            {
                yield return TryAction();
            }
        }
        protected virtual IEnumerator TryAction(float waitTime = 1f)
        {
            float acumTime = 0f;
            _isRunning = true;
            GameManager.DeclareCardAction(this);
            GameMessage message = GameMessage.FromAction(DeclaredMessage, this, true, -1f);
            GameManager.Instance.messageControl.ShowMessage(message);
            do
            {
                acumTime += Time.deltaTime;
              yield return new WaitForEndOfFrame();
            } while (acumTime <= waitTime && actionResult == ActionResult.Pending);

            //figure out how to respond to actions here, but for now, just allow players to undo an action if it's been countered
            actionResult = ActionResult.Succeed;
        }
        #region Doing Action
        
        public IEnumerator Do()
        {
            if (actionResult == ActionResult.Succeed)
            {
                GameMessage message = GameMessage.FromAction(_actionMessage, this, true, actionTime);
                GameManager.Instance.messageControl.ShowMessage(message);
                yield return PerformAction();
            }
            else
            {
                FailAction();
            }

            yield return new WaitUntil(() => isResolved);
        }
        public void FailAction()
        {
            ResolveAction(ActionResult.Failed);
        }
        public virtual IEnumerator PerformAction()
        {
            yield return null;
        }
        
        public void Cancel()
        {
            CancelAction();
        }
        protected virtual void CancelAction()
        {
            End(ActionResult.Cancel);
        }
        
        #endregion

        public UnityEvent OnActionEnd;
        public void End(ActionResult result)
        {
            ResolveAction(result);
            this.Thaw();
            _isResolved = true;
            _isRunning = false;
            Movements.Clear();
            OnActionEnd?.Invoke();
        }
        
        protected virtual void ResolveAction(ActionResult result)
        {
            actionResult = result;
            
        }


        #region Functions

        protected virtual IEnumerator DoMove(GameCard card, CardSlot to, float time = .65f)
        {
            float acumTime = 0f;
            Vector3 direction = GetDirection(card, to);

            do
            {
                this.Freeze();
                yield return new WaitForEndOfFrame();
                card.cardObject.transform.position += direction * Time.deltaTime;
                acumTime += Time.deltaTime;
            } while (Validate(acumTime, time));
            this.Thaw();
        }

        protected virtual IEnumerator DoMove(List<GameCard> cards, CardSlot to, float time = .65f, float staggerTime = .04f)
        {
            float acumTime = 0f;
            float fullTime = time + (staggerTime * (float)cards.Count);
            List<Vector3> directions = new List<Vector3>();

            for (int i = 0; i < cards.Count; i++)
            {
                Vector3 direction = GetDirection(cards[i], to);
                directions.Add(direction);
            }

            do
            {
                this.Freeze();
                yield return new WaitForEndOfFrame();
                for (int i = 0; i < cards.Count; i++)
                {
                    if (acumTime > staggerTime * i)
                    {
                        cards[i].cardObject.transform.position += directions[i] * Time.deltaTime;
                    }
                    

                }

                acumTime += Time.deltaTime;
            } while (Validate(acumTime, fullTime));
            this.Thaw();
        }

        

        protected virtual bool Validate(float acumTime, float time)
        {
            bool valid = (GameManager.Instance == true && acumTime < time && !isResolved);
            return valid;
        }

        protected Vector3 GetDirection(GameCard card, CardSlot to)
        {
            Vector3 from = Camera.main.ScreenToWorldPoint(card.cardObject.transform.position);
            Vector3 moveTo = Camera.main.ScreenToWorldPoint(to.Position);
            Vector3 direction = (moveTo - from);
            return direction;
        }
        protected Vector3 GetDirection(GameCard card, Vector2 targetPos)
        {
            Vector3 from = Camera.main.ScreenToWorldPoint(card.cardObject.transform.position);
            Vector3 moveTo = Camera.main.ScreenToWorldPoint(targetPos);
            Vector3 direction = (moveTo - from);
            return direction;
        }

        
        #endregion
    }
}

