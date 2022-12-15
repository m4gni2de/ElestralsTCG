using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decks;
using Gameplay.Decks;
using Defective.JSON;
using Users;

namespace Gameplay
{
    [System.Serializable]
    public class GameState 
    {
        #region Static Properties
        public static GameManager gameManager { get { return GameManager.Instance; } }
        public static Game activeGame { get; set; }
        #endregion


        #region State Objects


        [System.Serializable]
        public class PlayerState
        {
            public string name;
            public string userId;
            public ushort lobbyId;
            public FieldState field;

            public PlayerState(Player p)
            {
                name = p.username;
                userId = p.userId;
                lobbyId = p.lobbyId;
                field = new FieldState(p);
            }

            public string Print(bool prettyPrint = false)
            {
                JSONObject o = JSONObject.Create("Player");
                o.AddField("Name", name);
                o.AddField("id", userId);

                o.Add(field.Print());

                return o.Print(prettyPrint);
            }
        }


        [System.Serializable]
        public class FieldState
        {
            public string id;
            public int index;
            public string name;
            public List<SlotState> Zones = new List<SlotState>();

            public FieldState(Player p)
            {
                Field f = p.gameField;
                id = f.fieldId;
                index = f.baseIndex;
                name = f.name;
            }

            public void AddSlotState(SlotState slot)
            {
                Zones.Add(slot);
            }

            public JSONObject Print(bool prettyPrint = false)
            {
                JSONObject o = JSONObject.Create("Field");
                o.AddField("Name", name);
                o.AddField("id", id);

                for (int i = 0; i < Zones.Count; i++)
                {
                    o.Add(Zones[i].Print());
                }
                return o;

            }
        }

       

        [System.Serializable]
        public class SlotState
        {
            public string name;
            public string id;
            public int index;
            public int slotType;
            public List<CardState> Cards = new List<CardState>();

            public SlotState(CardSlot slot)
            {
                name = slot.SlotLocationName;
                id = slot.slotId;
                index = slot.index;
                slotType = (int)slot.slotType;
            }

            public void AddCardState(CardState card)
            {
                Cards.Add(card);
            }

            public JSONObject Print(bool prettyPrint = false)
            {
                JSONObject o = JSONObject.Create("Zone");
                o.AddField("Name", name);

                for (int i = 0; i < Cards.Count; i++)
                {
                    JSONObject c = JSONObject.Create();
                    c.AddField("id", Cards[i].id);
                    o.Add(c);
                }

                
                return o;

            }

        }

        [System.Serializable]
        public class CardState
        {
            public string id;
            public string uid;
            public int mode;
            public int index;
            public int type;
            public string[] linked;
            

            public CardState(GameCard card, int cardIndex)
            {
                id = card.card.cardData.cardKey;
                uid = card.cardId;
                index = cardIndex;
                mode = (int)card.mode;
                type = (int)card.CardType;

                GetLinkedCards(card);
                
            }

            void GetLinkedCards(GameCard card)
            {
                List<string> linkedCards = new List<string>();

                if (card.CardType == CardType.Rune)
                {
                    if (activeGame.EmpoweredRunes.ContainsKey(card))
                    {
                        linkedCards.Add(activeGame.EmpoweredRunes[card].cardId);
                    }
                }
                else if (card.CardType == CardType.Elestral)
                {
                    foreach (var item in activeGame.EmpoweredRunes)
                    {
                        if (item.Value == card)
                        {
                            linkedCards.Add(item.Key.cardId);
                        }
                    }
                }

                linked = new string[linkedCards.Count];
                for (int i = 0; i < linkedCards.Count; i++)
                {
                    linked[i] = linkedCards[i];
                }
                
                
            }
        }
        #endregion

        #region Properties
        [SerializeField] private string gameId = "";
        [SerializeField] private int turn = 0;
        public int Turn { get { return turn; } }

        [SerializeField] private int phaseIndex = 0;
        public int Phase { get { return phaseIndex; } }

        [SerializeField]
        private List<PlayerState> _players = null;
        public List<PlayerState> Players
        {
            get
            {
                _players ??= new List<PlayerState>();
                return _players;
            }
        }
        #endregion


        public string Print(bool prettyPrint = false)
        {

            //string s = JsonUtility.ToJson(this, prettyPrint);
            string s = "";

            //for (int i = 0; i < Players.Count; i++)
            //{
            //    s += Players[i].Print(prettyPrint);
            //}
            //Debug.Log(s);
            return s;
        }



        public GameState(Game game)
        {
            gameId = game.gameId;
            turn = GameManager.Instance.turnManager.TurnCount;
            phaseIndex = GameManager.Instance.turnManager.ActiveTurn.phaseIndex;

            activeGame = game;

            for (int i = 0; i < game.players.Count; i++)
            {
                SetPlayerState(game.players[i]);
            }
        }

        //void SetFields()

        void SetPlayerState(Player p)
        {
            

            PlayerState pState = new PlayerState(p);
            Field field = p.gameField;

            for (int i = 0; i < field.cardSlots.Count; i++)
            {
                SlotState slot = new SlotState(field.cardSlots[i]);
                pState.field.AddSlotState(slot);
                GetSlotStates(slot, field.cardSlots[i]);

            }

            Players.Add(pState);
        }

        void GetSlotStates(SlotState state, CardSlot slot)
        {
            for (int i = 0; i < slot.cards.Count; i++)
            {
                GameCard card = slot.cards[i];
                int index = i;
                if (slot.slotType == CardLocation.Deck)
                {
                    index = card.deckPosition;
                }
                CardState cardState = new CardState(card, index);
                state.AddCardState(cardState);
            }
        }
    }
}
