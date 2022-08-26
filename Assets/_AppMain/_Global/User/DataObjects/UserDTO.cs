using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;
using System;

namespace Databases
{
    [System.Serializable]
    public class UserDTO
    {
        [PrimaryKey]
        public string userKey { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public DateTime whenCreated { get; set; }
    }
}

