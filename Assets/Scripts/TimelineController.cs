using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector playDir;
    public GameObject playPanel;
    private Scene sc;

    public void PlayVideo()
    {
        playPanel.SetActive(false);
        sc = SceneManager.GetActiveScene();
        StartCoroutine(StartVideo());
    }

    IEnumerator StartVideo()
    {
        float cliplength = AudioManager.instance.GetAudioClipLength(sc.name);

        yield return new WaitForSeconds(0.5f);
        playDir.Play();
        yield return new WaitForSeconds(0.1f);
        playDir.Pause();
        AudioManager.instance.Play(sc.name);
        yield return new WaitForSeconds(5.5f);
        playDir.Resume();

        yield return new WaitForSeconds(cliplength + 1f);
        playPanel.SetActive(true);
    }
}
