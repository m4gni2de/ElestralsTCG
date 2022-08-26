using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Databases.Views
{
    [System.Serializable]
    public class qCardLookup
    {
        [PrimaryKey]
        public string cardKey { get; set; }
        public string title { get; set; }
        public int cardClass { get; set; }
        public string setName { get; set; }
    }
}

