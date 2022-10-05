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
        return OnlineGameManager.isInverted;
    }
    public static void MoveGamePosition(this iGameMover obj, Transform t, Vector3 toMove)
    {
        if (IsInverted(obj))
        {
            t.position -= toMove;
        }
        else
        {
            t.position += toMove;
        }
    }

    public static void Orient(this iGameMover obj, Transform t)
    {
        if (IsInverted(obj))
        {
            t.localEulerAngles = new Vector3(0f, 0f, 180f);
        }
        else
        {
            t.localEulerAngles = Vector3.zero;
        }
    }

}
