using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Turns
{
    public class TurnManager : MonoBehaviour
    {

        #region Instance/Global Properties
        public static TurnManager Instance { get; private set; }
        public CardActionData CraftingAction { get; set; }
        
        public static bool IsBattlePhase
        {
            get
            {
                if (Instance == null) { return false; }
                if (Instance.ActiveTurn == null) { return false; }
                if (Instance.ActiveTurn.ActivePhase == null) { return false; }
                return Instance.ActiveTurn.ActivePhase == Instance.ActiveTurn.BattlePhase;
            }
        }
        public static bool CanStartBattlePhase
        {
            get
            {
                if (Instance == null) { return false; }
                if (Instance.ActiveTurn == null) { return false; }
                if (Instance.ActiveTurn.ActivePhase == null) { return false; }
                if (Instance.ActiveTurn.ActivePhase == Instance.ActiveTurn.MainPhase && Instance.ActiveTurn.ActivePhase.ActionsComplete)
                {
                    return true;
                }
                return false;
            }
        }
        public static bool ValidateStartBattle(string caller = "")
        {
            if (string.IsNullOrEmpty(caller)) { caller = App.WhoAmI; }

            if (IsBattlePhase || CanStartBattlePhase)
            {
                return Instance.ActiveTurn.ActivePlayer.userId == caller;
            }
            return false;
        }
        #endregion

        #region Properties
        private List<Turn> _History = null;
        public List<Turn> History { get { _History ??= new List<Turn>(); return _History; } }
        public Game Game { get; set; }
        private Turn _ActiveTurn = null;
        public Turn ActiveTurn
        {
            get
            {
                return _ActiveTurn;
            }
            set
            {
                _ActiveTurn = value;
            }
        }
        public int RoundCount { get; set; }
        public int TurnCount
        {
            get
            {
                return History.Count + 1;
            }
        }
        #endregion

        #region Turn Order
        private int _RoundIndex = 0;
        protected int RoundIndex
        {
            get
            {
                return _RoundIndex;
            }
            set
            {
                if (value > TurnOrder.Count - 1)
                {
                    value = 0;
                }
                _RoundIndex = value;
            }
        }
        private List<Player> _TurnOrder = null;
        public List<Player> TurnOrder { get { _TurnOrder ??= GetTurnOrder(); return _TurnOrder; } }
        protected List<Player> GetTurnOrder()
        {
            List<Player> order = new List<Player>();
            List<Player> players = Game.players;
            for (int i = 0; i < players.Count; i++)
            {
                int rand = UnityEngine.Random.Range(0, order.Count);
                order.Insert(rand, players[i]);
            }
            return order;
        }
        #endregion



        #region Initialization
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        public void LoadGame(Game game)
        {
            this.Game = game;
        }
        #endregion


        #region Network Turn Events
        public void StartOnlineGame()
        {
            NetworkPipeline.OpeningDrawsOutbound();
        }
        #endregion

        #region Turn Events/Watchers
        public void StartGame()
        {
            DoOpeningDraws();
        }
        
        protected IEnumerator NewTurn(Turn turn)
        {
            Game.StartNewTurn(turn);
            yield return new WaitForSeconds(1f);

            //do some checking to see if the game is over here as well as hook up the Phase events to the current turn
            if (turn.PreparedActions.Count > 0)
            {
                List<CardAction> actions = new List<CardAction>();
                for (int i = 0; i < turn.PreparedActions.Count; i++)
                {
                    CardAction ac = turn.PreparedActions[i].cardAction;
                    //CardActions.Add(ac);
                }
            }
            int index = 0;
            if (TurnCount == 0) { index = 1; }
            turn.DeclareNewPhase(index);
            Game.SetGameState(GameManager.ActiveGame);
        }

        #endregion


        #region Phase Skipping/Choosing
        public static void StartBattlePhase()
        {
            if (Instance == null) { return; }
            Turn turn = Instance.ActiveTurn;

            //end the phase if it's the Main Phase
            if (turn.ActivePhase.TurnIndex == 1)
            {
                turn.OverwriteAndEndPhase(turn.ActivePhase, 2);
            }
            
        }

        public static void StartEndPhase()
        {
            if (Instance == null) { return; }
            Turn turn = Instance.ActiveTurn;

            //end the phase if it's the Main Phase
            if (turn.ActivePhase.TurnIndex == 1 || turn.ActivePhase.TurnIndex == 2)
            {
                turn.OverwriteAndEndPhase(turn.ActivePhase, 3);
            }

        }
        #endregion




        #region GameEvent Watchers
        protected int DrawCount = 0;
        public void DoOpeningDraws()
        {
            StartCoroutine(AwaitOpeningDraws());
            for (int i = 0; i < TurnOrder.Count; i++)
            {
                Player p = TurnOrder[i];
                TurnOrder[i].StartingDraw();
            }

        }
       

        protected IEnumerator AwaitOpeningDraws()
        {
            int cardCount = 0;
            do
            {
                int count = 0;
                for (int i = 0; i < TurnOrder.Count; i++)
                {
                    Player p = TurnOrder[i];
                    count += Game.CardsInHand(p);
                }
                yield return new WaitForEndOfFrame();
                cardCount = count;

            } while (true && cardCount < 10);
            StartFirstTurn();
        }

        #endregion

        #region Turn Action Handling
        public void StartFirstTurn()
        {
            RoundIndex = -1;
            for (int i = 0; i < GameManager.ActiveGame.players.Count; i++)
            {
                Player p = GameManager.ActiveGame.players[i];
                for (int j = 0; j < p.deck.Cards.Count; j++)
                {
                    GameCard c = p.deck.Cards[i];
                    c.SetWatchers();
                }
            }
            StartTurn();
        }
        public void StartTurn()
        {
            RoundIndex += 1;
            Player activePlayer = TurnOrder[RoundIndex];
            ActiveTurn = new Turn(activePlayer, TurnCount);
            StartCoroutine(NewTurn(ActiveTurn));
        }

        public void DeclareTurnPhase(int index)
        {
            StartCoroutine(DeclareNewPhase(index));
        }

        protected IEnumerator DeclareNewPhase(int index)
        {
            Game.PhaseStart(ActiveTurn, index); 
            yield return new WaitForSeconds(.25f);
            GamePhase ph = ActiveTurn.ActivePhase;
            Game.Events.PhaseStart.Call(ActiveTurn, ph);
            ph.DeclarePhase();
            yield return new WaitForSeconds(.25f);
            DoPhaseActions(ph);
        }

        #region Phase Actions/Action Interrupting
        protected void DoPhaseActions(GamePhase phase)
        {
            ActiveTurn.StartPhase(phase);
            StartCoroutine(AwaitPhaseActions(phase));
        }

        

        private IEnumerator AwaitPhaseActions(GamePhase phase)
        {
            List<CardAction> actions = phase.PhaseActions;

            if (actions.Count > 0)
            {
                do
                {
                    GameManager.SetActiveAction(actions[0]);
                    CardAction ActiveAction = GameManager.Instance.ActiveAction;
                    yield return DeclarePhaseAction(phase, ActiveAction);
                    yield return ActiveAction.Do();
                    //yield return ActiveAction.PerformAction();
                    phase.History.Add(ActiveAction.ActionData);
                    actions.RemoveAt(0);
                    yield return new WaitForEndOfFrame();

                } while (true && actions.Count > 0);
            }

            phase.CompleteActions();
        }

        private IEnumerator DeclarePhaseAction(GamePhase ph, CardAction ac)
        {
            yield return ac.DeclareAction();
            //do
            //{

            //} while (true);
        }
        #endregion

        public void EndTurn(Turn turn)
        {
            History.Add(turn);
            StartTurn();
        }
        #endregion

        
        
        public static void SetCrafingAction(CardActionData data = null)
        {
            if (Instance == null) { App.LogFatal("Turn Manager does not yet exist so Actions cannot yet be made."); }
            if (data == null)
            {
                Instance.CraftingAction = null;
            }
            else
            {
                Instance.CraftingAction = data;
            }
        }
       
        public static CardActionData NewAction(ActionCategory cat, Player p)
        {
            if (Instance == null) { return null; }
            CardActionData data = new CardActionData(cat);
            data.SetPlayer(p);
            data.SetResult(ActionResult.Pending);


            SetCrafingAction(data);
            return data;
        }


    }
}

