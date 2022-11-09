using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class Arena : MonoBehaviour
    {
        #region UI
        private RectTransform Rect;
       
       
        public struct FieldPosition
        {
            public Vector2 minAnchor, maxAnchor;
            public Vector3 rotation;

            public FieldPosition(Vector2 min, Vector2 max, Vector3 rotation)
            {
                minAnchor = min;
                maxAnchor = max;
                this.rotation = rotation;
            }

            public static FieldPosition Near
            {
                get
                {
                    return new FieldPosition(new Vector2(0f, 0f), new Vector2(1f, 0.475f), new Vector3(0f, 0f, 0f));
                }
            }

            public static FieldPosition Far
            {
                get
                {
                    return new FieldPosition(new Vector2(0f, 0.525f), new Vector2(1f, 1f), new Vector3(0f, 0f, 180f));
                }
            }
        }

        

        public float Height(bool useWorldScale = false)
        {
            if (!useWorldScale)
            {
                return Rect.rect.height;
            }
            else
            {
                return Rect.rect.height * WorldCanvas.scaleY;
            }
        }
        public float Width(bool useWorldScale = false)
        {
            if (!useWorldScale)
            {
                return Rect.rect.width;
            }
            else
            {
                return Rect.rect.width * WorldCanvas.scaleX;
            }
        }
        #endregion

        public Field NearField, FarField;

        private List<Field> _Fields = null;
        private List<Field> Fields
        {
            get
            {
                if (_Fields == null)
                {
                    _Fields = new List<Field>();
                    _Fields.Add(NearField);
                    _Fields.Add(FarField);
                }
                return _Fields;
            }
        }

        

        #region Player Properties
        public Field GetPlayerField(Player p)
        {
            if (NearField._player == p) { return NearField; }
            return FarField;
        }
        public Player GetSlotOwner(CardSlot slot)
        {
            for (int i = 0; i < Fields.Count; i++)
            {
                if (Fields[i].cardSlots.Contains(slot))
                {
                    return Fields[i]._player;
                }
            }
            return null;
        }

   
        public void RemoveSelector()
        {

        }

        //public CardSlot GetCardSlot(Player p, CardLocation location)
        //{
        //    Field f = GetPlayerField(p);

        //    switch (location)
        //    {
        //        case CardLocation.removed:
        //            break;
        //        case CardLocation.Elestral:
        //            break;
        //        case CardLocation.Rune:
        //            break;
        //        case CardLocation.Stadium:
        //            break;
        //        case CardLocation.Underworld:
        //            break;
        //        case CardLocation.Deck:
        //            break;
        //        case CardLocation.SpiritDeck:
        //            break;
        //        case CardLocation.Hand:
        //            break;
        //        default:
        //            break;
        //    }
        //}
        #endregion

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
        }
        //public void SetPlayer(Player p, string fieldId)
        //{
        //    Field f = NearField;
        //    if (!p.IsLocal) { f = FarField; }
        //    //if (NearField._player != null) { f = FarField; }
        //    f.SetPlayer(p, fieldId);
        //}
        public void SetPlayer(Player p)
        {
            Field f = NearField;
            //if (!p.IsLocal) { f = FarField; }
            if (NearField._player != null) { f = FarField; }
            f.SetPlayer(p);
        }

        public void SetPlayerOffline(Player p)
        {
            Field f = NearField;
            if (p != GameManager.ActiveGame.You) { f = FarField; }
            f.SetPlayer(p);
            f.AllocateCards();
        }
        



    }
}

