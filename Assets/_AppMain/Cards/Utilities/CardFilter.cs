using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class CardFilter 
{
    private List<CardType> _cardTypes = null;
    public List<CardType> CardTypeScope { get { _cardTypes ??= new List<CardType>(); return _cardTypes; } }
    private List<CardLocation> _locations = null;
    public List<CardLocation> LocationScope { get { _locations ??= new List<CardLocation>(); return _locations; } }


}
