using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public string startScene = "Forest";
	public void StartGame()
	{
		SceneManager.LoadScene(startScene);
	}
}
