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
    public Transform successPanel;
    public Button restartButton;
    private bool endGame = false;

    [Header("Speech Mode")]
    public bool scaleUp;
    public bool scaleDown;
    public bool rotate;

    [Header("Scale Limit")]
    [SerializeField]
    private float minScale = 0.425f;
    [SerializeField]
    private float maxScale = 2.5f;

    [Header("Magnet Reference")]
    public Transform clips;
    public Transform magnetPoint;
    public Transform target;

    private float[] rotationDegrees = { 355f, 345f, 330f }; //{ 350f, 340f, 330f, 320f, 310f };
    private float[] scaleUpArray = { 1.25f, 1.5f, 1.75f, 2.25f };
    private float[] scaleDownArray = { 0.90f, 0.85f, 0.75f, 0.60f };
    private List<LanguageOption> languageOptions;
    private string langId = "ms-MY";
    private string[] magnetKeywords = { "kanan", "gerak", "pusing" };
    private string[] scaleUpKeywords = { "besar", "zoom" };
    private string[] scaleDownKeywords = { "kecil", "zoom" };
    public float rotationDegree = 0;
    public float targetScale = 1;
    public Image targetOutline;

    private void Awake()
    {
        SetLanguage(langId);
        DisableBgMusic();

        successPanel = GameObject.Find("SuccessPanel").GetComponent<Transform>();
        restartButton = GameObject.Find("Restart-btn").GetComponent<Button>();
    }

    private void Start()
    {
        successPanel.gameObject.SetActive(false);
        restartButton.interactable = false;
        endGame = true;

        if (SpeechRecognizer.ExistsOnDevice())
        {
            SpeechRecognizerListener listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
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
            resultText.text = "Sorry, but this device doesn't support speech recognition";
            startRecordingButton.interactable = false;
        }
    }

    private void Update()
    {
        rotationDegree = target.transform.eulerAngles.z;
        targetScale = target.transform.localScale.x;

        if (rotate)
        {
            VerifyKeyword(magnetKeywords);

            if (rotationDegree <= 305f && rotationDegree >= 295f)
            {
                clips.transform.position = Vector3.MoveTowards(clips.position, magnetPoint.position, 400 * Time.deltaTime);
                if (clips.position == magnetPoint.position && endGame)
                {
                    StartCoroutine(ActiveSuccessPanel());
                    endGame = false;
                }
                targetOutline.color = new Color(0, 1, 0, 0.5f);
            }
            else
                targetOutline.color = new Color(1, 0, 0, 0.5f);
        }

        if (scaleUp)
        {
            VerifyKeyword(scaleUpKeywords);
        }

        if (scaleDown)
        {
            VerifyKeyword(scaleDownKeywords);
        }
    }

    IEnumerator ActiveSuccessPanel()
    {
        if (scaleDown)
        {
            yield return new WaitForSeconds(1f);
            playDir.Play();
            yield return new WaitForSeconds(2f);
        }

        yield return new WaitForSeconds(1f);
        AudioManager.instance.Play("yeay");
        successPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        restartButton.interactable = true;
    }

    private void VerifyKeyword(string[] keywords)
    {
        if (rotate)
        {
            if (Array.Exists(keywords, word => word == resultText.text))
            {
                target.Rotate(0, 0, -1f);

                if (Array.Exists(rotationDegrees, degree => degree == rotationDegree) || rotationDegree <= 300f)
                {
                    resultText.text = "...";
                    SpeechRecognizer.StopIfRecording();
                }

                UpdateDegreeOfRotationText();
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
                    if (endGame)
                    {
                        StartCoroutine(ActiveSuccessPanel());
                        endGame = false;
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
                    targetOutline.color = new Color(0, 1, 0, 1f);
                    if (endGame)
                    {
                        StartCoroutine(ActiveSuccessPanel());
                        endGame = false;
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
            resultText.text = "Speech Recognition not available";
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
        //resultText.text = "Try again! [" + error + "]";
    }

    #endregion Speech Recognition Properties

    public void StartSpeechRecording()
    {
        StopCoroutine(SpeechRecognitionProcess());
        startRecordingButton.interactable = false;
        SpeechRecognizer.StartRecording(true);
        resultText.text = "Cakap sesuatu";
        StartCoroutine(SpeechRecognitionProcess());
    }

    private IEnumerator SpeechRecognitionProcess()
    {
        int time = 5;

        while (time != 0)
        {
            time--;
            startRecordingButton.GetComponentInChildren<Text>().text = "<" + time + ">";
            yield return new WaitForSeconds(1f);
        }

        SpeechRecognizer.StopIfRecording();
        startRecordingButton.interactable = true;
        startRecordingButton.GetComponentInChildren<Text>().text = "Mula rakaman";
    }

    private void DisableBgMusic()
    {
        GameObject go = GameObject.Find("AudioManager");
        AudioSource aud = go.GetComponent<AudioSource>();
        aud.enabled = false;
    }
}