using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iOwnCanvas : iSortRenderer
{
    Canvas canvas { get; }
}
