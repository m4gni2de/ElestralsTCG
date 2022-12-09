using System.Collections;
using System.Collections.Generic;
using GlobalUtilities;
using UnityEngine;

public interface iCardData
{
    
    public string cardName { get; set; }
    public string cardKey { get; set; }
    public int cost1 { get; set; }
    public int cost2 { get; set; }
    public int cost3 { get; set; }
    public string effect { get; set; }
    public string artist { get; set; }  
    public int setNumber { get; set; }
    public string setCode { get; set; }
    public string image { get; set; }
    public Rarity rarity { get; set; }
    public ArtType artType { get; set; }
    public string setStamp1 { get; set; }
    public string baseKey { get; set; }
}
