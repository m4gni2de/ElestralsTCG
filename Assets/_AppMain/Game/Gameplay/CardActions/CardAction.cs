using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
   
    public class CardAction : iFreeze
    {
        public enum Result
        {
            Cancel = -1,
            Pending = 0,
            Succeed = 1,
            Failed = 2,
        }

        public string id;
        public GameCard sourceCard;
        public Player player;
        private bool _isRunning = false;
        public bool isRunning { get { return _isRunning; } }
        private bool _isResolved = false;
        public bool isResolved { get { return _isResolved; } }

        protected float actionTime;

        private List<CardAction> _responses = null;
        public List<CardAction> Resposnes { get { _responses ??= new List<CardAction>(); return _responses; } }
        public Result actionResult;

        private List<IEnumerator> _Movements = null;
        public List<IEnumerator> Movements { get { _Movements ??= new List<IEnumerator>(); return _Movements; } }

        

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

        CardAction()
        {
            id = UniqueString.GetShortId($"ca", 5);
        }
        public CardAction(Player p) : this()
        {
            player = p;
        }
        public CardAction(Player p, GameCard source) : this(p)
        {
            sourceCard = source;
        }
       

        
        #region Doing Action
        public void Do()
        {
            _isRunning = true;
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
            End(Result.Cancel);
        }

        #endregion

        public UnityEvent OnActionEnd;
        public void End(Result result)
        {
            ResolveAction(result);
            _isResolved = true;
            _isRunning = false;
            OnActionEnd?.Invoke();
        }
        
        protected virtual void ResolveAction(Result result)
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
        
        private bool Validate(float acumTime, float time)
        {
            if (GameManager.Instance == true && acumTime < time) { return true; }
            return IsValid();
            
        }
        protected virtual bool IsValid()
        {
            return false;
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

