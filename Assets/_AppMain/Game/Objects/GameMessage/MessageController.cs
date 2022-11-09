using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Gameplay.Messaging;

namespace Gameplay
{
    public class MessageController : MonoBehaviour
    {
        #region Instance
        public static MessageController Instance { get { return GameManager.Instance.messageControl; } }
        #endregion
        #region Properties
        private List<GameMessage> _messages = null;
        public List<GameMessage> Messages
        {
            get
            {
                _messages ??= new List<GameMessage>();
                return _messages;
            }
        }
        [SerializeField]
        private ScrollRect _scrollView;
        [SerializeField]
        private MessageViewModel vmPreviousMessage;
        private List<MessageViewModel> _prevMessages = null;
        public List<MessageViewModel> PrevMessages
        {
            get
            {
                _prevMessages ??= new List<MessageViewModel>();
                return _prevMessages;
            }
        }

        private MessageViewModel _activeMessage = null;
        public MessageViewModel ActiveMessage
        {
            get
            {
                if (_activeMessage == null)
                {
                    GameObject go = AssetPipeline.GameObjectClone(MessageViewModel.AssetName, transform);
                    _activeMessage = go.GetComponent<MessageViewModel>();

                    float xVal = GetComponent<RectTransform>().rect.width / 2f;
                    float yVal = -_activeMessage.GetComponent<RectTransform>().rect.height / 2f;
                    //_activeMessage.gameObject.transform.localPosition = new Vector3(xVal, yVal, -2f);
                    _activeMessage.gameObject.transform.localPosition = new Vector3(0f, 0f, -2f);
                }
                return _activeMessage;
            }
        }
       
        #region Hide/Show Properties
        protected float timeOn = 0f;
        protected float maxTimeOn = 0f;
        #endregion

        #region Slot Selection Display Properties
        [SerializeField]
        private GameObject slotSelectObject;
        [SerializeField]
        private Button undoButton;
        [SerializeField]
        private Button cancelButton;
        #endregion

        #endregion


        private void Awake()
        {
            vmPreviousMessage.Hide();
            HideSlotSelector();
        }
        private void OnDisable()
        {
           

        }
        public void ShowMessage(string msg, bool addToHistory = true)
        {
            GameMessage message = GameMessage.JustMessage(msg);
            ShowMessage(message, addToHistory);
        }

        public void ShowMessage(GameMessage msg, bool addToHistory = true)
        {
            if (addToHistory)
            {
                Messages.Add(msg);
            }
            
            ActiveMessage.ShowMessage(msg);

            if (msg != null && msg.DisplayTime > 0f)
            {
                
            }
        }

       
       
        #region Touch Events
        //public void EndOnTouch()
        //{
        //    if (ActiveMessage != null)
        //    {
        //        GameMessage msg = ActiveMessage;
        //        if (msg.CloseOnTouch)
        //        {
        //            ForceClose(msg);
        //        }
        //    }
        //}
        #endregion


       
        #region DisplayTimer
       
        protected void EndDisplayTimer()
        {
            timeOn = 0f;
            maxTimeOn = 0f;
            if (ActiveMessage.gameObject.activeSelf == true)
            {
                ActiveMessage.Hide();
            }
            
        }

        protected IEnumerator DoDisplay(float maxTime)
        {
            timeOn = 0f;
            maxTimeOn = maxTime;
            do
            {
                yield return new WaitForEndOfFrame();
                timeOn += Time.deltaTime;

            } while (true && ActiveMessage.gameObject.activeSelf == true && timeOn <= maxTimeOn);

            timeOn = 0f;
            maxTimeOn = 0f;
            EndDisplayTimer();
        }

        private void Update()
        {
            
        }
        #endregion

        #region Message History
        private void RefreshHistory()
        {
            for (int i = 0; i < PrevMessages.Count; i++)
            {
                Destroy(PrevMessages[i].gameObject);
            }
            PrevMessages.Clear();
        }
        public void ToggleHistory()
        {
            if (_scrollView.gameObject.activeSelf == true)
            {
                CloseHistory();
            }
            else
            {
                OpenHistory();
            }
        }
        public void OpenHistory()
        {
            ActiveMessage.Hide();
            RefreshHistory();
            _scrollView.gameObject.SetActive(true);

            for (int i = 0; i < Messages.Count; i++)
            {
                MessageViewModel prev = Instantiate(vmPreviousMessage, _scrollView.content);
                prev.DisplaySimple(Messages[i]);
                PrevMessages.Add(prev);
            }
        }
        public void CloseHistory()
        {
            _scrollView.gameObject.SetActive(false);
            RefreshHistory();
            
        }
        #endregion


        #region Slot Selection
        public void HideSlotSelector()
        {
            undoButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            if (_activeMessage)
            {
                ActiveMessage.Hide();
            }
            
            slotSelectObject.SetActive(false);
            
        }

        
        public void DisplaySlotSelector(SlotSelector selector, string msg)
        {
            undoButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            slotSelectObject.SetActive(true);
            undoButton.onClick.AddListener(selector.UndoSelect);
            cancelButton.onClick.AddListener(selector.TryCancel);

            ShowMessage(msg, false);

        }
        #endregion


    }
}

