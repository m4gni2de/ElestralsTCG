using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RiptideNetworking;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Gameplay.P2P
{
   

    public class GameChat : iNetworkGroup
    {
        #region Interfaces
        public MessageGroupId groupId { get { return MessageGroupId.GameChat; } }

        #endregion

        #region Properties
        public string chatId { get; set; }


        private List<ChatMessage> _messages = null;
        public List<ChatMessage> Messages
        {
            get
            {
                _messages ??= new List<ChatMessage>();
                return _messages;
            }
        }

        #endregion

        #region Functions
        private ChatMessage FindMessage(string id)
        {
            for (int i = 0; i < Messages.Count; i++)
            {
                if (Messages[i].id.ToLower() == id.ToLower()) { return Messages[i]; }
            }
            return null;
        }
        private bool IsDuplicate(ChatMessage msg)
        {
            for (int i = 0; i < Messages.Count; i++)
            {
                if (Messages[i].id.ToLower() == msg.id.ToLower()) { return true; }
            }
            return false;
        }
        #endregion

        #region Life Cycle
        public GameChat(string gameId)
        {
            chatId = gameId;
            OnNewChatRecieved += AddMessage;
        }
        #endregion


        #region Message Creating
        public void TrySendMessage(string sender, string msg, MessageType ty = MessageType.Normal)
        {
            string id = UniqueString.CreateId(4, chatId);
            ChatMessage chat = new ChatMessage(id, chatId, sender, msg, ty);
            if (GameManager.IsOnline)
            {
                //SendChatToServer(chat);
                SendNewChatToServer(chatId, chat);
                AddMessage(chat);
            }
            else
            {
                //SendChatToServer(chat);
                AddMessage(chat);
            }
            
            

        }


        public event Action<ChatMessage> OnNewMessageAdded;
        private void AddMessage(ChatMessage message)
        {
            if (!IsDuplicate(message))
            {
                Messages.Add(message);
                OnNewMessageAdded?.Invoke(message);
            }
        }

        private void DeleteMessage(string messageId)
        {
            ChatMessage toDelete = FindMessage(messageId);
            if (toDelete != null)
            {
                toDelete.DeleteMessage();
            }
        }
        private void EditMessage(string messageId, string newContent)
        {
            ChatMessage toDelete = FindMessage(messageId);
            if (toDelete != null)
            {
                toDelete.Edit(newContent);
            }
        }
        #endregion



        #region Networking

        private List<ChatJob> _jobQueue = null;
        public List<ChatJob> JobQueue
        {
            get
            {
                _jobQueue ??= new List<ChatJob>();
                return _jobQueue;
            }
        }

        private async void SendChatToServer(ChatMessage chat)
        {
            bool didSend = await Task.Run(chat.SendChatToServer);

            if (didSend)
            {
                AddMessage(chat);
            }
            //NativeArray<bool> result = new NativeArray<bool>(1, Allocator.TempJob);
            //ChatJob job = new ChatJob();

            //job.chatId = chat.id;
            //job.gameId = chat.gameId;
            //job.sendResults = new NativeArray<bool>();
            //job.didSend = false;
            //job.isComplete = false;
            //job.sendTimeoutSeconds = 15f;
            //job.sendResults = result;
            //JobQueue.Add(job);

            //JobHandle handle = job.Schedule();

            //handle.Complete();
            //bool didSend = job.sendResults[0];

            //if (didSend)
            //{
            //    AddMessage(chat);
            //}
            //result.Dispose();
        }

        //private async Task<bool> SendChatToServer(ChatMessage chat)
        //{
        //    SendNewChatToServer(chat.gameId, chat);
        //    OnNewChatResponse += AwaitChatResponse;
        //    //StartSendTimer().Start();
        //    await TaskUtils.WaitUntil(chat.IsComplete);
        //    return didSend;

        //}
        //private void AwaitChatResponse(string chatId)
        //{
        //    GameChat.OnNewChatResponse -= AwaitChatResponse;
        //    didSend = !chatId.IsEmpty();
        //    isComplete = true;
        //}

        //private void StartChatJob(ChatJob job, ChatMessage chat)
        //{



        //}
        #endregion


        #region Network Messaging

        #region SendNewChat
        public static void SendNewChatToServer(string gameId, ChatMessage chat)
        {
            string json = chat.Print;

            Message message = Message.Create(MessageSendMode.reliable, (ushort)ChatActivity.NewChat);
            message.Add(gameId);
            message.Add(json);

            NetworkPipeline.SendMessageToServer(message);
        }

        public static event Action<string> OnNewChatResponse;
        [MessageHandler((ushort)ChatActivity.NewChatResponse)]
        public static void GetNewChatReponse(Message message)
        {
            ushort sender = message.GetUShort();
            string chatId = message.GetString();
            OnNewChatResponse?.Invoke(chatId);
        }

        public static event Action<ChatMessage> OnNewChatRecieved;
        [MessageHandler((ushort)ChatActivity.NewChat)]
        public static void GetNewChat(Message message)
        {
            ushort sender = message.GetUShort();
            string chatJson = message.GetString();

            // ChatMessage chat = ChatMessage.FromJson(chatJson);
            ChatMessage chat = JsonUtility.FromJson<ChatMessage>(chatJson);
            if (chat != null)
            {
                OnNewChatRecieved?.Invoke(chat);
            }
            
        }
        #endregion

        #region Inbound


        #endregion
        #endregion




    }

}

