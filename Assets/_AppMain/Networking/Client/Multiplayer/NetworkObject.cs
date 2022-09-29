using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using nsSettings;
using Newtonsoft.Json.Schema;

public class NetworkObject : MonoBehaviour
{
    public enum c2sTransform : ushort
    {
        Position = 1,
        Rotation = 2,
        Scale = 3,
        All = 4,
    }
    public enum s2cTransform : ushort
    {
        Position = 1,
        Rotation = 2,
        Scale = 3,
        All = 4,
    }

    #region Function Messages
    private Message TransformMessage(Message message)
    {
        message.AddUShort(networkId);
        message.AddVector3(transform.position);
        message.AddVector3(transform.localEulerAngles);
        message.AddVector3(transform.localScale);
        return message;
    }
    #endregion

    public static Dictionary<ushort, NetworkObject> list = new Dictionary<ushort, NetworkObject>();

    public static void NewObject(NetworkObject obj)
    {
        ushort index = (ushort)list.Count;
        obj.SetNetworkId(index);
        list.Add(index, obj);
    }

    public static void RemoveObject(ushort id)
    {
        list.Remove(id);

    }

   

    #region Properties
    public bool IsLocal { get; private set; }
    public ushort networkId { get; private set; }
    
    public Vector3 lastPosition { get; private set; }
    
    private bool isDirty = false;
    //sync the transform of this GameObject across the network
    [SerializeField] private bool transformSync = true;
    #endregion
    public void SetNetworkId(ushort id)
    {
        this.networkId = id;
    }

    
    private void LateUpdate()
    {
        if (transformSync) { SyncTransform(); }
    }

    #region Messages
    private void SyncTransform()
    {
        if (IsLocal)
        {
            if (transform.position != lastPosition)
            {
                SendPosition();
            }
            SendClientTransform();
        }
    }
    

    #endregion

    #region Client
    public void SetPositionFromServer(Vector3 pos)
    {
        transform.position = pos;
    }
    public void SetRotationFromServer(Vector3 rot)
    {
        transform.localEulerAngles = rot;
    }
    public void SetScaleFromServer(Vector3 scale)
    {
        transform.localScale = scale;
    }
    private void SendClientTransform()
    {
        Message message = TransformMessage(Message.Create(MessageSendMode.unreliable, (ushort)c2sTransform.All));
        ClientManager.Instance.Client.Send(message);
    }
    private void SendPosition()
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)c2sTransform.Position);
        message.AddUShort(networkId);
        message.AddVector3(transform.position);
        ClientManager.Instance.Client.Send(message);
    }

    #region Message Listeners
    [MessageHandler((ushort)s2cTransform.All)]
    private static void GetClientTransform(ushort fromServerId, Message message)
    {
        NetworkObject obj = list[message.GetUShort()];
        if (!obj.IsLocal)
        {
            obj.SetPositionFromServer(message.GetVector3());
            obj.SetRotationFromServer(message.GetVector3());
            obj.SetScaleFromServer(message.GetVector3());
        }
    }
    #endregion

    #endregion



    #region Server

    //from SendClientTransform()
    //[MessageHandler((ushort)c2sTransform.All)]
    //private static void Transform(ushort fromClientId, Message message)
    //{
    //    SetAndSendTransform(message.GetUShort(), message.GetVector3(), message.GetVector3(), message.GetVector3());
    //}
    public static void SetAndSendTransform(ushort id, Vector3 newPos, Vector3 rotation, Vector3 scale)
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)s2cTransform.Position);
        message.AddUShort(id);
        message.AddVector3(newPos);
        message.AddVector3(rotation);
        message.AddVector3(scale);
        ServerManager.Instance.Server.Send(message, id);
    }

    #endregion
}
