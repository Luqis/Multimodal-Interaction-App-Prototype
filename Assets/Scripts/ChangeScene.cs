using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

    public void GoToScene(string sceneName)
	{
		SceneManager.LoadScene (sceneName);
	}

	public void QuitGame(){
        Application.Quit();
    }

}
