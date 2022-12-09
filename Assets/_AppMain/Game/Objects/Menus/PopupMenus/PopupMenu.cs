using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gameplay.Menus.Popup;
using UnityEngine.Events;
using System;


namespace Gameplay.Menus
{
    public class PopupMenu : MonoBehaviour, iGameMover, iFreeze, iDynamicObject
    {
        #region Properties
        
        public static PopupMenu Instance { get { return GameManager.Instance.popupMenu; } }
        private static int buttonsPerPage = 5;
        private bool _isOpen = false;
        public bool isOpen { get { return _isOpen; } }
        public Vector2 menuPosition { get { return menuObject.transform.position; } set { menuObject.transform.position = value; } }

        protected Vector2 DefaultPosition { get; set; }
        protected Vector2 DefaultLocalPosition { get; set; }

        protected Vector2 maxPos, minPos;

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
        private Button pageUpButton;
        [SerializeField]
        private Button pageDownButton;
        [SerializeField]
        private List<MenuPage> Pages = new List<MenuPage>();

        private List<PopupButton> _buttons = null;
        public List<PopupButton> buttons { get { _buttons ??= new List<PopupButton>(); return _buttons; } }

        private LineRenderer _line = null;
        private LineRenderer line { get { _line ??= GetComponentInChildren<LineRenderer>(); return _line; } }

        public GameObject menuObject;

        [SerializeField]
        private CardView selectedCard { get { return GameManager.Instance.DisplayedCard; } set { GameManager.Instance.DisplayedCard = value; } }

        private CardSlot slotFrom = null;
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

        #region Interface
        private DynamicObject _dynamicObject = null;
        public DynamicObject dynamicObject
        {
            get
            {
                _dynamicObject ??= GetComponent<DynamicObject>();
                return _dynamicObject;
            }
            set
            {
                _dynamicObject = value;
            }
        }
        #endregion

        private void Awake()
        {
            gameObject.SetActive(true);
           
            buttons.Add(_templateButton);
            Pages[0].LoadButtons(buttons);
            SetPageIndex(0);
            for (int i = 0; i < Pages.Count; i++)
            {
                for (int j = 0; j < buttonsPerPage - 1; j++)
                {
                    CreateButton(i);
                }
            }
            menuObject.SetActive(false);
            
        }

        private void Start()
        {
            SetDefaults();
        }
        private void SetDefaults()
        {
            DefaultPosition = menuPosition;
            DefaultLocalPosition = menuObject.transform.localPosition;
            //Vector2 max = GameManager.Instance.arena.Width(true);
            //Vector2 min = GameManager.Instance.arena.GetComponent<RectTransform>().rect.min;

            float width = GameManager.Instance.arena.GetComponent<RectTransform>().right.x;
            float minX = (width + menuObject.GetComponent<RectTransform>().rect.width) - width;
            float maxX = width - menuObject.GetComponent<RectTransform>().rect.width;

            maxPos = new Vector2(DefaultPosition.x + menuObject.GetComponent<RectTransform>().rect.width * 1.5f, DefaultPosition.y + menuObject.GetComponent<RectTransform>().rect.height);
            minPos = new Vector2(DefaultPosition.x - menuObject.GetComponent<RectTransform>().rect.width * 1.5f, DefaultPosition.y - menuObject.GetComponent<RectTransform>().rect.height);


            maxPos = new Vector2(this.WaypointPosition("Right", true).x, DefaultLocalPosition.y);
            minPos = new Vector2(this.WaypointPosition("Left", true).x, DefaultLocalPosition.y);

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
            return Instantiate(PageTemplate, menuObject.transform);
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


        #region Opening/Closing
        protected void Refresh()
        {
            StopAllCoroutines();
            GameManager.Instance.DisplayCard();
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Clear();
            }
            SetPageIndex(0);
            
        }


