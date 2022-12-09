using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

using UnityEngine.UIElements;
using RiptideNetworking;

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

[RequireComponent(typeof(CardView))]
public class NetworkCard : MonoBehaviour
{

   
    private CardView _cardView;


    private Vector3 lastPos, lastScale, lastRotation;
    private string lastParent, lastSortLayer;
    private int lastSiblingIndex;
    private int lastCardSort;

    /// <summary>
    /// The position of the transform if (0,0) was the Bottom left corner of the screen and (1,1) was the top right corner.
    /// </summary>
    public Vector3 NormalizedPosition
    {
        get
        {

            return transform.position * WorldCanvas.Instance.ScreenScale;

        }
    }

    private void Awake()
    {
        _cardView = GetComponent<CardView>();
    }



    private void LateUpdate()
    {

        //lastPos = Camera.main.WorldToScreenPoint(transform.position);
        lastPos = transform.position;

       
    }


   
    public void SendPosition()
    {
       
        Message message = Message.Create(MessageSendMode.unreliable, Senders.Position);
        message.AddString(_cardView.CardSessionId);
        message.AddVector3(NormalizedPosition);
        NetworkPipeline.SendMessageToServer(message);
        
    }


   
    
    public void SendRotation()
    {
        lastRotation = transform.localEulerAngles;
        Message message = Message.Create(MessageSendMode.unreliable, Senders.Rotation);
        message.AddString(_cardView.CardSessionId);
        message.AddVector3(lastRotation);
        NetworkPipeline.SendMessageToServer(message);
    }


   
    public void SendParent(string parentSlot = "")
    {
        if (string.IsNullOrEmpty(parentSlot))
        {
            lastSiblingIndex = -1;
        }
        else
        {
            lastSiblingIndex = transform.GetSiblingIndex();
        }
        

        Message message = Message.Create(MessageSendMode.reliable, Senders.Parent);
        message.AddString(_cardView.CardSessionId);
        message.AddString(parentSlot);
        message.AddInt(lastSiblingIndex);
        message.AddVector3(_cardView.GetScale());
        NetworkPipeline.SendMessageToServer(message);

    }


   
    public void SendSorting()
    {
        //lastSortLayer = _cardView.sp.SortLayerName;
        //lastCardSort = _cardView.sp.SortOrder;

        lastSortLayer = _cardView.CurrentConfig.BaseSortLayer;
        lastCardSort = _cardView.CurrentConfig.BaseSortOrder;

        Message message = Message.Create(MessageSendMode.unreliable, Senders.Sorting);
        message.AddString(_cardView.CardSessionId);
        message.AddString(lastSortLayer);
        message.AddInt(lastCardSort);
        NetworkPipeline.SendMessageToServer(message);
    }

    public void SendScale(Vector3 scale)
    {
        Message message = Message.Create(MessageSendMode.unreliable, Senders.Scale);
        message.AddString(_cardView.CardSessionId);
        message.AddVector3(scale);
        NetworkPipeline.SendMessageToServer(message);
    }

    public void Flip(bool toFacedown)
    {
        Message message = Message.Create(MessageSendMode.reliable, Senders.Flip);
        message.AddString(_cardView.CardSessionId);
        message.AddBool(toFacedown);
        NetworkPipeline.SendMessageToServer(message);
    }

   
}
