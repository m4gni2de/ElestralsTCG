using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Menus
{
    public class GameMenu : MonoBehaviour
    {
        #region Properties
        public bool IsOpen { get { return gameObject.activeSelf; } }
        [SerializeField]
        protected GameObject menuObject;
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
        }
        public virtual void Close()
        {
            menuObject.SetActive(false);
        }
    }
}

