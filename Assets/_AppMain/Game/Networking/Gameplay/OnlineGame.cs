using System.Collections;
using System.Collections.Generic;
using Decks;
using nsSettings;
using RiptideNetworking;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gameplay.Networking
{
    public class OnlineGame : Game
    {
        public static Dictionary<ushort, Player> ServerPlayers = new Dictionary<ushort, Player>();

        #region Properties
        public string ip;
        public ushort port;
        #endregion

        #region Overrides
        public override bool IsOnline() { return true; }
        #endregion
        public OnlineGame(string gameId, string ip, ushort port)
        {
            this.gameId = gameId;
            this.ip = ip;
            this.port = port;
            
        }





        #region Server Networking
        [MessageHandler((ushort)c2s.registerPlayer)]
        private static void PlayerToServer(ushort fromClientId, Message message)
        {
            AddServerPlayer(fromClientId, message.GetString(), message.GetString());
        }
        public static void AddServerPlayer(ushort id, string username, string decklist)
        {
            foreach (Player otherPlayer in ServerPlayers.Values)
            {
                Message message = Message.Create(MessageSendMode.reliable, (ushort)s2c.playerAdded);
                message.AddUShort(id);
                message.AddString(username);
                message.AddString(decklist);
                SendSpawned(id, message);
            }
                


            //Player p = new Player(id, username, decklist);
            //SendSpawned(id, username, decklist);
            //ServerPlayers.Add(id, p);
        }

        [MessageHandler((ushort)s2c.playerAdded)]
        private static void PlayerFromServer(ushort fromClientId, Message message)
        {
            AddServerPlayer(fromClientId, message.GetString(), message.GetString());
        }
        private static void SendSpawned(ushort toClientId, Message message)
        {
            ServerManager.Instance.Server.Send(message, toClientId);
        }
        private static void SendSpawned(ushort id, string username, string decklist)
        {
            Message message = Message.Create(MessageSendMode.reliable, (ushort)s2c.playerAdded);
            message.AddUShort(id);
            message.AddString(username);
            message.AddString(decklist);
            ServerManager.Instance.Server.SendToAll(message);
        }
        #endregion
    }
}

