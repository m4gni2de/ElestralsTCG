using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using RiptideNetworking;
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
        Cast = 2,
        Mode = 3,
        Attack = 4,
        Nexus = 5,
        Ascend = 6,
        Empower = 7,
    }
    public class CardAction : iFreeze
    {
        #region Static Properties
        
        #endregion

        public string id;
        public GameCard sourceCard;
        public Player player;
        private bool _isRunning = false;
        public bool isRunning { get { return _isRunning; } }
        private bool _isResolved = false;
        public bool isResolved { get { return _isResolved; } }

        private bool _isConfirmed = false;
        public bool isConfirmed { get { return _isConfirmed; } }

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

        private List<GameCard> _responseOptions = null;
        public List<GameCard> OptionalResponses { get { _responseOptions ??= new List<GameCard>(); return _responseOptions; } }
        public ActionResult actionResult;

        private List<IEnumerator> _Movements = null;
        public List<IEnumerator> Movements { get { _Movements ??= new List<IEnumerator>(); return _Movements; } }

        protected virtual CardActionData GetActionData()
        {
            return null;
        }
        protected CardActionData _ActionData = null;
        public CardActionData ActionData
        {
            get
            {
                _ActionData = GetActionData();
                return _ActionData;
            }
        }

        protected string GetActionMessage(bool isRemote)
        {
            if (!isRemote) { return LocalActionMessage; }
            return RemoteActionMessage;
        }

        protected virtual string LocalActionMessage { get { return ""; } }
        protected virtual string RemoteActionMessage { get { return LocalActionMessage; } }

        protected string GetDeclaredMessage(bool isRemote)
        {
            if (!isRemote) { return LocalDeclareMessage; }
            return RemoteDeclareMessage;
        }

        protected virtual string LocalDeclareMessage { get { return ""; } }
        protected virtual string RemoteDeclareMessage { get { return LocalDeclareMessage; } }



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
            GameMessage message = GameMessage.FromAction(GetDeclaredMessage(false), this, true, -1f);
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
                GameMessage message = GameMessage.FromAction(GetActionMessage(false), this, true, actionTime);
                GameManager.Instance.messageControl.ShowMessage(message);
                yield return PerformAction();
            }
            else
            {
                FailAction();
            }

            if (player.IsLocal) { NetworkPipeline.SendActionDeclare(this); }
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

        #region Action Ending

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
            OnActionEnd.RemoveAllListeners();
        }

        protected virtual void ResolveAction(ActionResult result)
        {
            actionResult = result;

        }
        #endregion

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
                Vector3 moveBy = direction * frames;
                card.MovePosition(moveBy);
                //card.cardObject.transform.position += (direction * frames);
                //card.NetworkCard.SendPosition();
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
                        //cards[i].cardObject.transform.position += directions[i] * Time.deltaTime;
                        //cards[i].NetworkCard.SendPosition();
                        Vector3 moveBy = directions[i] * Time.deltaTime;
                        cards[i].MovePosition(moveBy);
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
                //card.cardObject.SetScale(newScale);
                //card.NetworkCard.SendScale(newScale);
                card.SetScale(newScale);
                yield return new WaitForEndOfFrame();
                acumTime += time;

            } while (Validate(acumTime, time) || card.cardObject.GetScale().x > targetVal);
            card.SetCardMode(newMode);
            //card.NetworkCard.SendRotation();
            //card.cardObject.SetScale(startScale);
            //card.NetworkCard.SendScale(startScale);
            card.SetScale(startScale);
            card.FlipCard(toFaceDown);
            //card.cardObject.Flip(toFaceDown);
            //card.NetworkCard.Flip(toFaceDown);
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
        public virtual void SetOptionalResponse(GameCard card)
        {

            if (!OptionalResponses.Contains(card))
            {
                OptionalResponses.Add(card);
            }
        }
        #endregion



       
        [MessageHandler((ushort)FromServer.ActionConfirmed)]
        private static void ActionConfirmed(Message message)
        {
            string dataString = message.GetString();
            string id = message.GetString();
        }
        public virtual IEnumerator DisplayRemoteAction()
        {
            this.Freeze();


            float waitTime = .45f;
            GameMessage message = GameMessage.FromAction(RemoteActionMessage, this, true, waitTime);
            GameManager.Instance.messageControl.ShowMessage(message);

            float acumTime = 0;
            do
            {
                acumTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            } while (acumTime <= waitTime && !isResolved);
            this.Thaw();

        }
        
       
        public void ConfirmAttempt(ActionResult result)
        {
            actionResult = result;
        }
       



    }
}

