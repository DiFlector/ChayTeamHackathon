using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject _pauseUI;
    public int indexLevel;
    private void Start()
    {
        GameIsPaused = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause_menu();
            }
        }
    }
    public void Resume() 
    {
        Cursor.visible = false;
        _pauseUI.SetActive(false);
        Time.timeScale += 1;
        GameIsPaused = false;
    }

    private void Pause_menu()
    {
        Cursor.visible = true;
        _pauseUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Restart()
    {
        SceneManager.LoadSceneAsync(indexLevel);
    }


    public void Exit()
    {
        Application.Quit();
    }
}