        public void LoadMenu(CardSlot slotFrom, bool displayCard = true)
        {
            Refresh();
            if (slotFrom.ButtonCommands.Count == 0) { GameManager.Instance.HideDisplayCard();  return; }
            menuObject.SetActive(true);
            _isOpen = true;
            SetButtons(slotFrom);
            StopAllCoroutines();
            StartCoroutine(MoveLine(slotFrom));
            //StartCoroutine(MoveMenu(slotFrom.gameObject));
            StartCoroutine(SlideMenu(slotFrom.gameObject));

            this.slotFrom = slotFrom;

            if (displayCard && slotFrom.SelectedCard != null)
            {
                GameManager.Instance.DisplayCard(slotFrom.SelectedCard);
            }
            else
            {
                GameManager.Instance.HideDisplayCard();
            }
          

        }
        public void CloseMenu()
        {
            Refresh();
            menuObject.SetActive(false);
            //selectedCard.gameObject.SetActive(false);
            GameManager.Instance.HideDisplayCard();
            _isOpen = false;
        }
        public void HideMenu()
        {
            menuObject.SetActive(false);
            _isOpen = false;
        }
        public void ShowMenu()
        {
            menuObject.SetActive(true);
        }
        private void SetButtons(CardSlot slot)
        {
            int count = 0;
            int pagesNeeded = GetTotalPages(slot.ButtonCommands.Count);

            

            if (pagesNeeded > Pages.Count)
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

            SetPageIndex(0);

        }

        
        public void ChangePageNumber(int changeVal)
        {
            int newIndex = PageIndex + changeVal;
            if (newIndex > Pages.Count - 1) { newIndex = Pages.Count - 1; }
            if (newIndex < 0) { newIndex = 0; }
            SetPageIndex(newIndex);
        }

        private void SetPageIndex(int newPage)
        {
            PageIndex = newPage;

            pageUpButton.interactable = PageIndex < Pages.Count - 1;
            pageUpButton.gameObject.SetActive(pageUpButton.interactable);
            pageDownButton.interactable = PageIndex > 0;
            pageDownButton.gameObject.SetActive(pageDownButton.interactable);
        }
        #endregion


