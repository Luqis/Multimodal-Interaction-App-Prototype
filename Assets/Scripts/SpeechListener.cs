using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KKSpeech;
using System.Collections.Generic;

public class SpeechListener : MonoBehaviour
{
    public Button startRecordingButton;
    public Text resultText;

    public Transform clips;
    public Transform magnetPoint;
    public Transform target;
    public GameObject volume;

    public float sensitivity = 100f;
    public float loudness = 0;

    public RectTransform volumeBar;
    public AudioSource _audio;
    int currentMic = 0;
    int minFreqs;
    int maxFreqs;

    public float rotationDegree = 0;
    public string langId = "ms-MY";
    private List<LanguageOption> languageOptions;
    private float angle = 15f;

    private void Awake()
    {
        SetLanguage(langId);
    }

    void Start()
    {
        Microphone.GetDeviceCaps(null, out minFreqs, out maxFreqs);
        _audio.loop = true;
        rotationDegree = TouchManager.rotationDegree;
        if (SpeechRecognizer.ExistsOnDevice())
        {
            SpeechRecognizerListener listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
            listener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
            listener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
            listener.onErrorDuringRecording.AddListener(OnError);
            listener.onErrorOnStartRecording.AddListener(OnError);
            listener.onFinalResults.AddListener(OnFinalResult);
            listener.onPartialResults.AddListener(OnPartialResult);
            listener.onEndOfSpeech.AddListener(OnEndOfSpeech);
            listener.onSupportedLanguagesFetched.AddListener(OnSupportedLanguagesFetched);
            startRecordingButton.enabled = false;
            SpeechRecognizer.GetSupportedLanguages();
            SpeechRecognizer.RequestAccess();
        }
        else
        {
            resultText.text = "Sorry, but this device doesn't support speech recognition";
            startRecordingButton.enabled = false;
        }

    }

    private void Update()
    {
        rotationDegree = target.transform.eulerAngles.z;

        if (resultText.text == "kanan" || resultText.text == "gerak" || resultText.text == "pusing")
        {
            target.Rotate(0, 0, -0.5f);

            if (rotationDegree == 345f || rotationDegree == 330f || rotationDegree == 315f || rotationDegree <= 300f)
            {
                resultText.text = "...";
                
            }
            
            SpeechRecognizer.StopIfRecording();
        }

        loudness = GetAvgVolume() * sensitivity;
        if (loudness > 1)
        {
            volumeBar.localScale = new Vector2(1f, loudness);
        }
    }

    private void FixedUpdate()
    {
        if (rotationDegree <= 305f && rotationDegree >= 295f)
        {
            clips.transform.position = Vector3.MoveTowards(clips.position, magnetPoint.position, 400 * Time.deltaTime);
        }
    }

    void SetLanguage(string id)
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
        startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
    }

    public void OnError(string error)
    {
        Debug.LogError(error);
        resultText.text = "Something went wrong... Try again! \n [" + error + "]";
        startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
    }

    public void OnStartRecordingPressed()
    {
        if (SpeechRecognizer.IsRecording())
        {
            SpeechRecognizer.StopIfRecording();
            startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
        }
        else
        {
            SpeechRecognizer.StartRecording(true);
            _audio.clip = Microphone.Start(null, true, 3, maxFreqs);
            _audio.Play();
            startRecordingButton.GetComponentInChildren<Text>().text = "Stop Recording";
            resultText.text = "Cakap sesuatu";
        }
    }

    public float GetAvgVolume()
    {
        float[] data = new float[256];
        float a = 0;
        _audio.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;
    }
}
