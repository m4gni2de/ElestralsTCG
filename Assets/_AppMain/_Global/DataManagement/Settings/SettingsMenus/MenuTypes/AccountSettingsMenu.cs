using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nsSettings
{
    public class AccountSettingsMenu : SettingsMenu
    {
        #region Properties
       

        private GameSettings<AccountSettings> gameSettings = null;
        private AccountSettings settings
        {
            get
            {
                return gameSettings.Settings;
            }
        }

        #region UI Properties
        [Header("Sleeves")]
        [SerializeField] private CustomScroll sleevesScroll;
        [SerializeField] private SpriteDisplay selectedSleeve;
        [SerializeField] private Button btnSleeves;

        [Header("Playmatts")]
        [SerializeField] private CustomScroll mattScroll;
        [SerializeField] private SpriteDisplay selectedMatt;
        [SerializeField] private Button btnMatts;

        [Header("Save/Cancel")]
        [SerializeField] private Button saveButton, cancelButton;
        #endregion


        #endregion

        #region Overrides
        public override void LoadSettings()
        {
            
            this.gameSettings = SettingsManager.Account;
            Reload();
            Show();
        }
        public override void Reload()
        {
            Sprite sp = CardFactory.CardSleeveSprite(settings.Sleeves);
            selectedSleeve.SetSprite(sp);
            Sprite mattSprite = CardFactory.PlaymattSprite(settings.Playmatt);
            selectedMatt.SetSprite(mattSprite);
        }
        public override void Show()
        {
            base.Show();
        }
        public override void Hide()
        {
            base.Hide();
        }
        public override bool HasChanges()
        {
            return gameSettings.IsDirty;
        }
        #endregion

        #region Buttons
        public void SaveButton()
        {
            gameSettings.Save();
            Hide();
        }
        public void CancelButton()
        {
            SettingsManager.Account.Rollback();
            gameSettings = SettingsManager.Account;
            Hide();
        }
        public void SleevesButton()
        {
            ToggleSleeveChoice(true);
        }
        public void PlaymatButton()
        {
            TogglePlaymattChoice(true);
        }
        #endregion

        #region Main Menu
        private void ToggleMainMenu(bool isOn)
        {
            bgCanvas.interactable = isOn;
        }
        #endregion

        #region Sleeves
        private void SetSleeveCell(iGridCell obj, object data)
        {
            CardSleeveCell cell = (CardSleeveCell)obj;
            cell.Show();
            cell.SetClickListener(() => SelectSleeves(cell));
        }
        private void SelectSleeves(CardSleeveCell cell)
        {
            settings.Sleeves = cell.sleeveIndex;
            ToggleSleeveChoice(false);
            Reload();
        }
        public async void ToggleSleeveChoice(bool isOn)
        {
            ToggleMainMenu(!isOn);
            if (isOn)
            { 

                List<int> sleeves = new List<int>();
                List<string> sleeveList = await AssetPipeline.GetAssetList<Sprite>("Sleeves");
                

                for (int i = 0; i < sleeveList.Count; i++)
                {
                    string indexString = sleeveList[i].Substring(7);
                    int sleeveIndex = int.Parse(indexString);
                    sleeves.Add(sleeveIndex);
                }

                sleevesScroll.Toggle(true);
                if (!sleevesScroll.IsLoaded)
                {
                    GridSettings sett = GridSettings.CreateInfinite(4, 0, new Vector2(5f, 0f));
                    sleevesScroll.Initialize(sett, SetSleeveCell);
                    sleevesScroll.SetSorter(new ValueSorter("sleeveIndex", GlobalUtilities.SortDirection.ASC));
                    sleevesScroll.SetDataContext(sleeves);
                }
                
            }
            else
            {
                sleevesScroll.Toggle(false);
            }
        }
        #endregion


        #region Playmatt
        private void SetPlaymatCell(iGridCell obj, object data)
        {
            PlaymatScrollCell cell = (PlaymatScrollCell)obj;
            cell.Show();
            cell.SetClickListener(() => SelectMat(cell));
        }
        private void SelectMat(PlaymatScrollCell cell)
        {
            settings.Playmatt = cell.matIndex;
            TogglePlaymattChoice(false);
            Reload();
        }
        public async void TogglePlaymattChoice(bool isOn)
        {
            ToggleMainMenu(!isOn);
            if (isOn)
            {

                List<int> mats = new List<int>();
                List<string> matlist = await AssetPipeline.GetAssetList<Sprite>("Playmatt");


                for (int i = 0; i < matlist.Count; i++)
                {
                    string indexString = matlist[i].Substring(8);
                    int mattIndex = int.Parse(indexString);
                    mats.Add(mattIndex);
                }

                mattScroll.Toggle(true);
                if (!sleevesScroll.IsLoaded)
                {
                    GridSettings sett = GridSettings.CreateInfinite(2, 0, new Vector2(10f, 50f), .5f);
                    mattScroll.Initialize(sett, SetPlaymatCell);
                    mattScroll.SetSorter(new ValueSorter("matIndex", GlobalUtilities.SortDirection.ASC));
                    mattScroll.SetDataContext(mats);
                }

            }
            else
            {
                mattScroll.Toggle(false);
            }
        }
        #endregion
    }
}
