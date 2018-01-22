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
        if (sc.name != "MagnetSpeech" && playMusic)
        {
            bgMusic.enabled = true;
        }
        else
        {
            bgMusic.enabled = false;
        }

        if (sc.name == "Main")
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
