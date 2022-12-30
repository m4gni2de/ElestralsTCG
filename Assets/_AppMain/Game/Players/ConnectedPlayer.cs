using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Bson;
using nsSettings;
using RiptideNetworking;
using UnityEngine;

namespace Gameplay
{
   
}
public class ConnectedPlayer 
{
    #region Static Properties
    public static ConnectedPlayer Create(ushort serverId)
    {
        ConnectedPlayerDTO _data = new ConnectedPlayerDTO();

        _data.lobby = "";
        _data.sleeves = SettingsManager.Account.Settings.Sleeves;
        _data.playmatt = SettingsManager.Account.Settings.Playmatt;
        _data.userId = App.Account.Id;
        _data.username = App.Account.Name;
        _data.serverId = serverId;
        _data.whenConnect = DateTime.MinValue;

        return new ConnectedPlayer(_data);
    }
    #endregion

    #region Properties

    private ConnectedPlayerDTO _data = null;
    public ConnectedPlayerDTO playerData
    {
        get
        {
            if (_data == null || isDirty)
            {
                _data = new ConnectedPlayerDTO();
                _data.lobby = "";
                _data.sleeves = SettingsManager.Account.Settings.Sleeves;
                _data.playmatt = SettingsManager.Account.Settings.Playmatt;
                _data.userId = App.Account.Id;
                _data.username = App.Account.Name;
                _data.serverId = ClientManager.GetClientId();
                _data.whenConnect = DateTime.Now;

            }
            return _data;
        }
    }
    public ushort ServerId { get { return (ushort)playerData.serverId; } }
    public string lobby { get { return playerData.lobby; } }
    public DateTime ConnectTime { get { return playerData.whenConnect; } }


    private bool _isDirty = false;
    public bool isDirty { get { return _isDirty; } set { _isDirty = value; } }

    private bool _isRegistered = false;
    public bool isRegistered { get { return _isRegistered; }set { _isRegistered = value; } }

    #endregion

    #region Functions
   
    public void SetDirty(bool dirty)
    {
        _isDirty = dirty;
    }
    #endregion


    #region Initialization
    public ConnectedPlayer(ConnectedPlayerDTO dto)
    {
        SetData(dto);
        _isRegistered = false;
    }
    
    public void SetData(ConnectedPlayerDTO dto)
    {
        _data = dto;
        SetDirty(false);
       
    }
    #endregion


    #region Connection Management
    public async Task<bool> Connect()
    {
        _isRegistered = await RemoteData.RegisterPlayer(playerData.serverId);
        return _isRegistered;
    }
    public async Task<bool> UpdateServer()
    {
        _isRegistered = false;
        _isRegistered = await RemoteData.UpdatePlayer(playerData.serverId);
        return _isRegistered;
    }
    
    #endregion
}
