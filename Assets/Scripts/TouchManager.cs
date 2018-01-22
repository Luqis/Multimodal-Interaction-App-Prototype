using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Boundary
{
    [SerializeField] private float _width;
    [SerializeField] private float _height;
    [SerializeField] private float _xPos;
    [SerializeField] private float _yPos;

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
    public float rotationDegree = 0;
    public Text rotationText;

    [Header("Touch Mode")]
    public bool scaleUp;
    public bool scaleDown;
    public bool rotate;

    [Header("Scale Limit")]
    [SerializeField]
    private float minScale = 0.25f;
    [SerializeField]
    private float maxScale = 3.5f;

    [Header("Magnet Reference")]
    public Transform clips;
    public Transform target;
    public Transform magnetPoint;
    public Image magnetOutline;
    public Transform touchBoundary;
    public Boundary boundary;

    private float _width;
    private float _height;
    private float _xPos;
    private float _yPos;

    private void Reset()
    {
        touchBoundary = GameObject.Find("BoundaryFrame").GetComponent<Transform>();
    }

    private void Awake()
    {
        _width = touchBoundary.GetComponent<RectTransform>().rect.width;
        _height = touchBoundary.GetComponent<RectTransform>().rect.height;

        _xPos = touchBoundary.transform.position.x;
        _yPos = touchBoundary.transform.position.y;

        boundary = new Boundary(_width, _height, _xPos, _yPos);
    }

    private void Start()
    {
        if (scaleUp)
            ScaleObject(true);
        if (scaleDown)
            ScaleObject(false);
        if (rotate)
            RotateObject();
    }

    private void Update()
    {
        if (rotationDegree <= 305f && rotationDegree >= 295f)
        {
            clips.transform.position = Vector3.MoveTowards(clips.position, magnetPoint.position, 400 * Time.deltaTime);
            magnetOutline.color = new Color(0, 1, 0, 0.5f);
        }
        else
            magnetOutline.color = new Color(1, 0, 0, 0.5f);

        UpdateDegreeOfRotationText();
    }

    private void UpdateDegreeOfRotationText()
    {
        if (rotationDegree >= 0 && rotationDegree < 15)
        {
            rotationText.text = "0˚";
        }
        else
            rotationText.text = (360f - rotationDegree).ToString("0.#") + "˚";
    }

    public void ScaleObject(bool up)
    {
        float scaleFactor = 1.5f;
        float xOffset = _xPos - (_width * scaleFactor - _width) / 2f;
        float yOffset = _yPos - (_height * scaleFactor - _width) / 2f;

        var recognizer = new TKPinchRecognizer
        {
            //boundaryFrame = new TKRect(xOffset, yOffset, _width * scaleFactor, _height * scaleFactor)
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
        var recognizer = new TKRotationRecognizer();

        recognizer.gestureRecognizedEvent += (r) =>
        {
            float _degreeOfRotation = target.transform.eulerAngles.z;
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
            rotationDegree = _degreeOfRotation;
        };
        TouchKit.addGestureRecognizer(recognizer);
    }
}