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


[System.Serializable]
public class DownloadedDeckDTO
{
    public string deckKey { get; set; }
    public string title { get; set; }
    public List<string> deck { get; set; }
    public string uploadCode { get; set; }
    public string owner { get; set; }

}
