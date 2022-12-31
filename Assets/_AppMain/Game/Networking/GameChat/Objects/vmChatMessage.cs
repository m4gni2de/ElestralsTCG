using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalUtilities;

namespace Gameplay.P2P
{
    public class vmChatMessage : BaseScrollCell
    {
        #region Properties
        private ChatMessage _chatMessage = null;
        public ChatMessage chatMessage
        {
            get
            {
                return _chatMessage;
            }
            private set
            {
                if (_chatMessage == value) { return; }
                _chatMessage = value;
                if (value == null)
                {
                    Clear();
                    
                }
                else
                {
                    LoadChat(value);
                }
                
            }
        }
        [SerializeField] private TouchObject touch;

        private int _displayIndex;
        public override int Index => _displayIndex;

        private DateTime _sendTime = DateTime.MinValue;

        [SortableValue(SortBy.Age)]
        protected DateTime sendTime
        {
            get
            {
                if (_chatMessage == null)
                {
                    _sendTime = DateTime.MinValue;
                    return _sendTime;
                }

                if (_sendTime == DateTime.MinValue)
                {
                    _sendTime = chatMessage.whenSend;
                }
                return _sendTime; 
            }
        }

        [SerializeField] private MagicTextBox chatText;
        [SerializeField] private MagicTextBox txtSender;
        [SerializeField] private MagicTextBox txtTime;
        #endregion

        #region Overrides

        public override void LoadData(object data, int index)
        {
            isDirty = true;
            chatMessage = (ChatMessage)data;
            _displayIndex = index;
        }

        public override void Clear()
        {
            _chatMessage = null;
            _displayIndex = -1;
            chatText.Refresh();
            txtSender.Refresh();
            txtTime.Refresh();
            touch.ClearAll();
           
        }
        public override void Remove()
        {
            touch.ClearAll();
            Destroy(gameObject);
        }
        #endregion

        #region Functions
        private bool isDirty = false;
        private bool _isYours;
        public bool IsYours
        {
            get
            {
                if (chatMessage == null) { isDirty = false;  return false; }
                if (isDirty)
                {
                    Player sender = Game.FindPlayer(chatMessage.sender);
                    _isYours = sender.IsYou;
                    isDirty = false;
                }
                return _isYours;
            }
        }

        private string SenderText(ChatMessage message, bool isOnRight)
        {
            string username = Game.FindPlayer(message.sender).username;
            string st = $"{username}: ";
            if (isOnRight)
            {
                st = $" :{username}";
            }
            return st;
        }
        private string TimeText(ChatMessage message, bool isOnRight)
        {
            string time = message.whenSend.ToLocalTime().ToShortTimeString();
            return $"({time})";
        }
        #endregion

        #region Message Displaying
        private void LoadChat(ChatMessage message)
        {
            touch.AddHoldListener(() => OnMessageHold());

            bool isYours = IsYours;


            chatText.SetText(message.Content);
            txtSender.SetText(SenderText(message, isYours));
            txtTime.SetText(TimeText(message, isYours));
            FormatMessage(isYours);
        }
        private void FormatMessage(bool isYours)
        {
            FlipTextBox(chatText, isYours);
            FlipTextBox(txtSender, isYours);
            FlipTextBox(txtTime, isYours);
            if (isYours)
            {
                chatText.Align(TMPro.TextAlignmentOptions.TopRight);
               
            }
            else
            {
                chatText.Align(TMPro.TextAlignmentOptions.TopLeft);
            }
        }

        private void FlipTextBox(MagicTextBox txt, bool flip)
        {
            Vector2 pos = txt.defaultLocalPosition;
            if (flip)
            {
                pos = new Vector2(-txt.defaultLocalPosition.x, txt.defaultLocalPosition.y);
            }
            txt.transform.localPosition = pos;
        }
        #endregion

        #region Touch Controls
        private void OnMessageHold()
        {
            if (IsYours)
            {
                LoadMessageOptions();
            }
        }

        private void LoadMessageOptions()
        {
            chatText.Content.CopyToClipboard();
        }
        #endregion


    }
}

