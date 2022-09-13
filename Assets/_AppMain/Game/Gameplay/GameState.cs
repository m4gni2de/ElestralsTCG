using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decks;
using Gameplay.Decks;

namespace Gameplay
{
    [System.Serializable]
    public class GameState 
    {
        [System.Serializable]
        public class CardState
        {
            
            public string cardId;
            public string slotId;   
            public int cardMode;
            public int index;
            public string playerId;

            public CardState(Player player, GameCard card)
            {
                cardId = card.cardId;
                slotId = card.CurrentSlot.slotId;
                index = card.cardObject.transform.GetSiblingIndex();
                playerId = player.userId;
                cardMode = (int)card.mode;
            }
        }

        [SerializeField]
        private List<CardState> _cardStates = null;
        public List<CardState> CardStates
        {
            get
            {
                _cardStates ??= new List<CardState>();
                return _cardStates;
            }
        }
        

        public GameState(Game game)
        {
            for (int i = 0; i < game.players.Count; i++)
            {
                SetPlayerState(game.players[i]);
            }
        }

        void SetPlayerState(Player p)
        {
            Field field = p.gameField;

            for (int i = 0; i < field.cardSlots.Count; i++)
            {
                CardSlot s = field.cardSlots[i];
                foreach (GameCard card in s.cards)
                {
                    CardStates.Add(new CardState(p, card));
                }
            }
        }
    }
}
