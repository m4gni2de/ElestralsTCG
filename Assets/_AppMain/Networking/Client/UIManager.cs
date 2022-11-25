using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIManager : MonoBehaviour
{
    private static UIManager _singleton;
    public static UIManager Singleton
    {
        get => _singleton;
        set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(UIManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    [Header("Connect")]
    [SerializeField] private GameObject connectUI;
    [SerializeField] private TMP_InputField usernameField;

    private void Awake()
    {
        Singleton = this;
    }
    public void ClickConnect()
    {
        usernameField.interactable = false;
        connectUI.SetActive(false);

        //ClientManager.Instance.Connect();
    }

    public void BackToMain()
    {
        usernameField.interactable = true;
        connectUI.SetActive(true);
    }

    public void SendName()
    {
        //Message message = Message.Create(MessageSendMode.reliable, (ushort)c2s.registerPlayer);
        //message.AddString(usernameField.text);
        //ClientManager.Instance.Client.Send(message);
    }

}
