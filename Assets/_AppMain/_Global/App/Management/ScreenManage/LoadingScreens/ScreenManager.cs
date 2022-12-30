using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace AppManagement.Loading
{
    public class ScreenManager : MonoBehaviour
    {
        #region Instance
        public static ScreenManager Instance { get; private set; }

        #endregion
        #region Properties
        protected LoadingScreen Screen { get; set; }

        [SerializeField] private LoadingBar loadingBar;

        private int _screenIndex = -1;
        protected int ScreenIndex
        {
            get
            {
                return _screenIndex;
            }
            set
            {
                if (value == _screenIndex) { return; }
                _screenIndex = value;
            }
        }

        private LoadingScreen ChangeScreen(int newScreenIndex)
        {
            string assetName = LoadScreenService.ScreenAssetString(newScreenIndex);
            GameObject go = AssetPipeline.GameObjectClone(assetName, transform);
            go.transform.SetAsFirstSibling();
            LoadingScreen s = go.GetComponent<LoadingScreen>();
            s.Show();
            return s;
        }

        private float _maxDisplayTime = 10f;
        protected float maxDisplayTime { get { return _maxDisplayTime; } set { _maxDisplayTime = value; } }

        private float _minDisplayTime = 1f;
        protected float minDisplayTime { get { return _minDisplayTime; } set { _minDisplayTime = value; } }

        private float acumTime = 0f;
        #endregion
        #region Functions
        public bool IsOpen
        {
            get
            {
                return Screen != null && Screen.gameObject.activeSelf == true;
            }
        }
        #endregion

        #region Life Cycle
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }
        private void OnDestroy()
        {
            if (Instance != null && Instance == this)
            {
                Instance = null;
            }

        }
        public void Hide()
        {
            HideBar();
            HideScreen();
        }
        public void HideScreen()
        {
            
            if (IsOpen)
            {
                Screen.Hide();
            }
           
        }
        public void HideBar()
        {
            if (loadingBar.gameObject.activeSelf == true)
            {
                loadingBar.Hide();
            }

        }
        #endregion

        #region Loading
        public void DisplayScreen(int screenIndex, float displayTime = 0f)
        {
            this.ScreenIndex = screenIndex;

            if (Screen == null)
            {
                if (_screenIndex < 0) { _screenIndex = 0; }
                

            }
            Screen = ChangeScreen(_screenIndex);
            ShowScreen(displayTime);

            

        }
        public void ShowRandomScreen(float displayTime)
        {
            int newIndex = LoadScreenService.RandomScreenIndex();
            DisplayScreen(newIndex, displayTime);

        }
        public void ShowScreen(float displayTime = 0f)
        {
            Screen.Show();

            if (displayTime > 0f)
            {
                StartCoroutine(DoScreenDisplay(displayTime));
            }
            else
            {
                StartCoroutine(DoScreenDisplay());
            }
        }
        public void LoadingBarDisplay(string display, float start, float max, float min = 0f, int roundedDigits = -1)
        {
            loadingBar.Display(display, start, max, min, roundedDigits);
            
        }

        #endregion

        #region Display Manaagement
        private IEnumerator DoScreenDisplay(float maxTime = 0f)
        {
            acumTime = 0f;
            bool useTime = maxTime > 0f;
            do
            {
                yield return null;
                acumTime += Time.deltaTime;
                if (useTime)
                {

                    if (acumTime >= maxTime)
                    {
                        Screen.Hide();
                    }
                }

            } while (true && IsOpen);
        }
        #endregion

        #region Slider Management
        public void SetSlider(float amount)
        {
            loadingBar.SetSlider(amount);
        }
        #endregion
    }
}

