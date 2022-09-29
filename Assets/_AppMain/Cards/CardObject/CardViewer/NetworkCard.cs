using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

[RequireComponent(typeof(NetworkObject))]
public class NetworkCard : CardView
{
    public static Dictionary<ushort, NetworkCard> localCards = new Dictionary<ushort, NetworkCard>();
    public static Dictionary<ushort, NetworkCard> remoteCards = new Dictionary<ushort, NetworkCard>();


    private NetworkObject netObject { get; set; }
    private void Reset()
    {
        if (GetComponent<NetworkObject>() == null) { gameObject.AddComponent<NetworkObject>(); }
    }

    private void Awake()
    {
        netObject = GetComponent<NetworkObject>();
    }

    private void LateUpdate()
    {
        
    }

    #region Client

    #region Messages

    #endregion

    #endregion



    #region Server

    #region Messages

    #endregion

    #endregion
}
