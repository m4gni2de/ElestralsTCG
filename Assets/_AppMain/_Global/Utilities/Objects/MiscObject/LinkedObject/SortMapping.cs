using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SortMapping : MonoBehaviour
{
    public SpriteDisplay BaseImage;
    protected int baseSortOrder;
    protected string baseSortLayer;

    private List<MappedImage> _mappedImages = null;
    public List<MappedImage> mappedImages
    {
        get
        {
            if (_mappedImages == null)
            {
                _mappedImages ??= MapImages();
            }
            
            return _mappedImages;
        }
    }
    private void Awake()
    {
        baseSortLayer = BaseImage.SortLayerName;
        baseSortOrder = BaseImage.SortOrder;
    }

    private List<MappedImage> MapImages()
    {
        List<MappedImage> list = new List<MappedImage>();
        SpriteRenderer[] rend = GetComponentsInChildren<SpriteRenderer>(true);

        //int min = 0;
        //int max = 0;
        for (int i = 0; i < rend.Length; i++)
        {
            if (BaseImage != null && rend[i] != BaseImage.sp)
            {
                MappedImage map = new MappedImage(BaseImage.SortOrder, rend[i], rend[i].sortingOrder, rend[i].sortingLayerName);
                list.Add(map);

                //if (rend[i].sortingOrder < min) { min = rend[i].sortingOrder; }
                //if (rend[i].sortingOrder > max) { max = rend[i].sortingOrder; }
            }

        }



        //Debug.Log($"Min = {min} & Max = {max}");
        return list;
    }

   
    public void UpdateBaseOrder(int newSortOrder)
    {
        int currentVal = BaseImage.SortOrder;

        int diff = newSortOrder - currentVal;

        BaseImage.SetSortOrder(newSortOrder);
        for (int i = 0; i < mappedImages.Count; i++)
        {
            mappedImages[i].UpdateSortOrder(newSortOrder);
        }
    }

    public void UpdateBaseLayer(string layer)
    {
        BaseImage.SetSortLayer(layer);
        for (int i = 0; i < mappedImages.Count; i++)
        {
            mappedImages[i].UpdateSortLayer(layer);

        }
    }
}


