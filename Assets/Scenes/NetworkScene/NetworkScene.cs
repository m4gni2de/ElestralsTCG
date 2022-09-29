using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RiptideNetworking;
using UnityEngine.UI;

public class NetworkScene : MonoBehaviour
{
    public static string SceneName = "NetworkLobby";

    public TMP_InputField ipInput;
    public GameObject mainObject;
    public Button hostButton, connectButton;

    private void Start()
    {
        
    }

    #region Buttons
    public void HostMode()
    {
        ipInput.interactable = false;
        mainObject.SetActive(false);
        NetworkManager.Instance.Create(NetworkManager.NetworkMode.Host);
       
        //GameManager.HostGame();

    }

    public void ConnectMode()
    {

        ipInput.interactable = false;
        mainObject.SetActive(false);
        NetworkManager.Instance.Create(NetworkManager.NetworkMode.Client);
        if (string.IsNullOrEmpty(ipInput.text)){ ipInput.text = NetworkManager.localIp; }

        NetworkManager.Instance.Connect(ipInput.text);
        //ClientManager.Instance.Connect();
    }
    #endregion
}
