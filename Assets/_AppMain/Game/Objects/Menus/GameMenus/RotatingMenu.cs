using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Gameplay.Menus;
using System.Linq;

public class RotatingMenu : GameMenu
{

    private List<VmSubItem> _items = null;
    public List<VmSubItem> SubItems
    {
        get
        {
            if (_items == null)
            {
                VmSubItem[] items = gameObject.GetComponentsInChildren<VmSubItem>(true);
                _items = items.ToList();

            }
            return _items;
        }
    }



    public bool isHeld = false;
    private float holdTime = 0f;
    public Vector2 defaultPos;

    [SerializeField]
    private TouchObject touch;


    private Vector3 InputPos
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }


    #region Functions
    private RectTransform rect;
    protected float Radius
    {
        get
        {
            return rect.rect.width / 2f;
        }
    }
  
    protected Vector3 ItemPosition(int index)
    {
        float x0 = menuObject.transform.localPosition.x;
        float y0 = menuObject.transform.localPosition.y;
        float radius = Radius;

        float x = (float)(x0 + radius * Math.Cos(2 * Math.PI * index / SubItems.Count));
        float y = (float)(y0 + radius * Math.Sin(2 * Math.PI * index / SubItems.Count));
        return new Vector3(x, y, -2f);
    }

    #endregion
    //public bool IsOpen
    //{
    //    get
    //    {
    //        return gameObject.activeSelf == true;
    //    }
    //}
    //public bool IsSameMonster(MonsterToken token)
    //{
    //    if (ActiveMonster == null) { return true; }

    //    return token == ActiveMonster;

    //}

    protected override void Setup()
    {
        rect = menuObject.GetComponent<RectTransform>();
        Open();
    }
    public override void Open()
    {
        menuObject.SetActive(true);

        for (int i = 0; i < SubItems.Count; i++)
        {
            SubItems[i].SetPosition(ItemPosition(i), true);
        }

        base.Open();
    }


    public void MessageHistoryToggle()
    {

    }
   
    private void RotateMenu(float startClickAngle, float startAngle)
    {
        Vector2 dir = (Vector2)InputPos - (Vector2)menuObject.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float angleDiff = angle - startClickAngle;
        menuObject.transform.eulerAngles = new Vector3(0, 0, startAngle + angleDiff);

        for (int i = 0; i < SubItems.Count; i++)
        {
            SubItems[i].SetRotation(-menuObject.transform.localEulerAngles.z);
            
        }

       
    }

   

    private IEnumerator DoMenuRotate()
    {
        Vector2 dir = (Vector2)InputPos - (Vector2)menuObject.transform.position; 
        float startClickAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float startAngle = menuObject.transform.eulerAngles.z;
        do
        {
            yield return new WaitForEndOfFrame();
            RotateMenu(startClickAngle, startAngle);
            


            holdTime += Time.deltaTime;
        } while (true && Input.GetMouseButton(0));
    }

    
    
   private void OnPointerMoved(Vector3 nMousePos)
    {
       
        
    }

    public void OnHold()
    {

        StartCoroutine(DoMenuRotate());
        
    }

    void Update()
    {
        TouchControls();
    }

    void TouchControls()
    {
       
        //if (Input.GetMouseButtonUp(0))
        //{
        //    HoldCallback(false);
        //}
    }


}
