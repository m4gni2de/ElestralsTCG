using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Databases
{
    public class GameDTO
    {
        [PrimaryKey]
        public string gameId { get; set; }
        public string playerId { get; set; }
        public string deckKey { get; set; }
        public int sleeves { get; set; }
    }
}


