using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gameplay.Menus.Popup;
using UnityEngine.Events;
using System;

namespace Gameplay.Menus
{
    public class PopupMenu : MonoBehaviour
    {
        #region Properties
        public static PopupMenu Instance { get { return GameManager.Instance.popupMenu; } }
        private static int buttonCount = 5;
        private bool _isOpen = false;
        public bool isOpen { get { return _isOpen; } }

        #region UI
        [SerializeField]
        private PopupButton _templateButton;

        private List<PopupButton> _buttons = null;
        public List<PopupButton> buttons { get { _buttons ??= new List<PopupButton>(); return _buttons; } }

        private LineRenderer _line = null;
        private LineRenderer line { get { _line ??= GetComponentInChildren<LineRenderer>(); return _line; } }
        public Transform _buttonContent;
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
            for (int i = 0; i < buttonCount; i++)
            {
                PopupButton g = Instantiate(_templateButton, _buttonContent);
                buttons.Add(g);
                g.Clear();
            }
        }
        protected void Refresh()
        {

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Clear();
            }
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
        public void SetButtons(CardSlot slot)
        {
            int count = 0;
            foreach (var item in slot.ButtonCommands)
            {
                PopupButton b = buttons[count];
                b.SetText(item.Key);
                b.OnClickEvent.AddListener(item.Value);
                count += 1;
            }
        }

        #region Line Display
        protected IEnumerator MoveLine(CardSlot slot)
        {
            WaitForEndOfFrame frame = new WaitForEndOfFrame();
            do
            {
                line.SetPosition(0, slot.transform.position);
                line.SetPosition(1, transform.position);
                yield return frame;

            } while (true && isOpen);
        }
        #endregion

        #region User Inputs
        public void InputNumber(string title, Action<int> returnedVal)
        {
            StartCoroutine(AwaitNumericInput(title, callback =>
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
        protected IEnumerator AwaitNumericInput(string title, Action<bool> callback)
        {
            NumberInput.Load(transform.parent, title, 0);
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
        
        #endregion

        public void InspectCard(GameCard card)
        {

        }
        
    }
}

