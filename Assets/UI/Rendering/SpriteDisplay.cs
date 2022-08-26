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

    private SpriteRenderer _sp = null;
    private Image _image = null;

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

    public void SetColor(Color color)
    {
        if (RendType == RenderType.Image) { _image.color = color; }
        if (RendType == RenderType.Sprite) { _sp.color = color; }
    }
}
