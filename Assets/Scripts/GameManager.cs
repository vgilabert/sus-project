using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isGamePaused = false;

    private static GameManager instance;
    [SerializeField]
    bool showIndicators = true;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError("Game Manager not found!");
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }
    void Start()
    {


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
   
    public void SetGamePause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1; 
    }
}