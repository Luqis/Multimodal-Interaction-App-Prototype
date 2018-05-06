using KKSpeech;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class SpeechListener : MonoBehaviour
{
    public PlayableDirector playDir;
    public Button startRecordingButton;
    public Text resultText;
    public Text rotationText;
    public GameObject successPanel;
    public GameObject radial;
    public int taskNo;
    private bool endGame = false;
    private bool CR_play = false;
    public float rotationDegree = 0;
    public float targetScale = 1;
    public Color32 finalColor = new Color32(255, 100, 50, 255);

    [Header("Speech Mode")]
    public bool scaleUp;
    public bool scaleDown;
    public bool rotate;
    private bool scaleUpStatus = false;
    private bool scaleDownStatus = false;
    private bool rotateStatus = false;

    [Header("Scale Limit")]
    [SerializeField]
    private float minScale = 0.425f;
    [SerializeField]
    private float maxScale = 2.5f;

    [Header("Rotate Range")]
    public float rotateLimit;
    public float upperLimit = 0;
    public float lowerLimit = 0;

    [Header("Magnet Reference")]
    public Transform clips;
    public Transform magnetPoint;
    public Image targetImg;
    public Transform target;
    public Image targetOutline;

    [Header("Attribute")]
    public float rotationLimit = 0;
    [SerializeField]
    private float[] rotationDegrees = { 340f, 320f }; //{ 350f, 340f, 330f, 320f, 310f };
    private float[] scaleUpArray = { 1.5f, 2.25f };//{ 1.25f, 1.5f, 1.75f, 2.25f };
    private float[] scaleDownArray = { 0.85f, 0.60f };//{ 0.90f, 0.85f, 0.75f, 0.60f };
    private List<LanguageOption> languageOptions;
    private string langId = "ms-MY";
    private string[] magnetKeywords = { "putar", "gerak", "pusing" };
    private string[] scaleUpKeywords = { "besar", "kembang" };
    private string[] scaleDownKeywords = { "kecil", "masuk" };

    private void Awake()
    {
        SetLanguage(langId);
        DisableBgMusic();
        StopCoroutine(ActiveSuccessPanel());
        rotationLimit = 360f - rotateLimit;
        upperLimit = rotationLimit + 2.5f;
        lowerLimit = rotationLimit - 2.5f;
        RotationArray();
    }

    private void RotationArray()
    {
        for (int i = 0; i < rotationDegrees.Length; i++)
        {
            rotationDegrees[i] = rotateLimit / 3 * (i + 1) + rotationLimit;
        }
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (scaleUp && !GameControl.instance.zoomInSpeechTask[taskNo])
            scaleUpStatus = true;
        else if (scaleDown && !GameControl.instance.zoomOutSpeechTask[taskNo])
            scaleDownStatus = true;
        else if (rotate && !GameControl.instance.rotateSpeechTask[taskNo])
            rotateStatus = true;
        else
        {
            successPanel.SetActive(true);
        }
#else
        radial.SetActive(false);
        successPanel.SetActive(false);
        //restartButton.interactable = false;
        endGame = true;

        if (SpeechRecognizer.ExistsOnDevice())
        {
            SpeechRecognizerListener listener = FindObjectOfType<SpeechRecognizerListener>();
            listener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
            listener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
            listener.onErrorDuringRecording.AddListener(OnError);
            listener.onErrorOnStartRecording.AddListener(OnError);
            listener.onFinalResults.AddListener(OnFinalResult);
            //listener.onPartialResults.AddListener(OnPartialResult);
            listener.onEndOfSpeech.AddListener(OnEndOfSpeech);
            listener.onSupportedLanguagesFetched.AddListener(OnSupportedLanguagesFetched);
            startRecordingButton.enabled = true;
            SpeechRecognizer.GetSupportedLanguages();
            SpeechRecognizer.RequestAccess();
        }
        else
        {
            resultText.text = "Maaf. Rosak!";
            startRecordingButton.interactable = false;
        }

        if (scaleUp && !GameControl.instance.zoomInSpeechTask[taskNo])
            scaleUpStatus = true;
        else if (scaleDown && !GameControl.instance.zoomOutSpeechTask[taskNo])
            scaleDownStatus = true;
        else if (rotate && !GameControl.instance.rotateSpeechTask[taskNo])
            rotateStatus = true;
        else
        {
            successPanel.SetActive(true);
            //micBtn.SetActive(true);
        }
#endif
    }

    private void Update()
    {
        rotationDegree = target.transform.eulerAngles.z;
        targetScale = target.transform.localScale.x;

        if (rotateStatus)
        {
            VerifyKeyword(magnetKeywords);

            if (rotationDegree <= upperLimit && rotationDegree >= lowerLimit)
            {
                RotateTask(taskNo);
                targetOutline.color = new Color(0, 1, 0, 0.5f);
            }
            else
                targetOutline.color = new Color(1, 0, 0, 0.5f);
        }
        else if (scaleUpStatus)
        {
            VerifyKeyword(scaleUpKeywords);
        }
        else if (scaleDownStatus)
        {
            VerifyKeyword(scaleDownKeywords);
        }

        if (radial.gameObject.activeSelf)
        {
            radial.transform.Rotate(Vector3.forward);
            resultText.color = Color.black;
        }
    }

    private void RotateTask(int num)
    {
        if (num == 0)
        {
            clips.transform.position = Vector3.MoveTowards(clips.position, magnetPoint.position, 400 * Time.deltaTime);
            if (clips.position == magnetPoint.position && endGame)
            {
                endGame = false;
                StartCoroutine(ActiveSuccessPanel());
            }
        }
        else if (num == 1)
        {
            targetImg.color = Color.Lerp(targetImg.color, finalColor, 5 * Time.deltaTime);
            if (targetImg.color == finalColor && endGame)
            {
                endGame = false;
                StartCoroutine(ActiveSuccessPanel());
            }
        }
        else if (num == 2)
        {
            targetImg.color = Color.Lerp(targetImg.color, finalColor, 5 * Time.deltaTime);
            if (targetImg.color == finalColor && endGame)
            {
                endGame = false;
                StartCoroutine(ActiveSuccessPanel());
            }
        }

        GameControl.instance.rotateSpeechTask[num] = true;
    }

    IEnumerator ActiveSuccessPanel()
    {
        CR_play = true;
        if (!rotate)
        {
            yield return new WaitForSeconds(1f);
            playDir.Play();
            yield return new WaitForSeconds((float)playDir.duration + 0.5f);
        }

        yield return new WaitForSeconds(1f);
        AudioManager.instance.Play("yeay");
        successPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
    }

    private void VerifyKeyword(string[] keywords)
    {
        if (rotate)
        {
            if (Array.Exists(keywords, word => word == resultText.text))
            {
                target.Rotate(0, 0, -1f);
                resultText.color = new Color(0, 1f, 0);

                if (Array.Exists(rotationDegrees, degree => degree == rotationDegree) || rotationDegree <= rotationLimit)
                {
                    resultText.text = "...";
                    SpeechRecognizer.StopIfRecording();
                }

                UpdateDegreeOfRotationText();
            }
            else
            {
                resultText.color = new Color(1f, 0, 0);
            }
        }

        if (scaleUp)
        {
            float scaleFactor = 0.01f;
            if (Array.Exists(scaleUpKeywords, word => word == resultText.text))
            {
                target.transform.localScale += Vector3.one * scaleFactor;

                if (Array.Exists(scaleUpArray, scale => scale.ToString() == rotationText.text))
                {
                    resultText.text = "...";
                    SpeechRecognizer.StopIfRecording();
                }
                else if (targetScale >= maxScale)
                {
                    target.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
                    targetOutline.color = new Color(0, 1, 0, 0.5f);
                    if (endGame && !CR_play)
                    {
                        GameControl.instance.zoomInSpeechTask[taskNo] = true;
                        endGame = false;
                        StartCoroutine(ActiveSuccessPanel());
                    }
                }
                rotationText.text = targetScale.ToString("0.##");
            }
        }

        if (scaleDown)
        {
            float scaleFactor = 0.01f;
            if (Array.Exists(keywords, word => word == resultText.text))
            {
                target.transform.localScale += -Vector3.one * scaleFactor;

                if (Array.Exists(scaleDownArray, scale => scale.ToString() == rotationText.text))
                {
                    resultText.text = "...";
                    SpeechRecognizer.StopIfRecording();
                }
                else if (targetScale <= minScale)
                {
                    target.transform.localScale = new Vector3(minScale, minScale, minScale);
                    targetOutline.color = new Color(0, 1, 0, 0.5f);
                    if (endGame && !CR_play)
                    {
                        GameControl.instance.zoomOutSpeechTask[taskNo] = true;
                        endGame = false;
                        StartCoroutine(ActiveSuccessPanel());
                    }
                }
                rotationText.text = targetScale.ToString("0.##");
            }
        }
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

    #region Speech Recognition Properties

    private void SetLanguage(string id)
    {
        SpeechRecognizer.SetDetectionLanguage(id);
    }

    public void OnSupportedLanguagesFetched(List<LanguageOption> languages)
    {
        languageOptions = languages;
    }

    public void OnFinalResult(string result)
    {
        resultText.text = result;
    }

    public void OnPartialResult(string result)
    {
        resultText.text = result;
    }

    public void OnAvailabilityChange(bool available)
    {
        startRecordingButton.enabled = available;
        if (!available)
        {
            resultText.text = "Ada sesuatu tidak kena";
        }
        else
        {
            resultText.text = "Cakap sesuatu";
        }
    }

    public void OnAuthorizationStatusFetched(AuthorizationStatus status)
    {
        switch (status)
        {
            case AuthorizationStatus.Authorized:
                startRecordingButton.enabled = true;
                break;

            default:
                startRecordingButton.enabled = false;
                resultText.text = "Cannot use Speech Recognition, authorization status is " + status;
                break;
        }
    }

    public void OnEndOfSpeech()
    {
        //startRecordingButton.interactable = true;
        //startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
    }

    public void OnError(string error)
    {
        Debug.LogError(error);
        resultText.text = "Cuba lagi!";
        resultText.color = new Color(0, 0, 1f);
        //resultText.text = "Try again! [" + error + "]";
    }

    #endregion Speech Recognition Properties

    public void StartSpeechRecording()
    {
        StopCoroutine(SpeechRecognitionProcess());
        startRecordingButton.interactable = false;
        SpeechRecognizer.StartRecording(true);
        resultText.text = "Cakap sesuatu";
        radial.SetActive(true);
        StartCoroutine(SpeechRecognitionProcess());
    }

    public void StartSpeechRecording(GameObject go)
    {
        go.SetActive(false);
        StopCoroutine(SpeechRecognitionProcess());
        startRecordingButton.interactable = false;
        SpeechRecognizer.StartRecording(true);
        resultText.text = "Cakap sesuatu";
        radial.SetActive(true);
        StartCoroutine(SpeechRecognitionProcess());
    }

    private IEnumerator SpeechRecognitionProcess()
    {
        int time = 5;

        while (time != 0)
        {
            time--;
            yield return new WaitForSeconds(1f);
        }

        SpeechRecognizer.StopIfRecording();
        startRecordingButton.interactable = true;
        radial.SetActive(false);
    }

    private void DisableBgMusic()
    {
        GameObject go = GameObject.Find("AudioManager");
        AudioSource aud = go.GetComponent<AudioSource>();
        aud.enabled = false;
    }
}