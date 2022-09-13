using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.PlayingField
{
    public class GameCardManager : MonoBehaviour
    {
        #region Game Card Serialization
        [SerializeField]
        private List<GameCard> _cards = null;
        public List<GameCard> Cards
        {
            get
            {
                if (_cards == null || IsDirty)
                {
                    _cards = GetGameCards();

                }
                return _cards;
            }
        }



        private bool IsDirty = false;
        public void ToggleDirty(bool isDirty)
        {
            this.IsDirty = isDirty;
            
        }
        protected void RefreshCards()
        {
            _cards = GetGameCards();
            acumTime = 0f;
        }

        protected List<GameCard> GetGameCards()
        {
            List<GameCard> list = new List<GameCard>();

            for (int i = 0; i < GameManager.ActiveGame.players.Count; i++)
            {
                GameDeck deck = GameManager.ActiveGame.players[i].deck;

                list.AddRange(deck.Cards);
            }

            IsDirty = false;
            return list;
        }

        #endregion
        #region Properties
        protected float UpdateInterval = 5f;
        protected float acumTime = 0f;
        #endregion

        private void Awake()
        {
            
        }

        private void Update()
        {
            acumTime += Time.deltaTime;

            if (acumTime > UpdateInterval)
            {
                RefreshCards();
            }
        }
    }
}

