using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;
using Gameplay;
using Gameplay.Networking;
using Defective.JSON;

public enum ServerToClient : ushort
{
    serverValue = 0,
    playerRegistered = 1,
    cardSpawned = 2,
    cardMoved = 3,
    deckOrderChanged = 4,
    actionDeclared = 5,
    actionCancelled = 6,
    actionConfirmed = 7,
    actionEnd = 8,

}

public enum ClientToServer : ushort
{
    serverValue = 0,
    registerPlayer = 1,
    cardSpawned = 2,
    cardMoved = 3,
    deckOrderChanged = 4,
    actionDeclared = 5,
    actionCancelled = 6,
    actionConfirmed = 7,
    actionEnd = 8,

}

public enum ServerFunction
{
    HasActiveAction = 1,
}

public class NetworkPipeline
{
    #region Inbound Messages
   
    [MessageHandler((ushort)ServerToClient.playerRegistered)]
    private static void PlayerRegistered(Message message)
    {
        Player.RegisterPlayer(message.GetUShort(), message.GetString(), message.GetString(), message.GetString(), message.GetString(), message.GetString());
    }

    [MessageHandler((ushort)ServerToClient.cardSpawned)]
    private static void CardSet(Message message)
    {
        Player.RegisterPlayer(message.GetUShort(), message.GetString(), message.GetString(), message.GetString(), message.GetString(), message.GetString());
    }

    [MessageHandler((ushort)ServerToClient.cardMoved)]
    private static void MoveCard(Message message)
    {
        ushort sender = message.GetUShort();
        string card = message.GetString();
        int oldSlot = message.GetInt();
        int newSlot = message.GetInt();

        if (sender != NetworkManager.Instance.Client.Id)
        {
            MoveAction ac = new MoveAction(GameManager.ByNetworkId(sender), Game.FindCard(card), Game.FindSlot(newSlot));
            GameManager.Instance.AddRemoteAction(ac);
        }
        //
    }
    //when a new action is declare by another player, the opponent chooses a response action. 
    [MessageHandler((ushort)ServerToClient.actionDeclared)]
    private static void ActionDeclared(Message message)
    {
       
        JSONObject obj = new JSONObject(message.GetString());

        if (obj.type == JSONObject.Type.Null)
        {
            return;
        }
        CardActionData data = new CardActionData(obj);
        bool isLocal = data.Value<string>(CardActionData.PlayerKey) == App.Account.Id;
       
        if (!isLocal)
        {
            CardAction ca = CardActionData.ParseData(data);
            GameManager.Instance.DeclareNetworkAction(ca);
        }
        else
        {
            GameManager.Instance.DeclareNetworkAction();
        }
        
    }

    [MessageHandler((ushort)ServerToClient.actionConfirmed)]
    private static void ActionConfirmed(Message message)
    {
        bool isLocal = message.DidYouSend();
        string key = message.GetString();
        ActionResult result = (ActionResult)message.GetInt();
        GameManager.Instance.ConfirmActiveAction(result);
    }
    #endregion



    #region Outbound Messages

    #region Constructors
    public static Message OutboundMessage<T>(MessageSendMode sendMode, ushort fromClientId, T obj)
    {
        Message m = Message.Create(sendMode, fromClientId);
        m.Add(obj);
        return m;
    }
    public static Message OutboundMessage<T1, T2>(MessageSendMode sendMode, ushort fromClientId, T1 obj1, T2 obj2)
    {
        Message m = Message.Create(sendMode, fromClientId);
        m.Add(obj1);
        m.Add(obj2);
        return m;
    }
    public static Message OutboundMessage<T1, T2, T3>(MessageSendMode sendMode, ushort fromClientId, T1 obj1, T2 obj2, T3 obj3)
    {
        Message m = Message.Create(sendMode, fromClientId);
        m.Add(obj1);
        m.Add(obj2);
        m.Add(obj3);
        return m;
    }
    #endregion
    public static void SendMessageToServer(Message message)
    {
        NetworkManager.Instance.Client.Send(message);
    }

    public static void SpawnNewCard(int cardNetworkId, int slotSpawned)
    {
        //Message outbound = OutboundMessage(MessageSendMode.reliable, (ushort)ClientToServer.cardSpawned, cardNetworkId, slotSpawned);
        //SendMessageToServer(outbound);
    }
    public static void SendDeckOrder(int deckSlotId, List<string> cardIds, List<string> cardBaseNames)
    {
        Message outbound = OutboundMessage<int>(MessageSendMode.reliable, (ushort)ClientToServer.deckOrderChanged, deckSlotId);
        outbound.AddStrings(cardIds.ToArray(), true, true);
        outbound.AddStrings(cardBaseNames.ToArray(), true, true);
        SendMessageToServer(outbound);
    }
    
    //-1 new slot means it did not yet have a slot before being added to one(aka, the start of the game)
    public static void SendNewCardSlot(string cardId, int oldSlot, int newSlot)
    {
        Message outbound = OutboundMessage<string, int, int>(MessageSendMode.reliable, (ushort)ClientToServer.cardMoved, cardId, oldSlot, newSlot);
        SendMessageToServer(outbound);
    }

    public static void SendActionDeclare(CardAction ac)
    {
        Message outbound = OutboundMessage<string>(MessageSendMode.reliable, (ushort)ClientToServer.actionDeclared, ac.ActionData.GetJson);
        SendMessageToServer(outbound);
        NetworkPipeline.GetValue("bool", "HasActiveAction");
    }
    //use this eventually for allowing the other player to counter plays or cards. for now, just send action end
    public static void SendActionDecision(CardActionData ac)
    {
        Message outbound = OutboundMessage<string, int>(MessageSendMode.reliable, (ushort)ClientToServer.actionConfirmed, ac.actionKey, (int)ac.GetResult());
        SendMessageToServer(outbound);
    }
    public static void SendActionEnd(CardActionData ac)
    {
        Message outbound = OutboundMessage<string>(MessageSendMode.reliable, (ushort)ClientToServer.actionEnd, ac.actionKey);
        SendMessageToServer(outbound);

        
    }

    public static void GetValue(string valType, string methodName)
    {
        Message outbound = OutboundMessage(MessageSendMode.reliable, (ushort)ClientToServer.serverValue, valType, methodName);
        SendMessageToServer(outbound);
    }
    #endregion


}

public class TransportMessage<T>
{
    public T Value { get; set; }
    public ushort fromCliendId
    {
        get
        {
            return (ushort)ClientToServer.serverValue;
        }
    }

    public TransportMessage(string propName)
    {

    }
}
