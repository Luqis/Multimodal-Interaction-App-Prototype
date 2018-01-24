using System.Collections;
using UnityEngine;
using UnityEngine.Playables;


public class TimelineController : MonoBehaviour
{
    public PlayableDirector playDir;
    public GameObject playPanel;

    public void PlayVideo()
    {
        playPanel.SetActive(false);
        StartCoroutine(StartVideo());
    }

    IEnumerator StartVideo()
    {
        yield return new WaitForSeconds(0.5f);
        playDir.Play();
        yield return new WaitForSeconds(4.5f);
        playPanel.SetActive(true);
    }
}
