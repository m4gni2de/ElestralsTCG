using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace Gameplay.Menus
{
    public class GameMenu : MonoBehaviour, iInvert
    {
        #region Properties
        public bool IsOpen { get { return gameObject.activeSelf; } }
        [SerializeField]
        protected GameObject menuObject;

        protected bool _isDirty = false;
        public bool isDirty { get { return _isDirty; } }

        public event Action<bool> OnMenuToggled;
        
        public string CanvasSortLayer
        {
            get
            {
                return GetSortLayer;
            }
        }

        protected virtual string GetSortLayer { get => "GameMenu"; }
        #endregion

        #region Interface
        public void Invert(bool doInvert)
        {
            if (doInvert)
            {
                transform.localEulerAngles = new Vector3(0f, 0f, 180f);
            }
            else
            {
                transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            }
        }
        #endregion

        private void Awake()
        {
            Setup();
        }

        protected virtual void Setup()
        {
            menuObject.SetActive(false);
        }
        public virtual void Refresh()
        {

        }
        public void Toggle(bool open)
        {
            if (open)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
        public virtual void Open()
        {
            menuObject.SetActive(true);
            OnMenuToggled?.Invoke(true);
        }
        public virtual void Close()
        {
            menuObject.SetActive(false);
            OnMenuToggled?.Invoke(false);
        }

        public virtual void DisplayError(string error)
        {
            GameMessage message = GameMessage.JustMessage($"Error! {error}.");
            message.Show();
            
        }
    }
}

