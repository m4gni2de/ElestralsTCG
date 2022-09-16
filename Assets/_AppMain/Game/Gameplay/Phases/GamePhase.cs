using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Turns
{
    public enum PhaseSequence
    {
        DeclareNewPhase = 0,
        StartNewPhase = 1,
        DoNewPhase = 2,
        EndNewPhase = 3
    }
    public class GamePhase: iFreeze
    {

        #region Properties

        public bool IsStarted = false;
        private List<CardAction> _PhaseActions = null;
        public List<CardAction> PhaseActions { get { _PhaseActions ??= new List<CardAction>(); return _PhaseActions; } }
        public int TurnIndex { get { return GetTurnIndex(); } }
        protected virtual int GetTurnIndex()
        {
            return -1;
        }
        private PhaseSequence _phaseSequence;
        public PhaseSequence phaseSequence
        {
            get
            {
                return _phaseSequence;
            }
            set
            {
                _phaseSequence = value;
            }
        }
        public bool ActionsComplete = false;
        public bool AutoEnd = false;
        #endregion

        #region Events

        public event Action<GamePhase> OnActionsComplete;
        public void CompleteActions()
        {
            ActionsComplete = true;
            OnActionsComplete?.Invoke(this);
        }
        public UnityEvent OnPhaseStart;
        protected virtual void PhaseStart()
        {
            OnPhaseStart?.Invoke();
        }
        public event Action<GamePhase> OnPhaseEnd;
        protected virtual void PhaseEnd()
        {
            OnPhaseEnd?.Invoke(this);
        }

        public UnityEvent OnPhaseDeclared;
        protected virtual void PhaseDeclare()
        {
            OnPhaseDeclared?.Invoke();
        }
        #endregion


        public GamePhase(Player p)
        {
            
        }
            

        
        public void AddAction(CardAction ac)
        {
            if (!PhaseActions.Contains(ac))
            {
                PhaseActions.Add(ac);
            }
        }
        public void AddAction(CardAction ac, int order)
        {
            if (!PhaseActions.Contains(ac))
            {
                PhaseActions.Insert(order, ac);
            }
        }

        public void DeclarePhase()
        {
            PhaseDeclare();
        }
        public void StartPhase()
        {
            ActionsComplete = false;
            IsStarted = true;
            PhaseStart();
        }
        
       
        
        
        public void EndPhase()
        {
            PhaseEnd();
            ActionsComplete = true;
            IsStarted = false;
        }


        
        public IEnumerator DoActions()
        {
            List<CardAction> actions = PhaseActions;
            
            
            if (actions.Count > 0)
            {
                DoFreeze();
                do
                {
                    GameManager.SetActiveAction(actions[0]);
                    CardAction ActiveAction = GameManager.Instance.ActiveAction;
                    yield return ActiveAction.DeclareAction();
                    yield return ActiveAction.Do();
                    //yield return ActiveAction.PerformAction();
                    
                    actions.RemoveAt(0);
                    GameManager.Instance.gameLog.LogAction(ActiveAction);
                    yield return new WaitForEndOfFrame();

                } while (true && actions.Count > 0);
            }

            DoThaw();
            CompleteActions();
        }

        public IEnumerator DeclareAction(CardAction ac)
        {
            ac.DeclareAction();

            do
            {
                yield return new WaitForEndOfFrame();

            } while (true && ac.Resposnes.Count > 0);

        }


        #region Game Freezing
        protected void DoFreeze()
        {
            this.Freeze();
        }

        protected void DoThaw()
        {
            this.Thaw();
        }
        #endregion

    }
}

