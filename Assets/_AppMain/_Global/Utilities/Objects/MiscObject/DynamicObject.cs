using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObject : MonoBehaviour 
{
    #region Waypoints
    [SerializeField] private List<Waypoint> _waypoints = null;
    public List<Waypoint> Waypoints
    {
        get
        {
            return _waypoints;
        }
    }

    #endregion


    public GameObject MainObject;

    protected Waypoint ByKey(string key)
    {
        for (int i = 0; i < Waypoints.Count; i++)
        {
            if (Waypoints[i].key.ToLower() == key.ToLower()) { return Waypoints[i]; }
        }
        return null;
    }


    public Vector2 WaypointPosition(string key, bool useLocal = false)
    {
        Waypoint toMove = ByKey(key);
        if (toMove == null) { App.LogFatal($"Waypoint of Key {key} does not exist as a Child of this Object."); }

        if (!useLocal) { return toMove.Position; } else { return toMove.transform.localPosition; }
        
    }
    public void MoveToWaypoint(string key)
    {
        Waypoint toMove = ByKey(key);
        if (toMove == null) { App.LogFatal($"Waypoint of Key {key} does not exist as a Child of this Object."); }

        MainObject.transform.localPosition = toMove.Position;
    }
}


public static class DynamicObjectExtensions
{
    public static void MoveToWaypoint(this iDynamicObject dynamicObj, string key)
    {
        DynamicObject obj = dynamicObj.dynamicObject;
        obj.MoveToWaypoint(key);
    }
    public static Vector2 WaypointPosition(this iDynamicObject dynamicObj, string key, bool useLocal = false)
    {
        DynamicObject obj = dynamicObj.dynamicObject;
        return obj.WaypointPosition(key, useLocal);
    }
}
