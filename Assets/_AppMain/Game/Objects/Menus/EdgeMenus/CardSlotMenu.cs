using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

namespace Gameplay.Menus
{
    public class CardSlotMenu : GameMenu
    {
        public CardSlot SelectedSlot { get; set; }
        public GameCard ActiveCard { get { return SelectedSlot.SelectedCard; } }
        

        #region Spirit Types
        public GameObject SpiritObject;
        [SerializeField]
        private MultiImage t_typeSprite;
        private List<MultiImage> _spirits = null;
        public List<MultiImage> spSpirits
        {
            get
            {
                _spirits ??= new List<MultiImage>();
                return _spirits;
            }
        }
        private Dictionary<int, string> _SpiritMapping = null;
        public Dictionary<int, string> SpiritMapping
        {
            get
            {
                _SpiritMapping ??= GetSpiritMapping();
                return _SpiritMapping;
            }
        }
        protected Dictionary<int, string> GetSpiritMapping()
        {
            Dictionary<int, string> items = new Dictionary<int, string>();
            items.Add(0, "Nase");
            items.Add(1, "Element");
            return items;
        }
        protected MultiImage GetSpiritImage(int index)
        {
            if (index < spSpirits.Count)
            {
                return spSpirits[index];
            }
            return null;
        }
        #endregion


        #region Initialize
        protected override void Setup()
        {
            base.Setup();
            t_typeSprite.AddMapping(SpiritMapping);
            spSpirits.Add(t_typeSprite);

            
        }
        public override void Refresh()
        {
            for (int i = 0; i < spSpirits.Count; i++)
            {
                spSpirits[i].SetSprite(1, null);
                spSpirits[i].Hide();
            }
        }

        public void LoadMenu(CardSlot slot)
        {
            Refresh();
            Open();
            SelectedSlot = slot;
            transform.position = slot.Position;
            LoadSpiritSprites();

            
        }

        private void LoadSpiritSprites()
        {
            List<GameCard> elements = ActiveCard.EnchantingSpirits;
            for (int i = 0; i < elements.Count; i++)
            {
                Element e = elements[i].card.SpiritsReq[0];
                SetSpirit(i, e);
            }
        }
        private void SetSpirit(int index, Element e)
        {
            MultiImage spirit = GetSpiritImage(index);
            Sprite sp = AssetPipeline.ByKey<Sprite>(e.SpriteName);
            if (spirit != null)
            {
                spirit.SetSprite(1, sp);
                spirit.Show();
            }
            else
            {
                MultiImage clone = AddSpirit();
                clone.SetSprite(1, sp);
                clone.Show();
            }

            
        }
        private MultiImage AddSpirit()
        {
            MultiImage clone = Instantiate(t_typeSprite, SpiritObject.transform);
            int count = spSpirits.Count;
            int changeVal = (-2 * count);
            for (int i = 0; i < clone.images.Count; i++)
            {
                clone.images[i].image.ChangeSortOrder(changeVal);
            }
            clone.SetSprite(1, null);
            spSpirits.Add(clone);
            return clone;
        }
        

        #endregion
    }
}

