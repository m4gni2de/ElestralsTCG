using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Menus
{
    public class ScrollMenu : EdgeMenu, iScaleCard, iFreeze
    {
        #region Interface
        [SerializeField]
        protected Vector2 _cardScale = new Vector2(16f, 16f);
        public Vector2 CardScale { get { return _cardScale; } }

        public string SortLayer => "InputMenus";

        #endregion

        #region Properties
        private ScrollRect _scroll = null;
        public ScrollRect Scroll
        {
            get
            {
                _scroll ??= GetComponent<ScrollRect>();
                return _scroll;
            }
        }

        public RectTransform Content { get { return Scroll.content; } }

        public List<GameCard> _cards = null;
        public List<GameCard> cards { get { _cards ??= new List<GameCard>(); return _cards; } }

        #endregion
        private void Awake()
        {
            
        }
        private void Reset()
        {
            if (GetComponent<ScrollRect>() == null) { gameObject.AddComponent<ScrollRect>(); }
        }
        protected override void Open()
        {
            base.Open();
            gameObject.SetActive(true);
            DoFreeze();
            
            
        }
        protected override void Close()
        {
            base.Close();
            gameObject.SetActive(false);
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

