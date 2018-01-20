using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    public AudioSource _bgMusic;
    Scene sc;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        sc = SceneManager.GetActiveScene();

        if (sc.name != "MagnetSpeech")
        {
            if (!_bgMusic.enabled)
            {
                _bgMusic.enabled = true;
            }
        }
    }
}
