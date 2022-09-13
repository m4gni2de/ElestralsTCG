using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Menus.Popup
{
    public class PopupCommand
    {
        public string name;
        public UnityAction action;
        public int level;
        public int displayOrder;


        PopupCommand(string cmdName, UnityAction ac, int levelIndex, int order)
        {
            name = cmdName;
            action = ac;
            level = levelIndex;
            displayOrder = order;
        }

        public static PopupCommand Create(string name, UnityAction ac, int levelIndex = 0, int order = 0)
        {
            return new PopupCommand(name, ac, levelIndex, order);
        }
    }
}
