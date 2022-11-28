using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UIHelpers
{
    public static float FreeWidth(this RectTransform rect, RectTransform overlappingObject, float totalPadding = 0f)
    {
        float totalWidth = rect.sizeDelta.x;
        totalWidth -= overlappingObject.sizeDelta.x;
        totalWidth -= totalPadding;
        return totalWidth;
    }

    public static float FreeWidth(this RectTransform rect, float totalPadding = 0f)
    {
        float totalWidth = rect.sizeDelta.x;
        totalWidth -= totalPadding;
        return totalWidth;
    }


    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static bool IsUIObjectOnTop(this RectTransform transform)
    {
        float z = transform.localPosition.z;


        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        foreach (var item in results)
        {
            float objZ = item.gameObject.transform.localPosition.z;
            if (objZ >= z) { return false; }
        }
        return true;
    }

    public static bool IsPointerOverMe(this RectTransform transform)
    {
        Vector2 currPosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        Vector3[] corners = new Vector3[4];
        transform.GetWorldCorners(corners);

        return RectTransformUtility.RectangleContainsScreenPoint(transform, currPosition);
    }

    public static bool IsPointerOnScreen()
    {
        return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y);
    }

    
    public static Vector3[] GetWorldCorners(Transform transform, Rect rect)
    {
        Vector3[] fourCornersArray = GetLocalCorners(rect);

        Matrix4x4 matrix4x = transform.localToWorldMatrix;
        for (int i = 0; i < 4; i++)
        {
            fourCornersArray[i] = matrix4x.MultiplyPoint(fourCornersArray[i]);
        }
        return fourCornersArray;
    }


    public static Vector3[] GetLocalCorners(Rect rect)
    {
        Vector3[] fourCornersArray = new Vector3[4];

        float x = rect.x;
        float y = rect.y;
        float xMax = rect.xMax;
        float yMax = rect.yMax;
        fourCornersArray[0] = new Vector3(x, y, 0f);
        fourCornersArray[1] = new Vector3(x, yMax, 0f);
        fourCornersArray[2] = new Vector3(xMax, yMax, 0f);
        fourCornersArray[3] = new Vector3(xMax, y, 0f);

        return fourCornersArray;
    }

    public static bool DoesIntersect(this RectTransform rect, RectTransform target)
    {
        Vector3[] theseCorners = new Vector3[4];
        rect.GetWorldCorners(theseCorners);

        Vector3[] targetCorners = new Vector3[4];
        target.GetWorldCorners(targetCorners);

        bool contains = false;

        Vector2 min = Camera.main.ScreenToWorldPoint(target.rect.min);
        Vector2 max = Camera.main.ScreenToWorldPoint(target.rect.max);

        if (RectTransformUtility.RectangleContainsScreenPoint(target, rect.position))
        {
            return true;
        }
       
        return contains;

       
       
    }

    public static void OverrideCanvas(this Canvas canvas, string sortLayer, int sortOrder)
    {
        canvas.overrideSorting = true;
        canvas.sortingLayerName = sortLayer;
        canvas.sortingOrder = sortOrder;
        
    }
    public static Canvas GetDropdownCanvas(this TMP_Dropdown drop)
    {
        FieldInfo[] fields = drop.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var item in fields)
        {
            if (item.Name == "m_Dropdown")
            {
                GameObject go = (GameObject)item.GetValue(drop);
                if (go != null)
                {
                    return go.GetComponent<Canvas>();
                }
                
            }
        }
        return null;
    }
}
