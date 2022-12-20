using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Commands
{
    public class Ascend : GameCommand
    {
        #region Overrides
        protected override string DefaultKey { get { return "SpecialCast"; } }
        #endregion
        #region Special Cast Args
        public class AscendArgs : CommandArgs
        {
            public string TributedCard { get; private set; }
            public string AscendCard { get; private set; }
            public List<string> SpiritsUsed { get; private set; }
            public string SlotFrom { get; private set; }
            public string SlotTo { get; private set; }

            public AscendArgs(Ascend comm) : base(comm)
            {
                TributedCard = comm.tributedCard.cardId;
                AscendCard = comm.ascendCard.cardId;
                SpiritsUsed = new List<string>();
                for (int i = 0; i < comm.spiritsUsed.Count; i++)
                {
                    SpiritsUsed.AddRange(comm.spiritsUsed[i].cardId);
                }
                SlotFrom = comm.cardFrom.slotId;
                SlotTo = comm.cardTo.slotId;
            }
        }
        #endregion

        #region Properties
        protected GameCard tributedCard { get; set; }
        protected GameCard ascendCard { get; set; }
        protected List<GameCard> spiritsUsed { get; set; }
        protected CardSlot cardFrom { get; set; }
        protected CardSlot cardTo { get; set; }
        #endregion

        public Ascend(GameCard tributed, GameCard ascended, List<GameCard> spiritUsed, CardSlot from, CardSlot to)
        {
            SetCommand(DefaultKey);
            tributedCard = tributed;
            ascendCard = ascended;
            spiritsUsed = new List<GameCard>();
            for (int i = 0; i < spiritUsed.Count; i++)
            {
                spiritsUsed.Add(spiritsUsed[i]);
            }
            cardFrom = from;
            cardTo = to;
        }


    }
}
