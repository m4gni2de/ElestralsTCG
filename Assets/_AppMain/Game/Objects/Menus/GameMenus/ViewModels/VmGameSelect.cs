using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VmGameSelect : ViewModel
{

    #region Properties
    [SerializeField]
    private TMP_Text hostText;
    [SerializeField]
    private TMP_Text playerCountText;
    [SerializeField]
    private Button joinButton;
    private RemoteLobbyDTO _lobby = null;
    public RemoteLobbyDTO Lobby
    {
        get
        {
            return _lobby;
        }
    }
    #endregion



    public void Load(RemoteLobbyDTO dto)
    {
        _lobby = dto;
        hostText.text = dto.player1;

        
        if (string.IsNullOrEmpty(dto.player2))
        {
            playerCountText.text = "1/2";
            joinButton.interactable = true;
        }
        else
        {
            playerCountText.text = "2/2";
            joinButton.interactable = false;
        }
    }

    public override void Refresh()
    {
       if (Lobby != null)
        {
            RefreshLobby();
        }
    }

    private async void RefreshLobby()
    {
        RemoteLobbyDTO dto = await RemoteData.GetLobby(Lobby.lobbyKey);
        Load(dto);
    }

    public void Freeze()
    {
        joinButton.interactable = false;
    }

}
