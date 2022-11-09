using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iCustomFont
{
    string EncodedText { get; }
    string UnicodeString { get; }
    string UnicodeWithColor { get; }
}
