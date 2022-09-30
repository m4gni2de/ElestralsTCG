using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UploadedDeckDTO
{
    public string deckKey { get; set; }
    public string title { get; set; }
    public List<string> deck { get; set; }
    
}
