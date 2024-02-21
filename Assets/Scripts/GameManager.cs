using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class GameManager : MonoSingleton<GameManager>
{
    public bool isGamePaused = false;

    [SerializeField]
    bool showIndicators = true;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isGamePaused = !isGamePaused;
            SetGamePause(isGamePaused);
        }

        if(isGamePaused)
        {

        }
    }
   
    public void SetGamePause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1; 
    }
}