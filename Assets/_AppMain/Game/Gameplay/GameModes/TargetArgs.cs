using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class TargetArgs : iGameModeArgs
    {
        private List<CardType> _targetTypes = null;
        public List<CardType> TypeScope { get { _targetTypes ??= new List<CardType>(); return _targetTypes; } }
        private List<CardLocation> _locations = null;
        public List<CardLocation> LocationScope { get { _locations ??= new List<CardLocation>(); return _locations; } }

        private List<Player> _players = null;
        public List<Player> PlayerScope { get { _players ??= new List<Player>(); return _players; } }
        public int targetAmount;

        public List<GameCard> SelectedTargets = null;
        private List<GameCard> _Targets = null;
        public List<GameCard> Targets
        {
            get
            {
                _Targets ??= new List<GameCard>();
                return _Targets;
            }
        }
        
        public bool isHandled { get; set; }
        public bool isSuccess { get; set; }

        TargetArgs(List<CardType> types, List<CardLocation> locations, List<Player> players, int targetAmt)
        {
            for (int i = 0; i < types.Count; i++) { SetType(types[i]); }
            for (int i = 0; i < locations.Count; i++) { SetLocation(locations[i]); }
            for (int i = 0; i < players.Count; i++) { SetPlayer(players[i]); }
            SetTargetCount(targetAmt);
            isHandled = false;
            isSuccess = false;
        }
        protected void SetType(CardType ty)
        {
            if (!TypeScope.Contains(ty)) { TypeScope.Add(ty); }
        }
        protected void SetLocation(CardLocation loc)
        {
            if (!LocationScope.Contains(loc)) { LocationScope.Add(loc); }
        }
        protected void SetPlayer(Player p)
        {
            if (!PlayerScope.Contains(p)) { PlayerScope.Add(p); }
        }
        protected void SetTargetCount(int val)
        {
            targetAmount = val;
            SelectedTargets = new List<GameCard>();
        }

        public static TargetArgs EnemyElestrals(Player enemy, int targetCount)
        {
            List<CardType> types = CollectionHelpers.ListWith(CardType.Elestral);
            List<CardLocation> locs = CollectionHelpers.ListWith(CardLocation.Elestral);
            List<Player> players = CollectionHelpers.ListWith(enemy);

            TargetArgs args = new TargetArgs(types, locs, players, targetCount);
            return args;
        }


        #region Card Filtering
        public List<GameCard> CardTargets(List<GameCard> fullPopulation)
        {
            List<GameCard> targets = new List<GameCard>();
            for (int i = 0; i < fullPopulation.Count; i++)
            {
                GameCard g = fullPopulation[i];
                if (Validate(g))
                {
                    targets.Add(g);
                }
            }

            return targets;
        }

        public List<CardSlot> SlotTargets(List<CardSlot> fullPopulation)
        {
            List<CardSlot> targets = new List<CardSlot>();
            for (int i = 0; i < fullPopulation.Count; i++)
            {
                if (fullPopulation[i].MainCard != null)
                {
                    CardSlot slot = fullPopulation[i];
                    GameCard g = slot.MainCard;
                    if (Validate(g))
                    {
                        targets.Add(slot);
                    }
                }
                
                
            }

            return targets;
        }

        public bool Validate(GameCard card)
        {
            if (!TypeScope.Contains(card.CardType)) { return false; }
            if (!LocationScope.Contains(card.location)) { return false; }
            if (!PlayerScope.Contains(card.Owner)) { return false; }
            return true;

        }
        #endregion

        

        #region Target Adding/Removing
        public void AsAsTarget(GameCard card)
        {
            if (!Targets.Contains(card))
            {
                Targets.Add(card);
            }
        }

        public void SelectTarget(GameCard card)
        {
            if (SelectedTargets.Contains(card))
            {
                SelectedTargets.Remove(card);
            }
            else
            {
                AddTarget(card);
            }
        }

        protected void AddTarget(GameCard card)
        {
            SelectedTargets.Add(card);

        }
        #endregion

        public void Confirm()
        {
            HandleArgs(true);
        }
        public void Cancel()
        {
            HandleArgs(false);
        }
        protected void HandleArgs(bool success)
        {
            isHandled = true;
            isSuccess = success;
        }

    }
}

