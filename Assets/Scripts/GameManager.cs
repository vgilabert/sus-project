using UnityEngine;
using Utils;

public class GameManager : MonoSingleton<GameManager>
{
    public bool isGamePaused = false;

    [SerializeField]
    bool showIndicators = true;
    
    private void OnEnable()
    {
        UIGlobalController.setGamePause += SetGamePauseHandler;
    }

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
   
    public void SetGamePauseHandler(bool pause)
    {
        SetGamePause(pause);
    }
    
    public void SetGamePause(bool pause)
    {
        Time.timeScale = pause ? 0.1f : 1; 
    }
}