using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
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
    #region General Variables
    public PlayableDirector playDir;
    public GameObject successPanel;
    public GameObject dialogBox;
    public GameObject micBtn;
    public Text rotationText;
    public float rotationDegree = 0;
    public int taskNo;

    [Header("Touch Mode")]
    public bool scaleUp;
    public bool scaleDown;
    public bool rotate;

    [Header("Scale Limit")]
    [SerializeField]
    private float minScale = 0.425f;
    [SerializeField]
    private float maxScale = 2.5f;

    [Header("Target Reference")]
    public Transform clips;
    public Transform magnetPoint;
    public Image targetOutline;
    public Transform target;

    private bool endGame = false;
    private float _width;
    private float _height;
    private float _xPos;
    private float _yPos;
    private Scene sc;
    #endregion

    private void Reset()
    {
        //touchBoundary = GameObject.Find("BoundaryFrame").GetComponent<Transform>();
    }

    private void Awake()
    {
        sc = SceneManager.GetActiveScene();
        micBtn = GameObject.Find("Mic-btn");
        successPanel = GameObject.Find("SuccessPanel");
        dialogBox = GameObject.Find("DialogBox");
    }

    private void Start()
    {
        micBtn.SetActive(false);
        successPanel.SetActive(false);
        dialogBox.SetActive(false);
        //restartButton.interactable = false;
        endGame = true;

        if (scaleUp && !GameControl.instance.zoomInTouchTask[taskNo])
            ScaleObject(true);
        else if (scaleDown && !GameControl.instance.zoomOutTouchTask[taskNo])
            ScaleObject(false);
        else if (rotate && !GameControl.instance.rotateTouchTask[taskNo])
            RotateObject();
        else
        {
            successPanel.SetActive(true);
            micBtn.SetActive(true);
        }

        StartCor();
    }

    private void Update()
    {
        if (rotate)
        {
            if (rotationDegree <= 305f && rotationDegree >= 295f)
            {
                clips.transform.position = Vector3.MoveTowards(clips.position, magnetPoint.position, 400 * Time.deltaTime);
                if (clips.position == magnetPoint.position && endGame)
                {
                    StartCoroutine(ActiveSuccessPanel());
                    endGame = false;
                    GameControl.instance.rotateTouchTask[taskNo] = true;
                }
                targetOutline.color = new Color(0, 1, 0, 0.5f);
            }
            else
                targetOutline.color = new Color(1, 0, 0, 0.5f);

            UpdateDegreeOfRotationText();
        }
    }

    public void StartCor()
    {
        bool IntroPlayed = AudioManager.instance.IntroPlayed[sc.buildIndex];
        if (!IntroPlayed)
            StartCoroutine(PopUpDialogBox());
    }

    public void StopCor()
    {
        StopCoroutine(PopUpDialogBox());
    }

    public void StopDialogBoxSound()
    {
        AudioManager.instance.Stop("panduan");
    }

    public IEnumerator PopUpDialogBox()
    {
        yield return new WaitForSeconds(4f);
        dialogBox.SetActive(true);
        AudioManager.instance.Play("panduan");

        yield return new WaitForSeconds(AudioManager.instance.GetAudioClipLength("panduan"));
        dialogBox.SetActive(false);
    }

    private IEnumerator ActiveSuccessPanel()
    {
        if (scaleDown)
        {
            yield return new WaitForSeconds(1f);
            playDir.Play();
            yield return new WaitForSeconds(2f);
        }

        yield return new WaitForSeconds(1f);
        AudioManager.instance.Play("yeay");
        successPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        micBtn.SetActive(true);
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
        float scaleFactor = 0.25f;
        var recognizer = new TKPinchRecognizer
        {
            //boundaryFrame = new TKRect(xOffset, yOffset, _width * scaleFactor, _height * scaleFactor)
        };

        recognizer.gestureRecognizedEvent += (r) =>
        {
            if (up)
            {
                if (recognizer.deltaScale > 0)
                    target.transform.localScale += Vector3.one * scaleFactor * Mathf.Abs(recognizer.deltaScale);
                if (target.transform.localScale.x >= maxScale)
                {
                    target.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
                    targetOutline.color = new Color(0, 1, 0, 1f);
                    StartCoroutine(ActiveSuccessPanel());
                    GameControl.instance.zoomInTouchTask[taskNo] = true;
                }
            }
            else
            {
                if (recognizer.deltaScale < 0)
                    target.transform.localScale += Vector3.one * scaleFactor * -Mathf.Abs(recognizer.deltaScale);
                if (target.transform.localScale.x < minScale)
                {
                    target.transform.localScale = new Vector3(minScale, minScale, minScale);
                    targetOutline.color = new Color(0, 1, 0, 1f);
                    StartCoroutine(ActiveSuccessPanel());
                    GameControl.instance.zoomOutTouchTask[taskNo] = true;
                }
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