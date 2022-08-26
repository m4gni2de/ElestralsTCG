using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Databases
{
    [System.Serializable]
    public class vAltArt
    {
        public string baseKey { get; set; }
        public string altKey { get; set; }
        public string title { get; set; }
        public string setName { get; set; }
        public int artType { get; set; }
        public string altImageFile { get; set; }
    }
}

