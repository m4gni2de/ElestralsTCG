using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using static UnityEditor.Experimental.GraphView.GraphView;
#endif

public class SpriteDisplay : MonoBehaviour
{
    #region Operators
    public static implicit operator SpriteDisplay(SpriteRenderer rend)
    {
        SpriteDisplay display;
        rend.TryGetComponent(out display);
        if (display != null) { return display; }
        return null;
    }

    #endregion
    public enum RenderType
    {
        Undefined = -1,
        Sprite = 0,
        Image = 1
    };

    #region Renderer/Image
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
    private RenderType _rendType = RenderType.Undefined;
    public RenderType RendType
    {
        get
        {
            return _rendType;
        }
        set
        {
            _rendType = value;
        }
    }

    [SerializeField]
    private SpriteRenderer _sp = null;
    public SpriteRenderer sp
    {
        get { return _sp; }
        set { _sp = value; }
    }
    [SerializeField]
    private Image _image = null;
    public Image image
    {
        get { return _image; }
        set { _image = value; }
    }

    #endregion
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

    private void Reset()
    {
        CheckDisplay();
    }
    private void CheckDisplay()
    {
        if (RendType == RenderType.Sprite)
        {
            if (_sp == null)
            {
                if (GetComponent<SpriteRenderer>() == null) { gameObject.AddComponent<SpriteRenderer>(); }

                _sp = GetComponent<SpriteRenderer>();
            }  
        }



        if (RendType == RenderType.Image)
        {
            if (_image == null)
            {
                if (GetComponent<Image>() == null) { gameObject.AddComponent<Image>(); }
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

    #region Colors
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
    public void ChangeToColor(Color col, float changeDuration)
    {
        StartCoroutine(FadeColor(col, changeDuration));
    }
    private IEnumerator FadeColor(Color col, float totalTime)
    {
        Color prevCol = GetColor();
        float acumTime = 0f;
        SetColor(col);


        do
        {
            yield return new WaitForEndOfFrame();
            acumTime += Time.deltaTime;
            float percElapsed = (acumTime - totalTime) / totalTime;
            Color fade = Color.Lerp(col, prevCol, percElapsed);
            SetColor(fade);
        } while (true && acumTime <= totalTime);

        SetColor(col);
    }
    public void ChangeToColorForDuration(Color col, float time, float fadeDuration = 0f)
    {
        StartCoroutine(DoChangeAndFadeBack(col, time, fadeDuration));
    }

    /// <summary>
    /// Change the color of the spite to a certain color for a certain period of time, then fades back to the original color.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="time"></param>
    /// <param name="fadeDuration"></param>
    /// <returns></returns>
    private IEnumerator DoChangeAndFadeBack(Color col, float time, float fadeDuration)
    {
        Color prevCol = GetColor();
        float acumTime = 0f;
        float totalTime = time + fadeDuration;
        SetColor(col);


        do
        {
            yield return new WaitForEndOfFrame();
            acumTime += Time.deltaTime;

            if (acumTime >= time)
            {
                float elapsedFade = acumTime - time;
                float percFade = elapsedFade / fadeDuration;

                Color fade = Color.Lerp(col, prevCol, percFade);
                SetColor(fade);
            }
        } while (true && acumTime <= totalTime);

        SetColor(prevCol);
    }
    #endregion

    #region Functions

    public event Action<string> OnSortLayerSet;
    public void SetSortLayer(string sortLayer)
    {
        //if (RendType == RenderType.Sprite) { _sp.sortingLayerName = sortLayer; }
        //if (RendType == RenderType.Image) { _image.canvas.sortingLayerName = sortLayer; }
        SortLayer = sortLayer;
        OnSortLayerSet?.Invoke(sortLayer);


    }

    public event Action<int> OnSortOrderSet;
    public void SetSortOrder(int order)
    {
        //if (RendType == RenderType.Sprite) { _sp.sortingOrder = order; }
        //if (RendType == RenderType.Image) { _image.canvas.sortingOrder = order; }
        SortOrder = order;
        OnSortOrderSet?.Invoke(order);
    }

    public event Action<int> OnSortOrderChanged;
    public void ChangeSortOrder(int changeVal)
    {
        SortOrder += changeVal;
        OnSortOrderChanged?.Invoke(changeVal);
        //if (RendType == RenderType.Sprite)
        //{
        //    _sp.sortingOrder += changeVal;
        //}
        //if (RendType == RenderType.Image)
        //{
        //    _image.canvas.sortingOrder += changeVal;
        //}
    }
    public string SortLayer
    {
        get
        {
            string layer = "";
            if (RendType == RenderType.Sprite) { layer = _sp.sortingLayerName; }
            if (RendType == RenderType.Image) { layer = _image.canvas.sortingLayerName; }
            return layer;
        }
        set
        {
            if (RendType == RenderType.Sprite) { _sp.sortingLayerName = value; }
            if (RendType == RenderType.Image) { _image.canvas.sortingLayerName = value; }
        }
    }
    public int SortOrder
    {
        get
        {
            int order = 0;
            if (RendType == RenderType.Sprite) { order = _sp.sortingOrder; }
            if (RendType == RenderType.Image) { order = _image.canvas.sortingOrder; }
            return order;
        }
        set
        {
            if (RendType == RenderType.Sprite) { _sp.sortingOrder = value; }
            if (RendType == RenderType.Image) { _image.canvas.sortingOrder = value; }
        }
    }
    #endregion

    #region Bounds and Screen Positions
    private List<Vector2> _imageOutline = null;
    public List<Vector2> ImageOutline
    {
        get
        {
            if (_imageOutline == null)
            {
                _imageOutline = MainSprite.SetLocalOutline(m_Transform);
            }
            return _imageOutline;
        }
    }

    public bool IsPointerOverMe()
    {
        if (RendType == RenderType.Image)
        {
            RectTransform rect = (RectTransform)m_Transform;
            return rect.IsPointerOverMe();
        }
        else if (RendType == RenderType.Sprite)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            return mousePos.IsPointInside(ImageOutline);
        }
        return false;
        
    }
    #endregion
}
