using System.Collections;
using System.Collections.Generic;
using Databases;
using SimpleSQL;
using UnityEngine;

namespace Gameplay
{
    
    
    public class CardFind
    {
        #region Properties
        private static Game ActiveGame { get { return GameManager.ActiveGame; } }

        private CardFindQuery _query = null;
        public CardFindQuery query
        {
            get { return _query; }
            set
            {
                if (_query == value) { return; }
                _query = value;

                _useCardType = value.cardTypes.Count > 0;
                _useCardElements = value.cardElements.Count > 0;
                _useEnchantedElements = value.enchantedElements.Count > 0;
                _useCost = value.costs.Count > 0;
                _useName = !value.withName.IsEmpty();
            }
        }

        private string queryKey { get; set; }
        private List<Player> players { get; set; }
        private List<CardSlot> slotsToCheck { get; set; }
        private List<CardLocation> locations { get; set; }
        private List<CardType> cardTypes { get; set; }
        private List<ElementCode> ofTypes { get; set; }
        private List<ElementCode> enchantedTypes { get; set; }
        private List<int> cardCosts { get; set; }
        private string searchName { get; set; }
        private bool _useCardType { get; set; }
        private bool _useCardElements { get; set; }
        private bool _useEnchantedElements { get; set; }
        private bool _useCost { get; set; }
        private bool _useName { get; set; }
        #endregion

        #region Initialization / Local Object
        CardFind(CardFindQuery q)
        {
            query = q;
            queryKey = q.queryKey;
            players = FromPlayerScope((PlayerScope)q.playerScope);
            locations = Enums.IntToEnum<CardLocation>(q.locations);

            slotsToCheck = new List<CardSlot>();
            for (int i = 0; i < players.Count; i++)
            {
                foreach (var item in players[i].gameField.cardSlots)
                {
                    if (locations.Contains(item.slotType)) { slotsToCheck.Add(item); }
                }
            }
            cardTypes = Enums.IntToEnum<CardType>(q.cardTypes);
            ofTypes = Enums.IntToEnum<ElementCode>(q.cardElements);
            enchantedTypes = Enums.IntToEnum<ElementCode>(q.enchantedElements);
            cardCosts = q.costs;
            searchName = q.withName.ToLower().Trim();
        }

        private List<GameCard> GetFilteredCards()
        {
            List<GameCard> results = new List<GameCard>();
            

            foreach (var p in players)
            {
                foreach (var slot in slotsToCheck)
                {
                    List<GameCard> cardsInSlot = slot.cards;
                    foreach (var card in cardsInSlot)
                    {
                        if (_useCardType && (!IsInFilter(cardTypes, card.CardType))) { continue; }
                        if (_useCardElements && (!ContainsAny(ofTypes, card.card.DifferentElements.AsCodes()))) { continue; }
                        if (_useEnchantedElements && (!ContainsAny(enchantedTypes, card.EnchantingSpiritTypes))) { continue; }
                        if (_useCost && (!IsInFilter(cardCosts, card.card.SpiritsReq.Count))) { continue; }
                        results.Add(card);

                    }

                }
            }

            if (_useName)
            {
                List<GameCard> finalResults = new List<GameCard>();
                for (int i = 0; i < results.Count; i++)
                {
                    string cardName = results[i].card.DisplayName.ToLower().Trim();
                    
                    if (cardName.Contains(searchName)) { finalResults.Add(results[i]); }
                }
                return finalResults;
            }
            

            return results;
        }
        #endregion

        public static List<GameCard> FindCards(CardFindQuery q)
        {
            CardFind find = new CardFind(q);
            return find.GetFilteredCards();

        }


