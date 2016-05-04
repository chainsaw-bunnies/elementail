using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadScene : MonoBehaviour
{
	public void LoadSceneByIndex(int sceneIndex)
	{
		SceneManager.LoadScene(sceneIndex);
	}

	public void LoadSceneByName(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

  public void ResetLevel()
  {
    GameStatus.Level = 0;
    GameStatus.LevelsCompleted = 0;
    GameStatus.Score = 0;
  }

  public void Quit()
  {
    Application.Quit();
  }

}
