using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gameplay.Menus.Popup;
using UnityEngine.Events;
using System;

namespace Gameplay.Menus
{
    public class PopupMenu : MonoBehaviour, iGameMover
    {
        #region Properties
        public static PopupMenu Instance { get { return GameManager.Instance.popupMenu; } }
        private static int buttonsPerPage = 5;
        private bool _isOpen = false;
        public bool isOpen { get { return _isOpen; } }

        #region UI
        [SerializeField]
        private PopupButton _templateButton;
        protected int PageIndex
        {
            get
            {
                int index = 0;
                for (int i = 0; i < Pages.Count; i++)
                {
                    if (Pages[i].gameObject.activeSelf == true) { index = i; break; }
                }
                return index;
            }
            set
            {
                for (int i = 0; i < Pages.Count; i++)
                {
                    Pages[i].gameObject.SetActive(i == value);
                }
            }
        }

        [SerializeField]
        private MenuPage PageTemplate;
        [SerializeField]
        private List<MenuPage> Pages = new List<MenuPage>();

        private List<PopupButton> _buttons = null;
        public List<PopupButton> buttons { get { _buttons ??= new List<PopupButton>(); return _buttons; } }

        private LineRenderer _line = null;
        private LineRenderer line { get { _line ??= GetComponentInChildren<LineRenderer>(); return _line; } }

        public GameObject menuObject;

        
        #endregion

        public PopupButton this[string name]
        {
            get
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (buttons[i].CommandName.ToLower() == name.ToLower())
                    {
                        return buttons[i];
                    }
                }
                App.LogFatal($"There is no Button Command named {name}! You must set the command from the CardSlot that is being interacted with to open the Menu.");
                return null;
            }
        }
        #endregion

        private void Awake()
        {
            gameObject.SetActive(true);
            buttons.Add(_templateButton);
            Pages[0].LoadButtons(buttons);
            PageIndex = 0;
            for (int i = 0; i < Pages.Count; i++)
            {
                for (int j = 0; j < buttonsPerPage; j++)
                {
                    CreateButton(i);
                    //PopupButton g = Instantiate(_templateButton, pages[i].transform);
                    //buttons.Add(g);
                    //g.Clear();
                }
            }
            menuObject.SetActive(false);
            
        }

        #region Page Management
        protected int GetTotalPages(int buttonCount)
        {
            if (buttonCount <= buttonsPerPage) { return 1; }
            if (buttonCount % buttonsPerPage > 0)
            {
                return (buttonCount / buttonsPerPage) + 1;
            }
            return buttonCount / buttonsPerPage;
        }

        public void AddPages(int countToAdd)
        {
            for (int i = 0; i < countToAdd; i++)
            {
                AddPage();
            }
        }
        public void AddPage(bool isTemp = false)
        {
            MenuPage page = CreatePage();
            page.SetPage(Pages.Count, isTemp);

            List<PopupButton> buttons = new List<PopupButton>();
            for (int i = 0; i < buttonsPerPage; i++)
            {
                buttons.Add(CreateButton(page));
            }
            Pages.Add(page);
            page.LoadButtons(buttons);
        }
        protected MenuPage CreatePage()
        {
            return Instantiate(PageTemplate, transform);
        }

        protected PopupButton CreateButton(int page)
        {
            PopupButton g = Instantiate(_templateButton, Pages[page].transform);
            buttons.Add(g);
            g.Clear();
            return g;
        }
        protected PopupButton CreateButton(MenuPage parent)
        {
            PopupButton g = Instantiate(_templateButton, parent.transform);
            buttons.Add(g);
            g.Clear();
            return g;
        }
        #endregion


        protected void Refresh()
        {
            StopAllCoroutines();
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Clear();
            }
            PageIndex = 0;
        }
        public void LoadMenu(CardSlot slotFrom)
        {
            Refresh();
            menuObject.SetActive(true);
            _isOpen = true;
            SetButtons(slotFrom);
            StartCoroutine(MoveLine(slotFrom));

        }
        public void CloseMenu()
        {
            Refresh();
            menuObject.SetActive(false);
            _isOpen = false;
        }
        public void HideMenu()
        {
            menuObject.SetActive(false);
        }
        public void ShowMenu()
        {
            menuObject.SetActive(true);
        }
        private void SetButtons(CardSlot slot)
        {
            int count = 0;
            int pagesNeeded = GetTotalPages(slot.ButtonCommands.Count);

            if (pagesNeeded != Pages.Count)
            {
                AddPages(pagesNeeded - Pages.Count);
            }
            
            foreach (var item in slot.ButtonCommands)
            {
                PopupButton b = buttons[count];
                b.LoadCommand(item);
                b.Show();
                count += 1;
            }
        }


      
        #region Line Display
        protected IEnumerator MoveLine(CardSlot slot)
        {
            WaitForEndOfFrame frame = new WaitForEndOfFrame();
            do
            {
                Vector3 position = slot.transform.position;
                if (slot.SelectedCard != null)
                {
                    position = slot.SelectedCard.cardObject.transform.position;
                }
                line.SetPosition(0, position);
                line.SetPosition(1, transform.position);
                yield return frame;

            } while (true && isOpen);
        }
        #endregion

        #region User Inputs
        public void InputNumber(string title, Action<int> returnedVal, int min = -1, int max = -1)
        {
            this.Orient(transform);
            StartCoroutine(AwaitNumericInput(title, min, max, callback =>
            {
                if (callback)
                {
                    int val = NumberInput.Instance.Value;
                    returnedVal(val);
                }
                NumberInput.Instance.Close();
                CloseMenu();
            }));
        }

        protected IEnumerator AwaitNumericInput(string title, int min, int max, Action<bool> callback)
        {
            NumberInput.Load(transform.parent, title, min, max);
            HideMenu();
            
            do
            {
                yield return new WaitForEndOfFrame();
            } while (true && !NumberInput.Instance.IsHandled);

            if (NumberInput.Instance.Result == InputBox.InputResult.Confirm)
            {
                callback(true);
            }
            if (NumberInput.Instance.Result == InputBox.InputResult.Cancel)
            {
                callback(false);
            }
        }

        public void ConfirmAction(string msg, Action<bool> action)
        {
            App.AskYesNo(msg, action);
            HideMenu();
        }
        #endregion


       
        public void InspectCard(GameCard card)
        {

        }
        
    }
}

