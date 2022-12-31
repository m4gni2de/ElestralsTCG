using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.P2P
{
    [System.Serializable]
    public class ChatDTO
    {
        public string id { get; set; }
        public string gameId { get; set; }
        public string sender { get; set; }
        public string originalContent { get; set; }
        public string updatedContent { get; set; }
        public int type { get; set; }
        public string whenSend { get; set; }
        public int deleted { get; set; }
        public int edited { get; set; }
    }
}