        #region Generic Card Find
        public static List<GameCard> FindCardsBy(PlayerScope plScope = PlayerScope.All, LocationScope locScope = LocationScope.All, List<CardType> cTypes = null, List<ElementCode> reqTypes = null, List<ElementCode> typesEnchanted = null, List<int> ofCost = null)
        {
            

            List<Player> players = FromPlayerScope(plScope);
            List<CardSlot> slotsToCheck = FromLocationScope(locScope);

            List<CardType> cardTypes = new List<CardType>();
            if (cTypes != null) { cardTypes = cTypes; } else { cardTypes.AddRange(Enums.GetAll<CardType>(-1)); }

            List<ElementCode> ofTypes = new List<ElementCode>();
            if (reqTypes != null) { ofTypes = reqTypes; } else { ofTypes.AddRange(Enums.GetAll<ElementCode>(-1)); }

            List<ElementCode> enchantedByTypes = new List<ElementCode>();
            if (typesEnchanted != null) { enchantedByTypes = typesEnchanted; }

            List<int> spiritCost = new List<int>();
            if (ofCost != null) { spiritCost = ofCost; } else { spiritCost.AddRange(0, 1, 2, 3); }

            return DoFilter(players, slotsToCheck, cardTypes, ofTypes, enchantedByTypes, spiritCost);

        }

        public static List<GameCard> FindCardsBy(PlayerScope plScope = PlayerScope.All, List<CardLocation> locs = null, List<CardType> cTypes = null, List<ElementCode> reqTypes = null, List<ElementCode> typesEnchanted = null, List<int> ofCost = null)
        {


            List<Player> players = FromPlayerScope(plScope);

            List<CardLocation> locations = new List<CardLocation>();
            if (locs != null) { locations = locs; } else { locations.AddRange(Enums.GetAll<CardLocation>()); }
            List<CardSlot> slotsToCheck = new List<CardSlot>();

            for (int i = 0; i < players.Count; i++)
            {
                foreach (var item in players[i].gameField.cardSlots)
                {
                    if (locations.Contains(item.slotType)) { slotsToCheck.Add(item); }
                }
            }

            List<CardType> cardTypes = new List<CardType>();
            if (cTypes != null) { cardTypes = cTypes; } else { cardTypes.AddRange(Enums.GetAll<CardType>(-1)); }

            List<ElementCode> ofTypes = new List<ElementCode>();
            if (reqTypes != null) { ofTypes = reqTypes; } else { ofTypes.AddRange(Enums.GetAll<ElementCode>(-1)); }

            List<ElementCode> enchantedByTypes = new List<ElementCode>();
            if (typesEnchanted != null) { enchantedByTypes = typesEnchanted; }

            List<int> spiritCost = new List<int>();
            if (ofCost != null) { spiritCost = ofCost; } else { spiritCost.AddRange(0, 1, 2, 3); }

            return DoFilter(players, slotsToCheck, cardTypes, ofTypes, enchantedByTypes, spiritCost);

        }

        public static List<GameCard> FindCardsWithName(string name, bool matchFull = false, PlayerScope plScope = PlayerScope.All, LocationScope locScope = LocationScope.All)
        {
            List<GameCard> results = FindCardsBy(plScope, locScope);
            List<GameCard> withName = new List<GameCard>();

            string toSearch = name.ToLower().Trim();

            for (int i = 0; i < results.Count; i++)
            {
                string cardName = results[i].card.DisplayName.ToLower().Trim();
                if (matchFull)
                {
                    if (cardName == toSearch) { withName.Add(results[i]); }
                }
                else
                {
                    if (cardName.Contains(toSearch)) { withName.Add(results[i]); }
                }
            }
            return withName;
        }
        public static List<GameCard> FindCardsWithName(string name, List<CardLocation> locs, bool matchFull = false, PlayerScope plScope = PlayerScope.All)
        {
            List<GameCard> results = FindCardsBy(plScope, locs);
            List<GameCard> withName = new List<GameCard>();

            string toSearch = name.ToLower().Trim();

            for (int i = 0; i < results.Count; i++)
            {
                string cardName = results[i].card.DisplayName.ToLower().Trim();
                if (matchFull)
                {
                    if (cardName == toSearch) { withName.Add(results[i]); }
                }
                else
                {
                    if (cardName.Contains(toSearch)) { withName.Add(results[i]); }
                }
            }
            return withName;
        }

