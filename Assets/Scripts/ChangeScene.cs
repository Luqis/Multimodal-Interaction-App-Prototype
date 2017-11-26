using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {
	
	public void GoToScene(string sceneName)
	{
		SceneManager.LoadScene (sceneName);
	}

	public void StopMusic () {
		Destroy(GameObject.Find("MUSIC"));	
	}

	public void Logout(string sceneName){
		Destroy (GameObject.Find ("prefManager"));
		SceneManager.LoadScene (sceneName);
	}

}
