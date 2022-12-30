using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiImage : MonoBehaviour
{
    [System.Serializable]
    public class MappedImage
    {
        public string key;
        public SpriteDisplay image;
    }

    #region Properties
    private Dictionary<int, string> _mapping = null;
    public Dictionary<int, string> Mapping
    {
        get
        {
            _mapping ??= new Dictionary<int, string>();
            return _mapping;
        }
    }

    public List<MappedImage> images = new List<MappedImage>();
    #endregion

    #region Indexing
    protected SpriteDisplay this[string key]
    {
        get
        {
            return images[MappedKey(key)].image;
        }
    }
    public SpriteDisplay FromKey(string key)
    {
        return this[key];
    }
    public SpriteDisplay AtIndex(int index)
    {
        string key = Mapping[index];
        return this[key];
    }
    #endregion

    #region Life Cycle
    private void Awake()
    {
        if (images.Count > 0)
        {
            for (int i = 0; i < images.Count; i++)
            {
                if (images[i].key.IsEmpty())
                {
                    images[i].key = i.ToString();
                }
            }
        }

        Mapping.Clear();
        for (int i = 0; i < images.Count; i++)
        {
            Mapping.Add(i, images[i].key);
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    #endregion

    #region Mapping
    protected int MappedKey(string objectKey)
    {
        for (int i = 0; i < images.Count; i++)
        {
            if (images[i].key == objectKey)
            {
                return i;
            }
        }
        return -1;
    }

    public void AddMapping(int index, string key)
    {
        if (!Mapping.ContainsKey(index))
        {
            Mapping.Add(index, key);
        }

        ApplyMapping();
    }
    public void AddMapping(Dictionary<int, string> mapping)
    {
        foreach (var item in mapping)
        {
            if (!Mapping.ContainsKey(item.Key))
            {
                Mapping.Add(item.Key, item.Value);
            }
        }
        ApplyMapping();
    }


    protected void ApplyMapping()
    {
        for (int i = 0; i < images.Count; i++)
        {
            if (Mapping.ContainsKey(i))
            {
                images[i].key = Mapping[i];
            }
        }
    }
    #endregion

    #region Image Management
    public void AddImage(MappedImage map, bool overwriteMapping = false)
    {
        int count = Mapping.Count;
        if (overwriteMapping)
        {
            if (Mapping.ContainsKey(count))
            {
                map.key = Mapping[count];
            }
        }

        images.Add(map);
    }
    public void SetSprite(string objectKey, Sprite sp)
    {
        int key = MappedKey(objectKey);
        if (key > -1)
        {
            MappedImage map = images[key];
            map.image.SetSprite(sp);

        }
       
    }
    public void SetSprite(int indexKey, Sprite sp)
    {
       if (indexKey < images.Count)
        {
            MappedImage map = images[indexKey];
            map.image.SetSprite(sp);
        }

    }

    public void SetColor(string key, Color color)
    {
        this[key].SetColor(color);
    }
    public void ShowSprite(string key)
    {
        this[key].gameObject.SetActive(true);
    }
    public void HideSprite(string key)
    {
        this[key].gameObject.SetActive(false);
    }
   
    public void ClearAll()
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].image.Clear();
        }
    }
    #endregion

  
}


