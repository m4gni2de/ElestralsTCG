using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.P2P
{
    public class GameChatBox : MonoBehaviour, iShowHide, iFreeze
    {
        #region Interface
        public event Action<bool> OnDisplayChanged;

        public void Hide()
        {
            chat.OnNewMessageAdded -= OnNewChatMessage;
            OnDisplayChanged?.Invoke(false);
            gameObject.SetActive(false);
            this.ThawOnRelease();

        }

        public void Show()
        {
            OnDisplayChanged?.Invoke(true);
            gameObject.SetActive(true);
            this.Freeze();
            if (!scroll.IsLoaded)
            {
                GridSettings sett = GridSettings.CreateInfinite(1, 10, new Vector2(0f, 5f));
                scroll.Initialize(sett, DisplayChatMessage);
                scroll.SetDataContext(chat.Messages);
            }
            else
            {
                int objectCount = scroll.Cells.Count;
                int messageCount = chat.Messages.Count;

                int difference = messageCount - objectCount;
                

                for (int i = objectCount; i < messageCount -1; i++)
                {
                    scroll.AddData(chat.Messages[i]);
                }
            }
            
            chat.OnNewMessageAdded += OnNewChatMessage;
        }
        private void OnDisable()
        {
            this.Thaw();
        }
        #endregion

        #region Properties
        private GameChat _chat = null;
        public GameChat chat
        {
            get
            {
                return _chat;
            }
            set
            {
                _chat = value;
            }
        }
        private bool _isLoaded = false;
        #endregion

        #region UI Properties
        [SerializeField] private CustomScroll scroll;
        [SerializeField] private MagicInput txtInput;
        [SerializeField] private Button sendButton;
        [SerializeField] private MagicButton toggleButton;
        private float sendInterval = 3f;
        #endregion

        #region Life Cycle
        private void Awake()
        {
            txtInput.OnSubmitInput += SubmitInput;
        }
        private void OnDestroy()
        {
            if (txtInput)
            {
                txtInput.OnSubmitInput -= SubmitInput;
            }
            
        }
        private void Refresh()
        {
            txtInput.ClearInput();
            ToggleInput(true);
        }
        public void LoadChat(Game game)
        {
            chat = new GameChat(game.gameId);
            _isLoaded = true;
        }
        #endregion


        #region Events
        private void OnNewChatMessage(ChatMessage chat)
        {
            scroll.AddData(chat);
        }
        private void SubmitInput(MagicInput input)
        {
            if (txtInput.CanInput)
            {
                OnClickSend();
            }
            

        }
        private void DisplayChatMessage(iGridCell obj, object data)
        {
            vmChatMessage vm = (vmChatMessage)obj;
            vm.Show();
            //ChatMessage message = (ChatMessage)data;
            
        }
        private void ToggleSendButton(bool canClick)
        {
            sendButton.interactable = canClick;
        }
        private void ToggleInput(bool canInteract)
        {
            sendButton.interactable = canInteract;
            txtInput.ToggleInputEnabled(canInteract);
        }
        #endregion

        #region Buttons
        public void OnClickSend()
        {
            if (txtInput.Input.IsEmpty()) { return; }
            string chatText = txtInput.Input;
            chat.TrySendMessage(App.Account.Id, chatText);
            WaitForSendInterval();
        }
        private async void WaitForSendInterval()
        {
            ToggleInput(false);
            txtInput.ClearInput();
            float ms = sendInterval * 1000f;
            await Task.Delay(Mathf.RoundToInt(ms));
            Refresh();
        }
        
        public void OpenCloseButton()
        {
            bool isOpen = gameObject.activeSelf;
            if (isOpen)
            {
                Hide();
            }
            else
            {
                if (!_isLoaded)
                {
                    LoadChat(GameManager.ActiveGame);
                }
                Show();
            }

        }
        #endregion

        public void DisplayOpenClose(bool display)
        {
            toggleButton.CanClick = display;
        }

    }
}

