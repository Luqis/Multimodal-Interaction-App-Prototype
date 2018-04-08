using UnityEngine;

public class GameControl : MonoBehaviour
{

    public static GameControl instance;

    public bool[] rotateTouchTask;
    public bool[] rotateSpeechTask;
    public bool[] zoomInTouchTask;
    public bool[] zoomInSpeechTask;
    public bool[] zoomOutTouchTask;
    public bool[] zoomOutSpeechTask;

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

    }
}
