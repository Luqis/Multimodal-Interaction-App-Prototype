using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class SpeechRecognition : MonoBehaviour
{
    public string[] keywords = { "kanan pusing", "besar zoom", "kecil zoom" };
    public ConfidenceLevel confidence = ConfidenceLevel.Low;
    public float speed = 1;

    public float sensitivity = 100f;
    public float loudness = 0;

    public AudioSource _audio;

    public Text results;
    public Image target;

    protected PhraseRecognizer recognizer;
    protected string word;

    private void Reset()
    {
        _audio = FindObjectOfType<AudioSource>();
    }

    private void Awake()
    {
        foreach (string device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
        _audio.clip = Microphone.Start(null, true, 10, 44100);
        _audio.loop = true;

        while (!(Microphone.GetPosition(null) > 0)) { }
        _audio.Play();
    }

    private void Start()
    {
        if (keywords != null)
        {
            recognizer = new KeywordRecognizer(keywords, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
        }
    }

    public void BeginListener(int index)
    {
        int min = 0;
        int max = 0;

        Microphone.GetDeviceCaps(Microphone.devices[index], out min, out max);

        _audio.clip = Microphone.Start(Microphone.devices[index], true, 5, max);

        while (!(Microphone.GetPosition(Microphone.devices[index]) > 1))
        {
            // Wait until the recording has started
        }

        _audio.loop = true;
        _audio.Play();
    }

    private void Update()
    {
        loudness = GetAverageVolume() * sensitivity;

        switch (word)
        {
            case "kanan pusing":
                target.transform.Rotate(Vector3.back, Space.World);
                break;
            case "besar zoom":
                target.transform.Rotate(Vector3.forward, Space.World);
                //target.transform.localScale += Vector3.one;
                break;
            case "kecil zoom":
                target.transform.localScale -= Vector3.one;
                break;
        }

    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        word = args.text;
        results.text = "You said: <b>" + word + "</b>";
    }

    private float GetAverageVolume()
    {
        float[] data = new float[256];
        float a = 0;
        _audio.GetOutputData(data, 0);
        foreach (float item in data)
        {
            a += Mathf.Abs(item);
        }
        return a / 256;
    }

    private void OnApplicationQuit()
    {
        if (recognizer != null && recognizer.IsRunning)
        {
            recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            recognizer.Stop();
            _audio.Stop();
        }
    }
}