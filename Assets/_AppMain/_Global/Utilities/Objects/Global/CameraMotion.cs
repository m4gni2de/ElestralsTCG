using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMotion : MonoBehaviour
{
    public Canvas ParentCanvas;
    private float maxCameraSize, minCameraSize, defaultSize;
    private Vector2 minPos, maxPos;
    private float sizeDiff;
    public float minSizePercent = .2f;
    [Range(0.8f, 1f)]
    public float MaxPercentOfScreen = .95f;


    public static CameraMotion main
    {
        get
        {
            return Camera.main.GetComponent<CameraMotion>();
        }
    }


    private Camera _camera;
    private Camera mainCamera
    {
        get
        {
            _camera ??= GetComponent<Camera>();
            return _camera;
        }
    }

    public bool isClicked;

    public float cameraMoveSpeed;
    private Vector2 facingDirection;
    private float xDiff, yDiff, acumTime;

    public Vector3 clickPosition;

    private float ScreenHeight { get { return Screen.safeArea.height; } }
    private float ScreenWidth { get { return Screen.safeArea.width; } }

    private float Width { get { return ParentCanvas.GetComponent<RectTransform>().rect.width; } }
    private float Height { get { return ParentCanvas.GetComponent<RectTransform>().rect.height; } }


    public bool IsFrozen = false;
    public void Freeze(bool freeze)
    {
        IsFrozen = freeze;
    }
    // Start is called before the first frame update
    void Start()
    {
        GetScales();
        mainCamera.orthographicSize = defaultSize;
    }

    void GetScales()
    {
        float screenRatio = ScreenHeight / ScreenWidth;
        float targetRatio = Height / Width;
#if UNITY_EDITOR

        EditorScales();
#else
        if (screenRatio >= targetRatio)
        {
            defaultSize = (Width / targetRatio) * ParentCanvas.transform.localScale.y;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            defaultSize = Width / targetRatio * differenceInSize;
        }
        maxCameraSize = Width * MaxPercentOfScreen;

#endif



        minSizePercent = .95f;

        minCameraSize = defaultSize * (1 - minSizePercent);




    }

    void EditorScales()
    {
       
        defaultSize = ParentCanvas.renderingDisplaySize.x;

        maxCameraSize = defaultSize * (1 + minSizePercent);
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsFrozen)
        {


#if UNITY_EDITOR
            MouseCamera();
#else
        //TouchCamera();
#endif
            if (mainCamera.orthographicSize >= maxCameraSize)
            {
                mainCamera.orthographicSize = maxCameraSize;
            }

            if (mainCamera.orthographicSize <= minCameraSize)
            {
                mainCamera.orthographicSize = minCameraSize;
            }

            sizeDiff = 1 - (mainCamera.orthographicSize / maxCameraSize);

            minPos = new Vector2(-Width * sizeDiff / 2, -Height * sizeDiff / 2);
            maxPos = new Vector2(Width * sizeDiff / 2, Height * sizeDiff / 2);

        }
    }

    public void LateUpdate()
    {
        if (!IsFrozen)
        {


            if (mainCamera.transform.localPosition.x > maxPos.x)
            {
                mainCamera.transform.localPosition = new Vector2(maxPos.x, mainCamera.transform.localPosition.y);
            }
            if (mainCamera.transform.localPosition.x < minPos.x)
            {
                mainCamera.transform.localPosition = new Vector2(minPos.x, mainCamera.transform.localPosition.y);
            }

            if (mainCamera.transform.localPosition.y > maxPos.y)
            {
                mainCamera.transform.localPosition = new Vector2(mainCamera.transform.localPosition.x, maxPos.y);
            }
            if (mainCamera.transform.localPosition.y < minPos.y)
            {
                mainCamera.transform.localPosition = new Vector2(mainCamera.transform.localPosition.x, minPos.y);
            }

            mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, mainCamera.transform.localPosition.y, -40f);
        }

        //GameManager.Instance.CameraMove();
    }

    public void MouseCamera()
    {
        
        var worldMousePosition =
                    Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        var x = worldMousePosition.x;
        var y = worldMousePosition.y;
        if (Input.GetMouseButton(0) && UIHelpers.IsPointerOnScreen() && !IsPointerOverUIObject())
        //if (Input.GetMouseButton(0))
        {
            
            if (isClicked == false)
            {
                isClicked = true;
                clickPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
            }
            if (isClicked)
            {
                //Debug.Log(worldMousePosition);

                if (worldMousePosition != clickPosition)
                {
                    facingDirection = clickPosition - worldMousePosition;

                    xDiff = clickPosition.x - worldMousePosition.x;
                    yDiff = clickPosition.y - worldMousePosition.y;

                    if (facingDirection.x != 0) { mainCamera.transform.Translate(new Vector3(xDiff / cameraMoveSpeed, 0), Space.Self); }
                    if (facingDirection.y != 0) { mainCamera.transform.Translate(new Vector3(0, yDiff) / cameraMoveSpeed, Space.Self); }

                }
            }

        }
        else
        {

            if (isClicked == true)
            {
                acumTime += 3;
                mainCamera.transform.Translate(facingDirection / (cameraMoveSpeed + acumTime), Space.Self);

                if (acumTime >= 21 || Input.GetMouseButton(0))
                {
                    mainCamera.transform.position = mainCamera.transform.position;
                    isClicked = false;
                    acumTime = 0;
                }
            }
            //isClicked = false;
        }



        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            mainCamera.orthographicSize += (1 * cameraMoveSpeed);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            mainCamera.orthographicSize -= (1 * cameraMoveSpeed);
        }

        //if (mainCamera.orthographicSize >= maxCameraSize)
        //{
        //    mainCamera.orthographicSize = maxCameraSize;
        //}


    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
