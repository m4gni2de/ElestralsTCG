using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.CardActions;
using UnityEngine;

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

        #region Properties
        public Player ActivePlayer { get; set; }
        public int turnCount;
        public GamePhase ActivePhase { get; set; }
        private List<PreparedAction> _PreparedActions = null;
        public List<PreparedAction> PreparedActions { get { _PreparedActions ??= new List<PreparedAction>(); return _PreparedActions; } }
        public bool IsFirstTurn
        {
            get
            {
                return turnCount < 2;
            }
        }

        protected int _PhaseIndex { get { return ActivePhase.TurnIndex; } }
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
        #endregion

        public bool HasNormalEnchant = true;

        
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
       


        #region Initialization
        public Turn(Player p, int count)
        {
            ActivePlayer = p;
            turnCount = count;
            SetDefaults();
        }

        protected void SetDefaults()
        {
            HasNormalEnchant = true;
            DrawPhase = new DrawPhase(ActivePlayer, IsFirstTurn);
            MainPhase = new MainPhase(ActivePlayer);
            BattlePhase = new BattlePhase(ActivePlayer);
            EndPhase = new EndPhase(ActivePlayer);

        }


        public void AddPhaseAction(CardAction ac, GamePhase phase)
        {
            phase.AddAction(ac);
        }

        public void StartPhase(int index)
        {
            SetActivePhase(index);
        }

        public event Action<GamePhase> OnPhaseDeclared; 
        public void DeclareDrawPhase()
        {
            OnPhaseDeclared?.Invoke(DrawPhase);
            
        }

        public void SetActivePhase(int index, int nextIndex = -1)
        {
            ActivePhase = Phases[index];
            if (nextIndex > -1)
            {
                NextPhase = nextIndex;
            }
            else
            {
                NextPhase = ActivePhase.TurnIndex + 1;
            }
            TurnManager.Instance.StartTurnPhase(index);
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
            
            if (NextPhase == 0)
            {
                TurnManager.Instance.EndTurn(this);
            }
            else
            {
                StartPhase(NextPhase);
            }
        }
        public void OverwriteAndEndPhase(GamePhase phase, int newIndex)
        {
            NextPhase = newIndex;
            EndActivePhase(phase);
        }
       
        #endregion
    }
}

