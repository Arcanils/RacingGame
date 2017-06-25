using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour {

	

	public void Play()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(1);
	}

	public void Upgrade()
	{

	}

	public void Quit()
	{
		Application.Quit();
	}
}
