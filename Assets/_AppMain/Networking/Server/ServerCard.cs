using System.Collections;
using System.Collections.Generic;
using Gameplay;
using RiptideNetworking;
using UnityEngine;

[System.Serializable]
public class ServerCard
{
    public enum Senders : ushort
    {
        Position = 901,
        Parent = 902,
        Rotation = 903,
        Sorting = 904,
        Scale = 905,
        Flip = 906,
    }

    public enum Receivers
    {
        Position = 901,
        Parent = 902,
        Rotation = 903,
        Sorting = 904,
        Scale = 905,
        Flip = 906,
    }

    public string owner { get; private set; }
    public int localIndex;
    public string uniqueId;
    public string setKey;
    public string slotId;



    public ServerCard(string owner, int localId, string uniqueId, string setKey, string slot)
    {
        this.owner = owner;
        localIndex = localId;
        this.setKey = setKey;
        this.uniqueId = uniqueId;
        this.slotId = slot;
    }



    #region Messages
    [MessageHandler((ushort)Senders.Position)]
    private static void CardPositionChange(ushort fromClientId, Message message)
    {

        Message outbound = Message.Create(MessageSendMode.unreliable, (ushort)Receivers.Position);
        outbound.AddString(message.GetString());
        outbound.AddVector3(message.GetVector3());
        ServerGame.Instance.MessageSendToOpponent(outbound, fromClientId);
    }

    [MessageHandler((ushort)Senders.Parent)]
    private static void ParentChange(ushort fromClientId, Message message)
    {
        string cardId = message.GetString();
        string parent = message.GetString();
        int sibling = message.GetInt();
        Vector3 scale = message.GetVector2();

        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)Receivers.Parent);
        outbound.AddString(cardId);
        outbound.AddString(parent);
        outbound.AddInt(sibling);
        outbound.AddVector3(scale);
        ServerGame.Instance.MessageSendToOpponent(outbound, fromClientId);
    }


    [MessageHandler((ushort)Senders.Sorting)]
    private static void SortingChange(ushort fromClientId, Message message)
    {
        string cardId = message.GetString();
        string layer = message.GetString();
        int cardOrder = message.GetInt();

        Message outbound = Message.Create(MessageSendMode.unreliable, (ushort)Receivers.Sorting);
        outbound.AddString(cardId);
        outbound.AddString(layer);
        outbound.AddInt(cardOrder);
        ServerGame.Instance.MessageSendToOpponent(outbound, fromClientId);
    }

    [MessageHandler((ushort)Senders.Rotation)]
    private static void RotationChange(ushort fromClientId, Message message)
    {
        string cardId = message.GetString();
        Vector3 rotation = message.GetVector3();

        Message outbound = Message.Create(MessageSendMode.unreliable, (ushort)Receivers.Rotation);
        outbound.AddString(cardId);
        outbound.AddVector3(rotation);
        ServerGame.Instance.MessageSendToOpponent(outbound, fromClientId);
    }

    [MessageHandler((ushort)Senders.Scale)]
    private static void ScaleChange(ushort fromClientId, Message message)
    {
        string cardId = message.GetString();
        Vector3 scale = message.GetVector3();

        Message outbound = Message.Create(MessageSendMode.unreliable, (ushort)Receivers.Scale);
        outbound.AddString(cardId);
        outbound.AddVector3(scale);
        ServerGame.Instance.MessageSendToOpponent(outbound, fromClientId);
    }

    [MessageHandler((ushort)Senders.Flip)]
    private static void FlipCard(ushort fromClientId, Message message)
    {
        string cardId = message.GetString();
        bool toFacedown = message.GetBool();

        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)Receivers.Flip);
        outbound.AddString(cardId);
        outbound.AddBool(toFacedown);
        ServerGame.Instance.MessageSendToOpponent(outbound, fromClientId);
    }
    #endregion
}