        #region Parsing Lookup
        private static List<Player> FromPlayerScope(PlayerScope scope)
        {
            List<Player> results = new List<Player>();
            switch (scope)
            {
                case PlayerScope.None:
                    return results;
                case PlayerScope.All:
                    results.AddRange(ActiveGame.players);
                    break;
                case PlayerScope.User:
                    results.Add(ActiveGame.You);
                    break;
                case PlayerScope.Opponent:
                    results.Add(ActiveGame.You.Opponent);
                    break;
            }
            return results;
        }

        private static List<CardSlot> FromLocationScope(LocationScope scope)
        {
            List<CardSlot> results = new List<CardSlot>();
            switch (scope)
            {
                case LocationScope.All:
                    for (int i = 0; i < ActiveGame.players.Count; i++)
                    {
                        results.AddRange(ActiveGame.players[i].gameField.cardSlots);
                    }
                    break;
                case LocationScope.OnTarget:
                    return results;
                case LocationScope.OnField:
                    for (int i = 0; i < ActiveGame.players.Count; i++)
                    {
                        Player p = ActiveGame.players[i];
                        foreach (var item in p.gameField.cardSlots)
                        {
                            if (item.IsInPlay)
                            {
                                results.Add(item);
                            }
                        }
                    }
                    break;
                case LocationScope.OnYourField:
                    foreach (var item in ActiveGame.You.gameField.cardSlots)
                    {
                        results.Add(item);
                    }
                    break;
                case LocationScope.OnOpponentField:
                    foreach (var item in ActiveGame.You.Opponent.gameField.cardSlots)
                    {
                        results.Add(item);
                    }
                    break;
                case LocationScope.InSpiritDeck:
                    results.Add(ActiveGame.You.gameField.SpiritDeckSlot);
                    break;
                case LocationScope.InUnderWorld:
                    results.Add(ActiveGame.You.gameField.UnderworldSlot);
                    break;
                case LocationScope.InDeck:
                    results.Add(ActiveGame.You.gameField.DeckSlot);
                    break;
            }
            return results;
        }

        #endregion


        #region Do Filter
        private static List<GameCard> DoFilter(List<Player> players, List<CardSlot> slots, List<CardType> cardTypes, List<ElementCode> ofTypes, List<ElementCode> enchantedTypes, List<int> spiritCost)
        {
            List<GameCard> results = new List<GameCard>();

            foreach (var p in players)
            {
                foreach (var slot in slots)
                {
                    List<GameCard> cardsInSlot = slot.cards;
                    foreach (var card in cardsInSlot)
                    {
                        
                        if (!IsInFilter(cardTypes, card.CardType)) { continue; }               
                        if (!ContainsAny(ofTypes, card.card.DifferentElements.AsCodes())) { continue; }
                        if (!ContainsAny(enchantedTypes, card.EnchantingSpiritTypes)) { continue; }
                        if (!IsInFilter(spiritCost, card.card.SpiritsReq.Count)) { continue; }
                        results.Add(card);
                    }
                   
                }
            }

            return results;
        }

        private static bool IsInFilter<T>(List<T> list, T toCheck)
        {
            if (list.Count <= 0) { return true; }
            return list.Contains(toCheck);
        }

        private static bool ContainsAll<T>(List<T> list, List<T> args, bool skipIfEmpty = true)
        {
            if (skipIfEmpty)
            {
                if (list.Count == 0) { return true; }
            }
            return list.ContainsAll(args);
        }

        private static bool ContainsAny<T>(List<T> list, List<T> args, bool skipIfEmpty = true)
        {
            if (skipIfEmpty)
            {
                if (list.Count == 0) { return true; }
            }
            return list.ContainsAny(args);
        }


        #endregion

        #endregion

        #region Specific Card Finds
        public static List<GameCard> EnchantedElestralsOfType(ElementCode ofType, PlayerScope plScope, LocationScope locScope = LocationScope.OnField)
        {
            List<CardType> cTypes = new List<CardType>();
            cTypes.Add(CardType.Elestral);

            List<ElementCode> codes = new List<ElementCode>();
            codes.Add(ofType);
            return FindCardsBy(plScope, locScope, cTypes, null, codes, null);
        }
        #endregion
    }
}

