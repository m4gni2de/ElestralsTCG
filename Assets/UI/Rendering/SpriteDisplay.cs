using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteDisplay : MonoBehaviour
{
    public enum RenderType
    {
        Sprite = 0,
        Image = 1
    };

    
    private Sprite _mainSprite = null;
    public Sprite MainSprite
    {
        get
        {
            if (_mainSprite == null)
            {
                if (RendType == RenderType.Sprite) { _mainSprite = _sp.sprite; }
                if (RendType == RenderType.Image) { _mainSprite = _image.sprite; }
            }
            return _mainSprite;
            
        }
        set
        {
            CheckDisplay();
            if (RendType == RenderType.Sprite) { _sp.sprite = value; }
            if (RendType == RenderType.Image) { _image.sprite = value; }
            _mainSprite = value;
        }
    }

    [SerializeField]
    private RenderType _rendType = RenderType.Sprite;
    public RenderType RendType { get { return _rendType; } }

    [SerializeField]
    private SpriteRenderer _sp = null;
    private Image _image = null;

    public Transform m_Transform
    {
        get
        {
            Transform t = gameObject.transform;
            if (RendType == RenderType.Sprite) { t = _sp.gameObject.transform; }
            if (RendType == RenderType.Image) { t = _image.gameObject.transform; }
            return t;
        }
    }
    private void Awake()
    {
        CheckDisplay();
    }

    private void CheckDisplay()
    {
        if (RendType == RenderType.Sprite)
        {
            if (_sp == null)
            {
                if (GetComponent<SpriteRenderer>() == null)
                {

                    gameObject.AddComponent<SpriteRenderer>();

                }
                _sp = GetComponent<SpriteRenderer>();
            }  
        }



        if (RendType == RenderType.Image)
        {
            if (_image == null)
            {
                if (GetComponent<Image>() == null)
                {
                    gameObject.AddComponent<Image>();

                }
                _image = GetComponent<Image>();
            }
        }
    }

    public void SetSprite(Sprite sp)
    {
        MainSprite = sp;
    }

    public void Clear()
    {
        MainSprite = null;
        //if (RendType == RenderType.Sprite) { _sp.sprite = null; }
        //if (RendType == RenderType.Image) { _image.sprite = null; }
    }

    public void SetColor(Color color)
    {
        if (RendType == RenderType.Image) { _image.color = color; }
        if (RendType == RenderType.Sprite) { _sp.color = color; }
    }
    public Color GetColor()
    {
        if (RendType == RenderType.Image) { return _image.color; }
        if (RendType == RenderType.Sprite) { return _sp.color; }
        return Color.white;
    }

    #region Functions
    public void SetSortLayer(string sortLayer)
    {
        if (RendType == RenderType.Sprite) { _sp.sortingLayerName = sortLayer; }
        if (RendType == RenderType.Image) { _image.canvas.sortingLayerName = sortLayer; }
    }
    public void SetSortOrder(int order)
    {
        if (RendType == RenderType.Sprite) { _sp.sortingOrder = order; }
        if (RendType == RenderType.Image) { _image.canvas.sortingOrder = order; }
    }
    public void ChangeSortOrder(int changeVal)
    {
        if (RendType == RenderType.Sprite)
        {
            _sp.sortingOrder += changeVal;
        }
        if (RendType == RenderType.Image)
        {
            _image.canvas.sortingOrder += changeVal;
        }
    }
    public string SortLayerName
    {
        get
        {
            string layer = "";
            if (RendType == RenderType.Sprite) { layer = _sp.sortingLayerName; }
            if (RendType == RenderType.Image) { layer = _image.canvas.sortingLayerName; }
            return layer;
        }
    }
    #endregion
}