        #region Visual Displays
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
                line.SetPosition(1, menuPosition);
                yield return frame;

            } while (true && isOpen);
        }

        public IEnumerator SlideMenu(GameObject btn)
        {
            Vector2 maxLocal = this.WaypointPosition("Right", true);
            Vector2 minLocal = this.WaypointPosition("Left", true);

            Vector2 localPos = menuObject.transform.localPosition;
            Vector2 localBtn = GameManager.Instance.arena.transform.InverseTransformPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

            Vector2 targetPos = maxLocal;

            if (localBtn.x > 0f)
            {
                targetPos = minLocal;
            }
            if (localBtn.x < 0f)
            {
                targetPos = maxLocal;
            }

            targetPos = new Vector2(targetPos.x, localPos.y);


           

            float acumTime = 0f;
            float moveTime = .3f;
            Vector2 direction = targetPos - (Vector2)menuObject.transform.localPosition;
            float distance = direction.magnitude;

            bool atTarget = false;
            float speed = 1.4f;
            do
            {
               

                Vector3 perFrame = (Time.deltaTime * direction) * speed;
                menuObject.transform.localPosition += perFrame;
                CheckBounds(menuObject);
                yield return new WaitForEndOfFrame();
                acumTime += moveTime;

                if (targetPos.x > 0f)
                {
                    atTarget = menuObject.transform.localPosition.x >= targetPos.x;
                }
                if (targetPos.x < 0f)
                {
                    atTarget = menuObject.transform.localPosition.x <= targetPos.x;
                }

            } while (true && isOpen && !atTarget && acumTime <= moveTime);

            menuObject.transform.localPosition = new Vector2(targetPos.x, 0f);

        }

        public IEnumerator MoveMenu(GameObject btn)
        {
            float midPointX = GameManager.Instance.arena.GetComponent<RectTransform>().rect.width / 2f;
            float targetMin = menuObject.GetComponent<RectTransform>().rect.width;
            float targetMax = menuObject.GetComponent<RectTransform>().rect.width;
            Vector2 targetPos = new Vector2(btn.transform.position.x, menuPosition.y);
            Vector2 slotPos = btn.transform.position;
            



            if (btn.transform.position.x < midPointX)
            {
                targetPos = new Vector2(btn.transform.position.x + targetMin, menuPosition.y);
            }
            if (btn.transform.position.x > midPointX)
            {
                targetPos = new Vector2(btn.transform.position.x - targetMin, menuPosition.y);
            }


            float distance = (targetPos - menuPosition).magnitude;



            Vector2 direction = targetPos - menuPosition;
            Vector2 perFrame = Time.deltaTime * direction * 2;


            float acumTime = 0f;
            float moveTime = .3f;

            //if (distance >= targetMin && distance <= targetMax)
            if (distance >= targetMin)
            {
                yield return null;
            }
            else
            {
                do
                {
                    if ((targetPos - menuPosition).magnitude < targetMin)
                    {
                        direction = targetPos - menuPosition;
                        perFrame = Time.deltaTime * direction * 2;
                        menuPosition += perFrame;
                        //distance = Mathf.Abs((targetPos - menuPosition).magnitude);
                        CheckBounds(menuObject);
                    }
                    else if ((targetPos - menuPosition).magnitude > targetMax)
                    {
                        direction = targetPos - menuPosition;
                        perFrame = Time.deltaTime * direction * 2;
                        menuPosition -= perFrame;
                        //distance = Mathf.Abs((targetPos - menuPosition).magnitude);
                        CheckBounds(menuObject);
                    }

                    yield return new WaitForEndOfFrame();
                    distance = (targetPos - menuPosition).magnitude;
                    acumTime += Time.deltaTime;
                } while (true && isOpen && acumTime <= moveTime && distance <= targetMin);



            }


            yield return null;
        }

        public void CheckBounds(GameObject obj)
    {
        
        float maxX = maxPos.x;
        float minX = minPos.x;

        float maxY = maxPos.y;
        float minY = minPos.y;

        if (obj.transform.localPosition.x > maxX)
        {
            obj.transform.localPosition = new Vector3(maxX, obj.transform.localPosition.y, -5f);
        }

        if (obj.transform.localPosition.x < minX)
        {
            obj.transform.localPosition = new Vector3(minX, obj.transform.localPosition.y, -5f);
        }

        if (obj.transform.localPosition.y > maxY)
        {
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, maxY, -5f);
        }

        if (obj.transform.localPosition.y < minY)
        {
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, minY, -5f);
        }
    }


        #endregion

        #region User Inputs
        public void InputNumber(string title, Action<int> returnedVal, int min = -1, int max = -1, int startVal = 0)
        {
            
            StartCoroutine(AwaitNumericInput(title, min, max, startVal, callback =>
            {
                if (callback)
                {
                    int val = NumberInput.Instance.Value;
                    returnedVal(val);
                }
                
                CloseMenu();
            }));
        }

        protected IEnumerator AwaitNumericInput(string title, int min, int max, int startVal, Action<bool> callback)
        {
           NumberInput.Load(transform.parent, title, startVal, min, max);
            HideMenu();
            
            do
            {
                
                yield return new WaitForEndOfFrame();
            } while (true && !NumberInput.Instance.IsHandled && NumberInput.Instance);


            if (NumberInput.Instance == null || NumberInput.Instance.Result == InputBox.InputResult.Cancel)
            {
                callback(false);
            }
            if (NumberInput.Instance.Result == InputBox.InputResult.Confirm)
            {
                callback(true);
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

        private void LateUpdate()
        {
            if (isOpen && slotFrom != null)
            {
                RectTransform rect = menuObject.GetComponent<RectTransform>();

                if (Input.GetMouseButtonDown(0) && !UIHelpers.IsPointerOverMe(rect))
                {
                    slotFrom.CloseMenu(false);
                    slotFrom = null;
                }
            }
        }

        
    }
}

