using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    private Scene sc;
    public Button[] SceneButtons;

    private void Awake()
    {
        sc = SceneManager.GetActiveScene();
        SceneButtons = FindObjectsOfType<Button>();
        ToggleButtonsInScene(false);
    }

    private void Start()
    {
        StartCoroutine(PlaySceneIntroSound(sc.name));
    }

    private void Update()
    {
    }

    public void CheckSceneName(string name)
    {
        if (name.Contains("Speech"))
        {
        }
    }

    private IEnumerator EnableDialogBox(string sceneName)
    {
        yield return new WaitForSeconds(5f);
    }

    private IEnumerator PlaySceneIntroSound(string sceneName)
    {
        bool IntroPlayed = AudioManager.instance.IntroPlayed[sc.buildIndex];
        string introSound = "intro" + sceneName;
        float cliplength = AudioManager.instance.GetAudioClipLength(introSound);

        if (!IntroPlayed)
        {
            AudioManager.instance.Play(introSound);
            yield return new WaitForSeconds(cliplength);
            ToggleButtonsInScene(true);
            AudioManager.instance.IntroPlayed[sc.buildIndex] = true;
        }
        else
            ToggleButtonsInScene(true);
    }

    private void ToggleButtonsInScene(bool status)
    {
        foreach (Button btn in SceneButtons)
        {
            btn.GetComponent<Button>().interactable = status;
        }
    }

    public void GoToScene(string sceneName)
    {
        StopCoroutine(PlaySceneIntroSound(sc.name));
        SceneManager.LoadScene(sceneName);
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
}