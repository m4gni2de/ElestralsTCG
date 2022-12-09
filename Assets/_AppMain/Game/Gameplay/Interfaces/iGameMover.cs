using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iGameMover 
{
    
}

public static class iGameMoverExtensions
{
    public static bool IsInverted(this iGameMover obj)
    {
        if (GameManager.IsOnline)
        {
            return GameManager.isInverted;
        }
        return false;
       
    }
    public static void MoveGamePosition(this iGameMover obj, Transform t, Vector3 toMove)
    {
        //t.position += toMove;
        t.position += toMove;
        //if (IsInverted(obj))
        //{
        //    t.position -= toMove;
        //}
        //else
        //{
        //    t.position += toMove;
        //}
    }

    

}
