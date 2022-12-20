using System;
using System.Collections;
using System.Collections.Generic;
using Databases;
using UnityEngine;
using Cards.Collection;

public class CardCollection
{
    #region CollectionData
    public class CollectionData : iArchive
    {
        #region Interface
        public string Print
        {
            get
            {
                string p = $"Quantity of {rarity} '{cardKey}': {quantity}.";
                return p;
            }
           
        }
        #endregion

        public string cardKey;
        public Rarity rarity;
        public int quantity;
        public DateTime? lastAcquired;

        CollectionData(string key, int rare, int qty, DateTime? dtWhen)
        {
            this.cardKey = key;
            this.rarity = (Rarity)rare;
            this.quantity = qty;
            if (dtWhen.HasValue) { this.lastAcquired = dtWhen.Value; } else { this.lastAcquired = null; }
                

        }

        #region Operators
        public static implicit operator CollectionData(CardCollectionDTO dto)
        {
            return new CollectionData(dto.setKey, dto.rarity, dto.qty, dto.colWhen);
        }
        #endregion
    }
    #endregion

    #region Properties
    private Archive<CollectionData> _collectionHistory = null;
    public Archive<CollectionData> CollectionHistory
    {
        get
        {
            _collectionHistory ??= new Archive<CollectionData>();
            return _collectionHistory;
        }
    }
    #endregion

    #region Functions
    public static int GetQuantity(Card card)
    {
        return CardCollectionService.QuantityOf(card);
    }
    #endregion

    #region Instance/Initialization
    public static CardCollection Instance { get; private set; }
    public static void GenerateCollection()
    {
        if (Instance != null) { return; }
        Instance = new CardCollection();
        Instance.Generate();
    }
        
    private void Generate()
    {
        //int cardsAdded = CardCollectionService.NewCardsAdded();
        //App.Log($"Card Collection updated with {cardsAdded} additional cards!");
    }
    #endregion


    #region Collection Management
    public void AddQuantity(string setKey, int rarity, int qty)
    {
        CollectionData current = CardCollectionService.FindData(setKey, rarity);
        current.quantity += qty;
        current.lastAcquired = DateTime.Now;

        CollectionHistory.Add(current);

    }
    public void RemoveQuantity(string setKey, int rarity, int qty)
    {
        CollectionData current = CardCollectionService.FindData(setKey, rarity);

        int newQty = current.quantity - qty; 
        if (newQty < 0) { newQty = 0; }
        current.quantity = newQty;
        CollectionHistory.Add(current);

    }

    public void Save()
    {
        if (CollectionHistory.Count == 0) { return; }

        for (int i = 0; i < CollectionHistory.Count; i++)
        {
            var data = CollectionHistory[i].Item;
            CardCollectionDTO dto = new CardCollectionDTO { setKey = data.cardKey, rarity = (int)data.rarity, qty = data.quantity, colWhen = data.lastAcquired };
            CardCollectionService.SaveCard(dto);
        }
        CollectionHistory.Clear();
        
    }
    #endregion


}


