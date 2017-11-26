using UnityEngine;

[System.Serializable]
public struct Boundary
{
    [SerializeField] float _width;
    [SerializeField] float _height;
    [SerializeField] float _xPos;
    [SerializeField] float _yPos;

    public Boundary(float w, float h, float x, float y)
    {
        _width = w;
        _height = h;
        _xPos = x;
        _yPos = y;
    }
}

public class TouchManager : MonoBehaviour
{
    [Header("Touch Mode")]
    public bool scaleUp;
    public bool rotate;

    [Header("Scale Limit")]
    [SerializeField]
    private float minScale = 0.25f;
    [SerializeField]
    private float maxScale = 3.5f;

    [Header("GameObject")]
    public Transform target;
    public Transform touchBoundary;
    public Boundary boundary;

    private float _degreeOfRotation;
    private float _width;
    private float _height;
    private float _xPos;
    private float _yPos;

    void Reset()
    {
        touchBoundary = GameObject.Find("BoundaryFrame").GetComponent<Transform>();
    }

    void Awake()
    {
        _width = target.GetComponent<RectTransform>().rect.width;
        _height = target.GetComponent<RectTransform>().rect.height;

        _xPos = touchBoundary.transform.position.x;
        _yPos = touchBoundary.transform.position.y;

        boundary = new Boundary(_width, _height, _xPos, _yPos);

        _degreeOfRotation = target.transform.eulerAngles.z;
    }

    void Start()
    {
        if (scaleUp)
            ScaleObject(true);
        else
            ScaleObject(false);

        if (rotate)
            RotateObject();
    }

    public void ScaleObject(bool up)
    {
        float scaleFactor = 1.5f;
        float xOffset = _xPos - (_width * scaleFactor - _width) / 2f;
        float yOffset = _yPos - (_height * scaleFactor - _width) / 2f;

        var recognizer = new TKPinchRecognizer
        {
            boundaryFrame = new TKRect(xOffset, yOffset, _width * scaleFactor, _height * scaleFactor)
        };

        recognizer.gestureRecognizedEvent += (r) =>
        {
            if (up)
            {
                if (recognizer.deltaScale > 0)
                    target.transform.localScale += Vector3.one * Mathf.Abs(recognizer.deltaScale);
                if (target.transform.localScale.z >= maxScale)
                    target.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
            }
            else
            {
                if (recognizer.deltaScale < 0)
                    target.transform.localScale += Vector3.one * -Mathf.Abs(recognizer.deltaScale);
                if (target.transform.localScale.z < minScale)
                    target.transform.localScale = new Vector3(minScale, minScale, minScale);
            }
            Debug.Log("pinch recognizer fired: " + r);
        };
        TouchKit.addGestureRecognizer(recognizer);
    }

    public void RotateObject()
    {
        var recognizer = new TKRotationRecognizer
        {
            boundaryFrame = new TKRect(_xPos, _yPos, _width, _height)
        };
        recognizer.gestureRecognizedEvent += (r) =>
        {
            // Limit the object rotation
            if (_degreeOfRotation >= 270f || _degreeOfRotation == 0)
            {
                target.Rotate(Vector3.back, recognizer.deltaRotation * 0.9f);
            }
            if (_degreeOfRotation > 0 && _degreeOfRotation < 90f)
            {
                Vector3 desAng = new Vector3(0, 0, 0);
                Vector3 smooth = Vector3.Lerp(desAng, target.transform.eulerAngles, Time.deltaTime);
                target.transform.eulerAngles = smooth;
            }
            if (_degreeOfRotation < 270f && _degreeOfRotation > 180f)
            {
                Vector3 desAng = new Vector3(0, 0, 270f);
                Vector3 smooth = Vector3.Lerp(desAng, target.transform.eulerAngles, Time.deltaTime);
                target.transform.eulerAngles = smooth;
            }

            //Debug.Log (cube.transform.eulerAngles.z);
            Debug.Log("rotation recognizer fired: " + r);
        };
        TouchKit.addGestureRecognizer(recognizer);
    }
}
