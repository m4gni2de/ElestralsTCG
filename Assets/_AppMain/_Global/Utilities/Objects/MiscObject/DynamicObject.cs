using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObject : MonoBehaviour 
{
    #region Waypoints
    private List<Waypoint> _waypoints = null;
    public List<Waypoint> Waypoints
    {
        get
        {
            if (_waypoints == null)
            {
                _waypoints = FindWaypoints;
            }
            return _waypoints;
        }
    }

    private List<Waypoint> FindWaypoints
    {
        get
        {
            List<Waypoint> waypoints = new List<Waypoint>();
            GetComponentsInChildren<Waypoint>(true, waypoints);
            return waypoints;
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

    public void MoveToWaypoint(string key)
    {
        Waypoint toMove = ByKey(key);
        if (toMove == null) { App.LogFatal($"Waypoint of Key {key} does not exist as a Child of this Object."); }

        MainObject.transform.localPosition = toMove.Position;
    }
}
