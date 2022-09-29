using System.Collections;
using System.Collections.Generic;
using SimpleSQL;
using UnityEngine;

namespace Databases
{
    public class SettingsDTO
    {
        [PrimaryKey]
        public string settingsKey { get; set; }
        public string settingsValue { get; set; }
    }
}

