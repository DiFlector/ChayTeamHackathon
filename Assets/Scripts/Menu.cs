using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public int sceneID;
    public void StartGame(int sceneID)
    {
        StartCoroutine(LoadYourAsyncScene(sceneID));
    }

    public void Exit()
    {
        Application.Quit();
    }

    IEnumerator LoadYourAsyncScene(int sceneID)
    {

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneID);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

}
