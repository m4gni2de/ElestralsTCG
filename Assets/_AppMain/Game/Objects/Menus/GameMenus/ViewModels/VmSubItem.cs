using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Menus
{
    public class VmSubItem : ViewModel
    {
        

        public void SetPosition(Vector3 newPosition, bool isLocal)
        {
            if (isLocal) { transform.localPosition = newPosition; } else { transform.position = newPosition; } 
        }
        public void SetRotation(float rotate)
        {
            transform.localEulerAngles = new Vector3(0f, 0f, rotate);
        }
    }
}

