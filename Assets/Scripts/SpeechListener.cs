using KKSpeech;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SpeechListener : MonoBehaviour
{
    public Button startRecordingButton;
    public Text resultText;

    public Transform clips;
    public Transform magnetPoint;
    public Transform target;

    public float sensitivity = 100f;
    public float loudness = 0;
    private float barScale = 0.1f;

    public RectTransform volumeBar;
    public AudioMixer audioMixer;
    public AudioSource _audio;
    private int currentMic = 0;
    private int minFreqs;
    private int maxFreqs;

    public float rotationDegree = 0;
    public string langId = "ms-MY";
    private List<LanguageOption> languageOptions;
    private float angle = 15f;

    private void Awake()
    {
        SetLanguage(langId);
        DisableBgMusic();
        Microphone.GetDeviceCaps(null, out minFreqs, out maxFreqs);
    }

    private void Start()
    {
        rotationDegree = TouchManager.rotationDegree;
        if (SpeechRecognizer.ExistsOnDevice())
        {
            SpeechRecognizerListener listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
            listener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
            listener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
            listener.onErrorDuringRecording.AddListener(OnError);
            listener.onErrorOnStartRecording.AddListener(OnError);
            //listener.onFinalResults.AddListener(OnFinalResult);
            listener.onPartialResults.AddListener(OnPartialResult);
            listener.onEndOfSpeech.AddListener(OnEndOfSpeech);
            listener.onSupportedLanguagesFetched.AddListener(OnSupportedLanguagesFetched);
            startRecordingButton.enabled = true;
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
                SpeechRecognizer.StopIfRecording();
            }
        }

        //SpeechLoudness();
    }

    private void FixedUpdate()
    {
        if (rotationDegree <= 305f && rotationDegree >= 295f)
        {
            clips.transform.position = Vector3.MoveTowards(clips.position, magnetPoint.position, 400 * Time.deltaTime);
        }
    }

    private void SpeechLoudness()
    {
        loudness = GetAvgVolume() * sensitivity;
        if (loudness > 0)
        {
            if (volumeBar.localScale.y >= 175f)
            {
                volumeBar.localScale = new Vector2(1f, 175f);
            }
            Mathf.Clamp(barScale, 0.1f, 175f);
            barScale = ((loudness * 10) / 175) * 100;
            volumeBar.localScale = new Vector2(1f, barScale);
        }
    }

    #region Speech Recognition properties

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
        startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
    }

    public void OnError(string error)
    {
        Debug.LogError(error);
        resultText.text = "Something went wrong... Try again! \n [" + error + "]";
        startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
        _audio.Stop();
    }

    #endregion

    public void StartMicRecording()
    {
        resultText.text = "Cakap sesuatu";
        //Stop speech recognizer
        if (SpeechRecognizer.IsRecording())
        {
            SpeechRecognizer.StopIfRecording();
            startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
        }
        else
        {
            SpeechRecognizer.StartRecording(true);
            resultText.text = "Cakap sesuatu";
            startRecordingButton.GetComponentInChildren<Text>().text = "Stop Recording";
        }
    }

    public void StartSpeechRecording()
    {
        resultText.text = "...";
        StartCoroutine(InitiateSpeechRecognition());
    }

    private IEnumerator InitiateSpeechRecognition()
    {
        int time = 2;
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
            _audio.Stop();
        }
        while (time != 0)
        {
            time--;
            resultText.text = "<" + time + ">";
            yield return new WaitForSeconds(1f);
        }
        // Start speech recognition
        SpeechRecognizer.StartRecording(true);
        // Unmute the mic
        audioMixer.SetFloat("micVol", 0);
        _audio.loop = false;
        // Play the recoded sound
        _audio.Play();
        // wait until recorded audio end
        yield return new WaitForSeconds(_audio.clip.length);
        // Stop speech recognition
        //if (SpeechRecognizer.IsRecording())
        //{
        SpeechRecognizer.StopIfRecording();
        _audio.Stop();
        startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
        //}
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

    private void DisableBgMusic()
    {
        GameObject go = GameObject.Find("AudioManager");
        AudioSource aud = go.GetComponent<AudioSource>();
        aud.enabled = false;
    }
}