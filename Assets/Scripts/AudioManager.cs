using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Button musicOn;
    public Button musicOff;
    public AudioSource bgMusic;
    public bool playMusic = true;
    public bool hasSound = true;
    public bool[] IntroPlayed;

    [Header("Sound List")]
    public Sound[] sounds;

    private string[] sceneNameArray = { "MagnetSpeech", "UdaraSpeech", "PlanetSpeech" };
    private Scene sc;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        LoopSoundArray();

        if (playMusic)
            musicOff.gameObject.SetActive(false);
    }

    private void Update()
    {
        sc = SceneManager.GetActiveScene();
        CheckSceneName(sc.name);
        Debug.Log(sc.name);
    }

    public void LoopSoundArray()
    {
        IntroPlayed = new bool[SceneManager.sceneCountInBuildSettings];
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.priority = s.priority;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        if (hasSound)
            ValidateSoundName(name).source.Play();
    }

    public void Stop(string name)
    {
        if (hasSound)
            ValidateSoundName(name).source.Stop();
    }

    public bool IsPlaying(string name)
    {
        bool status = ValidateSoundName(name).source.isPlaying;
        return status;
    }

    private Sound ValidateSoundName(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            hasSound = false;
        }
        else
            hasSound = true;

        return s;
    }

    public float GetAudioClipLength(string name)
    {
        float cliplength = ValidateSoundName(name).source.clip.length;
        return cliplength;
    }

    public void CheckSceneName(string name)
    {
        if (name.Contains("Speech"))
        {
            bgMusic.enabled = false;
        }
        else if (playMusic)
        {
            bgMusic.enabled = true;
        }

        if (name == "Main")
        {
            GameObject obj, obj2;
            obj = GameObject.Find("BtnSoundOn");
            obj2 = GameObject.Find("BtnSoundOff");

            if (obj != null && obj2 != null)
            {
                musicOn = obj.GetComponent<Button>();
                if (musicOn.IsActive())
                    musicOn.onClick.AddListener(DisableBgMusic);

                musicOff = obj2.GetComponent<Button>();
                if (musicOff.IsActive())
                    musicOff.onClick.AddListener(EnableBgMusic);

                if (playMusic)
                    musicOff.gameObject.SetActive(false);
            }
        }
    }

    public void EnableBgMusic()
    {
        bgMusic.enabled = true;
        playMusic = true;
    }

    public void DisableBgMusic()
    {
        bgMusic.enabled = false;
        playMusic = false;
    }
}