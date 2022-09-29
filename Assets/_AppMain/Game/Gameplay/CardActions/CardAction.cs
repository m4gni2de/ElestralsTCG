using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
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
    public enum ActionCategory
    {
        None = -1,
        Draw = 0,
        Shuffle = 1,
        Enchant = 2,
        Mode = 3,
        Attack = 4,
        Nexus = 5,
        Ascend = 6,
    }
    public class CardAction : iFreeze
    {
       

        public string id;
        public GameCard sourceCard;
        public Player player;
        private bool _isRunning = false;
        public bool isRunning { get { return _isRunning; } }
        private bool _isResolved = false;
        public bool isResolved { get { return _isResolved; } }
        public ActionCategory category
        {
            get
            {
                return GetCategory();
            }
        }
        protected virtual ActionCategory GetCategory()
        {
            return ActionCategory.None;
        }

        public virtual void ForceCompleteAction()
        {
            _isResolved = true;
        }

        protected float actionTime;
        protected bool IsCounterable;
        

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
                    //if (!_isResolved || _isRunning) { return null; }
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




        public virtual IEnumerator DeclareAction()
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
        protected virtual IEnumerator TryAction(float waitTime = .1f)
        {
            float acumTime = 0f;
            _isRunning = true;
            GameMessage message = GameMessage.FromAction(DeclaredMessage, this, true, -1f);
            GameManager.Instance.messageControl.ShowMessage(message);
            GameManager.DeclareCardAction(this);
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
            End(ActionResult.Failed);
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

        private UnityEvent _OnActionEnd = null;
        public UnityEvent OnActionEnd
        {
            get
            {
                _OnActionEnd ??= new UnityEvent();
                return _OnActionEnd;
            }
        }
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


        #region Base Action Commands

        protected virtual IEnumerator DoMove(GameCard card, CardSlot to, float time = .65f)
        {
            float acumTime = 0f;
            Vector3 direction = GetDirection(card, to);

            do
            {
                this.Freeze();
                yield return new WaitForEndOfFrame();
                float frames = Time.deltaTime / time;
                card.cardObject.transform.position += (direction * frames);
                acumTime += Time.deltaTime;
            } while (Validate(acumTime, time));
            this.Thaw();
        }

        protected virtual IEnumerator DoStaggeredMove(List<GameCard> cards, CardSlot to, float time = .65f, float staggerTime = .04f)
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
        protected virtual IEnumerator DoMovements()
        {
            do
            {
                IEnumerator move = Movements[0];
                yield return move;
                yield return new WaitForEndOfFrame();
                Movements.Remove(move);


            } while (Movements.Count > 0);
            End(ActionResult.Succeed);
        }

        protected IEnumerator DoFlip(GameCard card, CardMode newMode, float time = .65f)
        {
            float acumTime = 0f;
            bool toFaceDown = false;
            if (newMode == CardMode.Defense) { toFaceDown = true; }
            float targetVal = 0f;
            Vector3 startScale = card.cardObject.GetScale();
            do
            {
                Vector3 cardScale = card.cardObject.GetScale();
                Vector2 newScale = cardScale - new Vector3((startScale.x * (Time.deltaTime * 2f)), 0f, 0f);
                card.cardObject.SetScale(newScale);
                yield return new WaitForEndOfFrame();
                acumTime += time;

            } while (Validate(acumTime, time) || card.cardObject.GetScale().x > targetVal);
            card.SetCardMode(newMode);
            card.cardObject.SetScale(startScale);
            card.cardObject.Flip(toFaceDown);
        }
        #endregion

        #region Functions

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

        #region Action Watchers
        public virtual void SourceCardWatcher(GameCard card, bool isSelected)
        {

            if (isSelected)
            {

                ActionData.SetData(CardActionData.SourceKey, card.cardId);
            }
            else
            {
                ActionData.SetData(CardActionData.SourceKey, "");
            }
        }
        #endregion


        #region Network Actions

        public void SendAction(bool send)
        {
            _isRunning = true;
            GameMessage message = GameMessage.FromAction(DeclaredMessage, this, false, -1f);
            GameManager.Instance.messageControl.ShowMessage(message);
            if (send)
            {
                NetworkPipeline.SendActionDeclare(this);
            }
            
        }
       
        public void ConfirmAttempt(ActionResult result)
        {
            actionResult = result;
        }
        #endregion



    }
}

