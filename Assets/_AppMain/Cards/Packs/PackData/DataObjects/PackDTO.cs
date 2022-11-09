using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;
using System;

namespace Packs
{
    public class GameSetDTO
    {
        public int setIndex { get; set; }
        public string setName { get; set; }
        public string setAbbr { get; set; }
        public DateTime releaseDate { get; set; }
        public int cardCount { get; set; }
        public string stamp { get; set; }
    }
}

