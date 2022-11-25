using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor.ShaderGraph.Internal;
#endif

public class CustomLayoutGroup : GridLayoutGroup
{
    private static float MaxRotate = 90f;
    public int Capacity = 8;
    public override void SetLayoutHorizontal()
    {

        base.SetLayoutHorizontal();


        for (int i = 0; i < rectChildren.Count; i++)
        {
            float rotateVal = ((float)i / (float)Capacity) * MaxRotate;
            if (i > 0 && !i.IsEvenNumber())
            {
                rotateVal = -rotateVal;
                rectChildren[i].transform.localEulerAngles = new Vector3(0f, 0f, rotateVal);
            }

        }
    }


}
