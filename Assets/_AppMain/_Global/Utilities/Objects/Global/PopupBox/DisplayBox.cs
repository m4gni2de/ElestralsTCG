using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace PopupBox
{
    public class DisplayBox : BasePopup
    {
        public TMP_Text txtMessage;

        public enum DisplayType
        {
            ConfirmOnly = 0,
            ConfirmAndCancel = 1,
            Timed = 2,
            Conditional = 3,
        }


        public void Show(string msg, Action callback, bool showConfirm = true, bool showCancel = false)
        {
            ToggleHandled(false);
            txtMessage.text = msg;
            ConfirmButton.gameObject.SetActive(showConfirm);
            CancelButton.gameObject.SetActive(showCancel);
            if (callback != null)
            {
                OnHandled = callback;
                
            }
            ConfirmButton.onClick.AddListener(() => Confirm());



        }

        public override void Confirm()
        {
            SendResult();
        }


        #region Timed Messages
        public void ShowPersistent(string msg, Action callback, float time)
        {
            ToggleHandled(false);
            txtMessage.text = msg;
            ConfirmButton.gameObject.SetActive(false);
            CancelButton.gameObject.SetActive(false);
            if (callback != null)
            {
                OnHandled = callback;

            }
            StartCoroutine(ShowTimedMessage(time));
        }

        private IEnumerator ShowTimedMessage(float time)
        {
            float acumTime = 0f;
            do
            {
                yield return new WaitForEndOfFrame();
                acumTime += time;
            } while (true && acumTime < time && !Handled);
            SendResult();
        }
        #endregion

        #region Conditional Messages
        public void ShowUntilCondition(string msg, Func<bool> func, bool conditionValue, Action callback = null)
        {
            ToggleHandled(false);
            txtMessage.text = msg;
            ConfirmButton.gameObject.SetActive(false);
            CancelButton.gameObject.SetActive(false);
            if (callback != null)
            {
                OnHandled = callback;

            }
            StartCoroutine(ShowConditionalMessage(func, conditionValue));
        }

        private IEnumerator ShowConditionalMessage(Func<bool> func, bool conditionValue)
        {
            float acumTime = 0f;
            float maxWaitTime = 60f;
            do
            {
                yield return new WaitForEndOfFrame();
                if (func.Invoke() == conditionValue)
                {
                    ToggleHandled(true);
                }
            } while (true && func.Invoke() != conditionValue);
            SendResult();
            PopupManager.SetActivePopup();
        }
        #endregion

        #region Specific Message Displays
        //public static void ShowUntilCondition(string msg, Func<bool> func, bool reqValue)
        //{
        //    PopupManager.Instance.Dis
        //}
        #endregion
    }
}
