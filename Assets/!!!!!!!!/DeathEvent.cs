using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathEvent : MonoBehaviour
{

    public int sceneID;
    public Enemy _enemy;

    public void Update()
    {
        if (_enemy._hp <= 10)
        {
            StartCoroutine(LoadYourAsyncScene(sceneID));
        }
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
