using KKSpeech;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechListener : MonoBehaviour
{
    public Button startRecordingButton;
    public Text resultText;
    public Text rotationText;

    [Header("Magnet Reference")]
    public Transform clips;
    public Transform magnetPoint;
    public Transform target;

    private float[] rotationDegrees = { 348f, 336f, 324f, 312f };
    private List<LanguageOption> languageOptions;
    private string langId = "ms-MY";
    private string[] keywords = { "kanan", "gerak", "pusing" };
    public float rotationDegree = 0;
    public Image magnetOutline;

    private void Awake()
    {
        SetLanguage(langId);
        DisableBgMusic();
    }

    private void Start()
    {
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
        VerifyKeyword();

        if (rotationDegree <= 305f && rotationDegree >= 295f)
        {
            clips.transform.position = Vector3.MoveTowards(clips.position, magnetPoint.position, 400 * Time.deltaTime);
            magnetOutline.color = new Color(0, 1, 0, 0.5f);
        }
        else
            magnetOutline.color = new Color(1, 0, 0, 0.5f);
    }

    private void VerifyKeyword()
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