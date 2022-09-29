using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus.Popup;
using UnityEngine;

namespace Gameplay.CardActions
{
    public class ActionBuilder : MonoBehaviour
    {
        public static ActionBuilder Instance { get; private set; }

        protected CardSlot ActiveSlot { get; set; }
        

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

       
        //public static List<PopupCommand> FromSlot(CardSlot slotClicked)
        //{
        //    List<PopupCommand> commands = new List<PopupCommand>();
        //    Instance.ActiveSlot = slotClicked;
        //    if (!slotClicked.IsYours) { return commands; }
        //}

        //#region Elestral Slot Commands
        //private List<PopupCommand> ElestralSlotCommands(SingleSlot slot)
        //{
        //    List<PopupCommand> commands = new List<PopupCommand>();

        //    if (slot.MainCard != null)
        //    {
        //        commands.Add(PopupCommand.Create("Change Mode", () => ChangeModeCommand()));
        //    }
            
        //}

        //protected void ChangeModeCommand()
        //{
        //    CardMode current = ActiveSlot.SelectedCard.mode;
        //    CardMode newMode = CardMode.Defense;
        //    if (current == CardMode.Defense) { newMode = CardMode.Attack; }

        //    GameManager.Instance.ChangeCardMode(ActiveSlot.Owner, ActiveSlot.SelectedCard, newMode);

        //    ClosePopMenu();
        //    Refresh();

        //}
        //#endregion
    }
}

