using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Defective.JSON;
using Newtonsoft.Json;
using UnityEngine;

namespace Gameplay.P2P
{
    public enum MessageType
    {
        Normal = 0,
        Whisper = 1,
        Shout = 2,
        Alert = 3,
    }
    public class ChatMessage
    {

        public string id;
        public string gameId;
        public string sender;
        public string originalContent;
        public string updatedContent;
        public MessageType type;
        public DateTime whenSend;
        public bool deleted;
        public bool edited;

        public string Content
        {
            get
            {
                if (deleted || edited) { return updatedContent; }
                return originalContent;
            }
        }

        //public bool didSend = false;
        //public bool isComplete = false;
        //public bool IsComplete()
        //{
        //    return isComplete;
        //}

        #region Converters
        public string Print
        {
            get
            {
                JSONObject o = new JSONObject();
                o.AddField("id", this.id);
                o.AddField("gameId", this.gameId);
                o.AddField("sender", this.sender);
                o.AddField("originalContent", this.originalContent);
                o.AddField("updatedContent", this.updatedContent);
                o.AddField("type", (int)this.type);
                o.AddField("whenSend", this.whenSend.ToString());
                o.AddField("deleted", this.deleted.BoolToInt());
                o.AddField("edited", this.edited.BoolToInt());

                return o.Print();
            }
        }
        public ChatDTO GetChatDto()
        {
            ChatDTO dto = new ChatDTO
            {
                id = this.id,
                gameId = this.gameId,
                sender = this.sender,
                originalContent = this.originalContent,
                updatedContent = this.updatedContent,
                type = (int)this.type,
                whenSend = this.whenSend.ToString(),
                deleted = this.deleted.BoolToInt(),
                edited = this.edited.BoolToInt(),
            };
            return dto;

        }
        #endregion

        #region Initialization

        ChatMessage(ChatDTO dto)
        {
            id = dto.id;
            gameId = dto.gameId;
            sender = dto.sender;
            originalContent = dto.originalContent;
            updatedContent= dto.updatedContent;
            type = (MessageType)dto.type;
            whenSend = DateTime.Parse(dto.whenSend);
            deleted = dto.deleted.IntToBool();
            edited = dto.edited.IntToBool();
        }
        public ChatMessage(string messageId, string gameId, string author, string msg, MessageType ty = MessageType.Normal)
        {
            this.id = messageId;
            this.gameId = gameId;
            sender = author;
            originalContent = msg;
            updatedContent = "";
            type = ty;
            whenSend = DateTime.UtcNow;
            deleted = false;
            edited = false;
        }
        public static ChatMessage FromJson(string json)
        {
            var array = new JSONObject(json);
            if (!array.isNull)
            {
                foreach (var prop in array)
                {
                    ChatDTO dto = new ChatDTO();
                    dto.id = prop[0].stringValue;
                    dto.gameId = prop[1].stringValue;
                    dto.sender = prop[2].stringValue;
                    dto.originalContent = prop[3].stringValue;
                    dto.updatedContent = prop[4].stringValue;
                    dto.type = prop[5].intValue;
                    dto.whenSend = prop[6].stringValue;
                    dto.deleted = prop[7].intValue;
                    dto.edited = prop[8].intValue;
                    return new ChatMessage(dto);
                }

                return null;
            }
            return null;
        }

        #endregion

        public void DeleteMessage()
        {
            if (deleted) return;
            deleted = true;
            updatedContent = $"Message Deleted at {DateTime.UtcNow.ToLocalTime().ToShortTimeString()}";
        }
        public void Edit(string newContent)
        {
            edited = true;
            updatedContent = $"(Edited) - {newContent}";
            
        }


        public async Task<bool> SendChatToServer()
        {
            GameChat.SendNewChatToServer(gameId, this);
            GameChat.OnNewChatResponse += AwaitChatResponse;
            //StartSendTimer().Start();


            //await TaskUtils.WaitUntil(IsComplete);
            //return didSend;

            return true;

        }
        private void AwaitChatResponse(string chatId)
        {
            //GameChat.OnNewChatResponse -= AwaitChatResponse;
            //didSend = !chatId.IsEmpty();
            //isComplete = true;
        }
    }
}

