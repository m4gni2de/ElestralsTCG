using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class MobileScaling : MonoBehaviour
{
    RectTransform worldCanvas;
    RectTransform scalerCanvas;

    float height, width;
    public float widthVal, heightVal, offset;

    // Start is called before the first frame update
    void Start()
    {

        worldCanvas = WorldCanvas.Instance.GetComponent<RectTransform>();
        scalerCanvas = GetComponent<RectTransform>();

       
        //width = worldCanvas.rect.width;
        
       
        //Scale();

#if UNITY_EDITOR

        width = worldCanvas.rect.width;
        height = worldCanvas.rect.height;


#else
 width = UnityEngine.Device.Screen.safeArea.width;
        height = UnityEngine.Device.Screen.safeArea.height;
#endif
        widthVal = 960f;
        heightVal = 640f;
        offset = 1f;
        Scale();
    }

    public void Scale()
    {
        float xDiff = width / widthVal;
        float yDiff = worldCanvas.rect.height;

        if (width < widthVal)
        {
            xDiff = width / heightVal;
        }

        //float newX = (1f / xDiff) * offset;
        float newX = (offset / xDiff);
        float newY = (offset / xDiff);
        // newY = (1f / xDiff) * offset;
        scalerCanvas.transform.localScale = new Vector3(newX, newY, scalerCanvas.transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
