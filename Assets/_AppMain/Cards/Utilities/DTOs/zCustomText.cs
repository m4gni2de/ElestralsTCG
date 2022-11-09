using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

[System.Serializable]
public class zCustomText
{
    [PrimaryKey]
    public string charKey { get; set; }
    public string encodedVal { get; set; }
    public string title { get; set; }
    public string unicode { get; set; }
    public string colorHex { get; set; }
}
