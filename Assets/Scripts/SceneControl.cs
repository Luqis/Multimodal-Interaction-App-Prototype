using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public string currentSceneSound = string.Empty;
    private Scene sc;
    private bool CR_running;

    private void Awake()
    {
        sc = SceneManager.GetActiveScene();
    }

    private void Start()
    {
        StartCoroutine(PlaySceneIntroSound(sc.name));
    }

    private void Update()
    {
    }

    public void CheckSceneName(ref string name, ref bool status)
    {
        if (name.Contains("Video"))
        {
            name = sc.name;
            status = true;
            AudioManager.instance.IntroPlayed[sc.buildIndex] = true;
        }
    }

    private IEnumerator PlaySceneIntroSound(string sceneName)
    {
        CR_running = true;

        bool IntroPlayed = AudioManager.instance.IntroPlayed[sc.buildIndex];
        currentSceneSound = "intro" + sceneName;
        CheckSceneName(ref currentSceneSound, ref IntroPlayed);
        float cliplength = AudioManager.instance.GetAudioClipLength(currentSceneSound);

        if (!IntroPlayed)
        {
            yield return new WaitForSeconds(1.5f);
            AudioManager.instance.bgMusic.volume = 0.05f;
            AudioManager.instance.Play(currentSceneSound);
            AudioManager.instance.IntroPlayed[sc.buildIndex] = true;

            yield return new WaitForSeconds(cliplength);
            AudioManager.instance.bgMusic.volume = 0.25f;
        }
        CR_running = false;
    }

    public void GoToScene(string sceneName)
    {
        AudioManager.instance.Stop(currentSceneSound);
        AudioManager.instance.Stop("panduan");
        AudioManager.instance.bgMusic.volume = 0.25f;
        StopCoroutine(PlaySceneIntroSound(sc.name));
        SceneManager.LoadScene(sceneName);
    }

    public void ToggleSceneIntroSound()
    {
        bool status = AudioManager.instance.IsPlaying(currentSceneSound);

        if (status)
        {
            AudioManager.instance.Stop(currentSceneSound);
            AudioManager.instance.bgMusic.volume = 0.25f;
            StopCoroutine(PlaySceneIntroSound(sc.name));
        }
        else if (!CR_running)
        {
            AudioManager.instance.IntroPlayed[sc.buildIndex] = false;
            StartCoroutine(PlaySceneIntroSound(sc.name));
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ButtonClicked()
    {
        AudioManager.instance.Play("btnClick");
    }

    public void ButtonClicked(string soundName)
    {
        AudioManager.instance.Play("btnClick");
        AudioManager.instance.Play(soundName);
    }

    public void BtnClkStopSound()
    {
        AudioManager.instance.Stop(currentSceneSound);
        AudioManager.instance.bgMusic.volume = 0.25f;
        StopCoroutine(PlaySceneIntroSound(sc.name));
    }
}