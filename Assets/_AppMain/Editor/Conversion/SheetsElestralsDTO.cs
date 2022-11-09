using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Conversion
{
    public class SheetsElestralsDTO
    {
        public int cardMade { get; set; }
        public string ElestralName { get; set; }
        public string Codename { get; set; }
        public string Element1 { get; set; }
        public string Element2 { get; set; }
        public string Element3 { get; set; }
        public int Cost { get; set; }
        public string SubClass { get; set; }
        public string SubClass2 { get; set; }
        public string Artist { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public string Effect { get; set; }

    }

    [System.Serializable]
    public class DocsElestralDTO
    {
        public string SetName { get; set; }
        public string SetNumber { get; set; }
        public string ElestralName { get; set; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public string Type3 { get; set; }
        public string Subclass { get; set; }
        public string Subclass2 { get; set; }
        public string Artist { get; set; }
        public int? A { get; set; }
        public int? D { get; set; }
        public string Effect { get; set; }
        public string ElestralCodename { get; set; }
    }
    [System.Serializable]
    public class DocsRuneDTO
    {
        public string SetName { get; set; }
        public string SetNumber { get; set; }
        public string CardName { get; set; }
        public string ElestralCodename { get; set; }
        public string RuneType { get; set; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public string Type3 { get; set; }
        public string Artist { get; set; }
        public string Effect { get; set; }
       
    }
}

