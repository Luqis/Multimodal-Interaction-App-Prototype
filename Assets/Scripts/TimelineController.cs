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
        AudioManager.instance.Play(sc.name);
        playDir.Play();

        yield return new WaitForSeconds(cliplength + 1f);
        playPanel.SetActive(true);
    }
}
