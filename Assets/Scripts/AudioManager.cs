using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public GameObject SettingPanel = null;

    public Button musicOn;
    public Button musicOff;
    public AudioSource bgMusic;
    public bool playMusic = true;
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
        ValidateSoundName(name).source.Play();
    }

    public void Stop(string name)
    {
        ValidateSoundName(name).source.Stop();
    }

    private Sound ValidateSoundName(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
        return s;
    }

    public float GetAudioClipLength(string name)
    {
        float cliplength = ValidateSoundName(name).source.clip.length;
        return cliplength;
    }

    public void CheckSceneName(string name)
    {
        if (Array.Exists(sceneNameArray, sceneName => sceneName == name))
        {
            bgMusic.enabled = false;
        }
        else if (playMusic)
        {
            bgMusic.enabled = true;
        }

        if (name == "Main")
        {
            SettingPanel = GameObject.Find("SettingPanel");
            if (SettingPanel != null)
            {
                musicOn = GameObject.Find("BtnYes").GetComponent<Button>();
                musicOff = GameObject.Find("BtnNo").GetComponent<Button>();

                if (musicOff.IsActive())
                {
                    musicOff.onClick.AddListener(DisableBgMusic);
                }

                if (musicOn.IsActive())
                {
                    musicOn.onClick.AddListener(EnableBgMusic);
                }
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