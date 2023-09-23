using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    public void StartGame()
    {
        Application.LoadLevelAsync(1);
    }

    public void Exit()
    {
        Application.Quit();
    }
    
}
