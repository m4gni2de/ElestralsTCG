using System.Collections;
using System.Collections.Generic;
using Decks;
using Gameplay;
using UnityEngine;

public class NetworkPlayer 
{
    public ushort networkId { get; private set; }
    public string userId { get; private set; }
    public bool IsLocal { get; private set; }
    public string deckKey { get; private set; }
    public string deckName { get; private set; }


    public NetworkPlayer(ushort netId, string id, string key, string name, bool isLocal)
    {
        networkId = netId;
        userId = id;
        this.IsLocal = isLocal;
        deckKey = key;
        deckName = name;
    }

    public void AddDeck(string key, string name)
    {
        
    }
}
