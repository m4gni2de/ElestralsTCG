using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UploadedDeckDTO
{
    public string uploadCode { get; set; }
    public List<string> deck { get; set; }
    public string owner { get; set; }
}
