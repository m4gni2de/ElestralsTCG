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

    public Vector2?[] oldTouchPositions = {
        null,
        null
    };
    Vector2 oldTouchVector;
    float oldTouchDistance;

    public bool isFree = true;
    private bool isShaking = false;

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
       
        
    }

    void GetScales()
    {
        float screenRatio = ScreenHeight / ScreenWidth;
        float targetRatio = Height / Width;

        
#if UNITY_EDITOR
        EditorScales();
        //defaultSize = Width * ParentCanvas.transform.localScale.x;
#else
        defaultSize = ScreenWidth;
#endif

        minSizePercent = .2f;

        minCameraSize = defaultSize * (1 - minSizePercent);
        maxCameraSize = defaultSize * (1 + minSizePercent);

        mainCamera.orthographicSize = defaultSize;
        //#if UNITY_EDITOR

        //        EditorScales();
        //#else
        //        if (screenRatio >= targetRatio)
        //        {
        //            mainCamera.orthographicSize = (Width / 2) * ParentCanvas.transform.localScale.y;
        //            defaultSize = (Width / targetRatio) * ParentCanvas.transform.localScale.y;
        //        }
        //        else
        //        {
        //            float differenceInSize = targetRatio / screenRatio;
        //            mainCamera.orthographicSize = Width / 2 * differenceInSize;
        //        }


        //        minSizePercent = .95f;

        //        minCameraSize = mainCamera.orthographicSize * (1 - minSizePercent);
        //        maxCameraSize = mainCamera.orthographicSize * (1 + minSizePercent);
        //        defaultSize = minCameraSize;
        //        mainCamera.orthographicSize = minCameraSize;
        //#endif

        //if (screenRatio >= targetRatio)
        //{
        //    mainCamera.orthographicSize = (Width / 2) * ParentCanvas.transform.localScale.y;
        //    defaultSize = (Width / targetRatio) * ParentCanvas.transform.localScale.y;
        //}
        //else
        //{
        //    float differenceInSize = targetRatio / screenRatio;
        //    mainCamera.orthographicSize = Width / 2 * differenceInSize;
        //}

        //defaultSize = ScreenWidth;
        //minSizePercent = .2f;

        //minCameraSize = mainCamera.orthographicSize * (1 - minSizePercent);
        //maxCameraSize = mainCamera.orthographicSize * (1 + minSizePercent);

        //mainCamera.orthographicSize = defaultSize;




    }

    void EditorScales()
    {
       
        defaultSize = ParentCanvas.renderingDisplaySize.x;

        maxCameraSize = defaultSize * (1 + minSizePercent);
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsFrozen && isFree)
        {


#if UNITY_EDITOR
            //MouseCamera();
            MouseWheel();

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

                if (worldMousePosition != clickPosition)
                {
                    facingDirection = clickPosition - worldMousePosition;

                    xDiff = clickPosition.x - worldMousePosition.x;
                    yDiff = clickPosition.y - worldMousePosition.y;

                    if (facingDirection.x != 0) { mainCamera.transform.Translate(new Vector3(xDiff / cameraMoveSpeed + 0.01f, 0), Space.Self); }
                    if (facingDirection.y != 0) { mainCamera.transform.Translate(new Vector3(0, yDiff) / (cameraMoveSpeed + 0.01f), Space.Self); }

                }
            }

        }
        else
        {

            if (isClicked == true)
            {
                acumTime += 3;
                mainCamera.transform.Translate(facingDirection / ((cameraMoveSpeed + 0.01f) + acumTime), Space.Self);

                if (acumTime >= 21 || Input.GetMouseButton(0))
                {
                    mainCamera.transform.position = mainCamera.transform.position;
                    isClicked = false;
                    acumTime = 0;
                }
            }
            //isClicked = false;
        }



        

        //if (mainCamera.orthographicSize >= maxCameraSize)
        //{
        //    mainCamera.orthographicSize = maxCameraSize;
        //}


    }

    protected void MouseWheel()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            mainCamera.orthographicSize += (1 * (cameraMoveSpeed + 0.01f));
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            mainCamera.orthographicSize -= (1 * (cameraMoveSpeed + 0.01f));
        }
    }

    public void ToggleFree(bool free, bool reset = false)
    {
        isFree = free;
        if (reset)
        {
            ResetCamera();
        }
    }

    public void TouchCamera()
    {
        if (Input.touchCount == 0 || IsPointerOverUIObject())
        {
            oldTouchPositions[0] = null;
            oldTouchPositions[1] = null;
            ToggleFree(true);

        }
        else if (Input.touchCount == 1 && !IsPointerOverUIObject())
        {



            if (oldTouchPositions[0] == null || oldTouchPositions[1] != null)
            {
                oldTouchPositions[0] = Input.GetTouch(0).position;
                oldTouchPositions[1] = null;
            }
            else
            {
                Vector2 newTouchPosition = Input.GetTouch(0).position;

                mainCamera.transform.position += mainCamera.transform.TransformDirection((Vector3)((oldTouchPositions[0] - newTouchPosition) * mainCamera.orthographicSize / mainCamera.pixelHeight * 2f));

                oldTouchPositions[0] = newTouchPosition;
            }
        }
        else
        {
            if (!!IsPointerOverUIObject())
            {
                if (oldTouchPositions[1] == null)
                {
                    oldTouchPositions[0] = Input.GetTouch(0).position;
                    oldTouchPositions[1] = Input.GetTouch(1).position;
                    oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
                    oldTouchDistance = oldTouchVector.magnitude;
                }
                else
                {
                    Vector2 screen = new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight);

                    Vector2[] newTouchPositions = {
                    Input.GetTouch(0).position,
                    Input.GetTouch(1).position
                };
                    Vector2 newTouchVector = newTouchPositions[0] - newTouchPositions[1];
                    float newTouchDistance = newTouchVector.magnitude;

                    mainCamera.transform.position += mainCamera.transform.TransformDirection((Vector3)((oldTouchPositions[0] + oldTouchPositions[1] - screen) * mainCamera.orthographicSize / screen.y));
                    //transform.localRotation *= Quaternion.Euler(new Vector3(0, 0, Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 0.0174532924f));
                    mainCamera.orthographicSize *= oldTouchDistance / newTouchDistance;
                    mainCamera.transform.position -= mainCamera.transform.TransformDirection((newTouchPositions[0] + newTouchPositions[1] - screen) * mainCamera.orthographicSize / screen.y);

                    oldTouchPositions[0] = newTouchPositions[0];
                    oldTouchPositions[1] = newTouchPositions[1];
                    oldTouchVector = newTouchVector;
                    oldTouchDistance = newTouchDistance;
                }
            }

        }





        //for (var i = 0; i < Input.touchCount; ++i)
        //{

        //    Input.GetTouch(0).
        //}
    }

    public void SetToXMin(bool isLock)
    {
        mainCamera.transform.localPosition = new Vector2(minPos.x, 0f);
        if (isLock)
        {
            ToggleFree(false);
        }
    }

    public void SetToXMax(bool isLock)
    {
        //ToggleFree(!isLock);
        mainCamera.transform.localPosition = new Vector2(maxPos.x, 0f);

    }

    private void ResetCamera()
    {
        mainCamera.orthographicSize = defaultSize;
        mainCamera.transform.localPosition = new Vector2(0f, 0f);
    }
    private void CameraShake()
    {

        Vector3 orgPos = mainCamera.transform.position;
        float shakeAmt = 2f;
        StartCoroutine(Shake(orgPos, shakeAmt));
    }

    private IEnumerator Shake(Vector3 org, float shakeAmt)
    {
        if (shakeAmt > 0)
        {
            float acumTime = 0f;
            do
            {
                isShaking = true;
                float quakeAmt = UnityEngine.Random.value * shakeAmt * 2 - shakeAmt;
                Vector3 pp = mainCamera.transform.position;
                pp.y += quakeAmt; // can also add to x and/or z
                pp.x -= quakeAmt;
                mainCamera.transform.position = pp;

                acumTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            } while (acumTime < 1f);

            mainCamera.transform.position = org;
            isShaking = false;
        }
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
