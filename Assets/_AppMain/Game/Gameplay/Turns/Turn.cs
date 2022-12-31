using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.CardActions;
#if UNITY_EDITOR
using UnityEditor.Build.Pipeline;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace Gameplay.Turns
{
    public enum TurnSequence
    {
        DeclareNewTurn = 0,
        StartNewTurn = 1,
        DoNewTurn = 2,
        EndNewTurn = 3,
    }
    public class Turn
    {
        #region Rules/Restrictions
        private bool _canNormalEnchant = false;
        public bool CanNormalEnchant { get { return _canNormalEnchant;  } private set { _canNormalEnchant = value; } }
        #endregion

        #region Properties
        public Player ActivePlayer { get; set; }
        public int turnCount;
        private GamePhase _activePhase = null;
        public GamePhase ActivePhase
        {
            get
            {
                if (_activePhase == null)
                {
                    return new StandbyPhase();
                }
                return _activePhase;
            }
            set
            {
                _activePhase = value;
            }
        }
        private List<PreparedAction> _PreparedActions = null;
        public List<PreparedAction> PreparedActions { get { _PreparedActions ??= new List<PreparedAction>(); return _PreparedActions; } }
        public bool IsYours { get { return ActivePlayer == GameManager.ActiveGame.You; } }
        #endregion

        #region Phases
        public int phaseIndex { get; private set; }
        private int _NextPhase;
        public int NextPhase
        {
            get
            {
                return _NextPhase;
            }
            set
            {
                if (value > Phases.Count - 1) { value = 0; }
                _NextPhase = value;
            }
        }
        

        

        
        public GamePhase DrawPhase;
        public GamePhase MainPhase;
        public GamePhase BattlePhase;
        public GamePhase EndPhase;

        public GamePhase GetPhase(int index)
        {
            return Phases[index];
        }
        public List<GamePhase> Phases
        {
            get
            {

                List<GamePhase> phases = new List<GamePhase>();
                phases.Add(DrawPhase);
                phases.Add(MainPhase);
                phases.Add(BattlePhase);
                phases.Add(EndPhase);
                return phases;
            }
        }
        #endregion
        #region Functions
        public bool IsFirstTurn
        {
            get
            {
                return turnCount < 2;
            }
        }
        //public bool IsMainPhase { get { return ActivePhase.i; } }
        #endregion

        #region Initialization
        public Turn(Player p, int count)
        {
            ActivePlayer = p;
            turnCount = count;
            SetDefaults();
        }

        protected void SetDefaults()
        {
            CanNormalEnchant = true;
            DrawPhase = new DrawPhase(ActivePlayer, IsFirstTurn);
            MainPhase = new MainPhase(ActivePlayer);
            BattlePhase = new BattlePhase(ActivePlayer);
            EndPhase = new EndPhase(ActivePlayer);

            
        }

       
        #region Phases
        public void AddPhaseAction(CardAction ac, GamePhase phase)
        {
            phase.AddAction(ac);
        }

        public void DeclareNewPhase(int index, int nextIndex = -1)
        {
            phaseIndex = index;
            ActivePhase = Phases[index];
            if (nextIndex > -1)
            {
                NextPhase = nextIndex;
            }
            else
            {
                NextPhase = ActivePhase.TurnIndex + 1;
            }
            TurnManager.Instance.DeclareTurnPhase(index);
        }

       
        public void StartPhase(GamePhase phase)
        {
            ActivePhase.OnActionsComplete -= CompletePhaseActions;
            phase.StartPhase();
            ActivePhase.OnActionsComplete += CompletePhaseActions;
        }

        public void CompletePhaseActions(GamePhase phase)
        {
            phase.OnPhaseEnd -= EndActivePhase;
            if (phase.AutoEnd)
            {
                EndActivePhase(phase);
            }
            else
            {
                phase.OnPhaseEnd += EndActivePhase;
            }
        }
        public void EndActivePhase(GamePhase phase)
        {
            phase.OnPhaseEnd -= EndActivePhase;
            phase.EndPhase();
            Game.Events.PhaseEnd.Call(this, phase);

            if (NextPhase == 0)
            {
                EndTurn();
            }
            else
            {
                DeclareNewPhase(NextPhase);
            }
        }

        public void EndTurn()
        {
            TurnManager.Instance.EndTurn(this);
        }
        public void OverwriteAndEndPhase(GamePhase phase, int newIndex)
        {
            NextPhase = newIndex;
            EndActivePhase(phase);
        }
        #endregion

        
        #endregion
    }
}

