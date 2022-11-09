using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus;
using UnityEngine;

public class PauseMenu : GameMenu
{

    
    public void LeaveGame()
    {
        App.AskYesNo("Do you want to forfeit this game and leave?", TryLeave);
    }

    protected void TryLeave(bool isLeaving)
    {
        Close();
        if (isLeaving)
        {
            GameManager.Instance.LeaveGame();
        }
    }
}
