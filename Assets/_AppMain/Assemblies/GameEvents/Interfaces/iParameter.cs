using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvents
{
    public interface iParameter
    {
        string Name { get; }
        object GetValue();
        void SetValue(object val);
    }
}
