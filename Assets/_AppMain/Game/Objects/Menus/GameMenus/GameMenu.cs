using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace Gameplay.Menus
{
    public class GameMenu : MonoBehaviour
    {
        #region Properties
        public bool IsOpen { get { return gameObject.activeSelf; } }
        [SerializeField]
        protected GameObject menuObject;

        protected bool _isDirty = false;
        public bool isDirty { get { return _isDirty; } }

        public event Action<bool> OnMenuToggled;
        
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

