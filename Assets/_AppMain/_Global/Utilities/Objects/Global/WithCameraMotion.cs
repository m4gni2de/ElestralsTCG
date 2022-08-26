using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithCameraMotion : MonoBehaviour
{
    private float maxSize, minSize, defaultSize;
    private Vector2 minPos, maxPos;
    private float sizeDiff;
    public float minSizePercent = .2f;
    public float minVal = -500f;
    public float maxVal = 500f;
    [Range(0.8f, 1f)]
    public float MaxPercentOfScreen = .95f;

    public bool isClicked;

    public float moveSpeed;
    private Vector2 facingDirection;
    private float xDiff, yDiff, acumTime;

    public Vector3 clickPosition;

    private float ScreenHeight { get { return Screen.safeArea.height; } }
    private float ScreenWidth { get { return Screen.safeArea.width; } }

    private float Width { get { return GetComponent<RectTransform>().rect.width; } }
    private float Height { get { return GetComponent<RectTransform>().rect.height; } }

    private void Start()
    {
        defaultSize = 1f;
        maxSize = 4f;
        float screenRatio = ScreenHeight / ScreenWidth;
        float targetRatio = Height / Width;
        minSize = defaultSize * (1 - minSizePercent);
    }

    private void Update()
    {
        MouseCamera();

        if (transform.localScale.x >= maxSize)
        {
            transform.localScale = new Vector3(maxSize, maxSize);
        }

        if (transform.localScale.x <= minSize)
        {
            transform.localScale = new Vector3(minSize, minSize);
        }

        sizeDiff = 1 - (transform.localScale.x / maxSize);

        minPos = new Vector2(-Width * sizeDiff / 2, -Height * sizeDiff / 2);
        maxPos = new Vector2(Width * sizeDiff / 2, Height * sizeDiff / 2);
    }

    public void LateUpdate()
    {
        if (transform.localPosition.x > maxVal)
        {
            transform.localPosition = new Vector2(maxVal, transform.localPosition.y);
        }
        if (transform.localPosition.x < minVal)
        {
            transform.localPosition = new Vector2(minVal, transform.localPosition.y);
        }

        if (transform.localPosition.y > maxVal)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, maxVal);
        }
        if (transform.localPosition.y < minVal)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, minVal);
        }

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -2f);

        //GameManager.Instance.CameraMove();
    }


    public void MouseCamera()
    {

        var worldMousePosition =
                    Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        var x = worldMousePosition.x;
        var y = worldMousePosition.y;
        //if (Input.GetMouseButton(0) && !IsPointerOverUIObject())
        if (Input.GetMouseButton(0))
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

                    if (facingDirection.x != 0) { transform.Translate(new Vector3(xDiff * moveSpeed, 0), Space.Self); }
                    if (facingDirection.y != 0) { transform.Translate(new Vector3(0, yDiff) * moveSpeed, Space.Self); }

                }
            }

        }
        else
        {

            if (isClicked == true)
            {
                acumTime += 3;
                transform.Translate(facingDirection / (acumTime / moveSpeed), Space.Self);

                if (acumTime >= 21 || Input.GetMouseButton(0))
                {
                    transform.position = transform.position;
                    isClicked = false;
                    acumTime = 0;
                }
            }
            //isClicked = false;
        }



        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            
            transform.localScale -= new Vector3(moveSpeed, moveSpeed);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.localScale += new Vector3(moveSpeed, moveSpeed);
        }

        //if (mainCamera.orthographicSize >= maxCameraSize)
        //{
        //    mainCamera.orthographicSize = maxCameraSize;
        //}


    }
}
