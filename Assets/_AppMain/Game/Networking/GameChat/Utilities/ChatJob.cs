using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameEvents;
using RiptideNetworking;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Gameplay.P2P
{
    public struct ChatJob : iNetworkGroup
    {
        //public ChatMessage message;
        public string chatId;
        public string gameId;
        public MessageGroupId groupId { get { return MessageGroupId.GameChat; } }
        public NativeArray<bool> sendResults;
        public bool didSend;
        public bool isComplete;

        public float sendTimeoutSeconds;


        #region Functions
        private bool HasResponse()
        {
            return isComplete == true;
        }
        #endregion

       
        //public async void Execute()
        //{
        //    sendResults[0] = await SendChatToServer();
        //}

        //private async Task<bool> SendChatToServer()
        //{
        //    GameChat.SendNewChatToServer(gameId, chatId);
        //    GameChat.OnNewChatResponse += AwaitChatResponse;
        //    isComplete = false;
        //    StartSendTimer().Start();
        //    await TaskUtils.WaitUntil(HasResponse);
        //    return didSend;

        //}

        //private async Task StartSendTimer()
        //{
        //    float acumTime = 0f;
        //    do
        //    {
        //        await Task.Delay(1000);
        //        acumTime += Time.deltaTime;

        //    } while (true && acumTime < sendTimeoutSeconds && !isComplete);

        //    if (isComplete) { return; }
        //    if (acumTime >= sendTimeoutSeconds)
        //    {
        //        AwaitChatResponse("error");
        //    }
        //}
        //private void AwaitChatResponse(string chatId)
        //{
        //    GameChat.OnNewChatResponse -= AwaitChatResponse;
        //    didSend = !chatId.IsEmpty();
        //    isComplete = true;
        //}

       

    }
}

