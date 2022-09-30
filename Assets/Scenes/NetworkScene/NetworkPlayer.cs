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


    public NetworkPlayer(ushort netId, string id, bool isLocal)
    {

    }
}
