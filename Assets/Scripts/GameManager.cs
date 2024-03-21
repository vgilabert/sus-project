using System.Collections;
using Character;
using UnityEngine;
using Utils;

public class GameManager : MonoSingleton<GameManager>
{
    private bool _isGamePaused = false;

    [SerializeField]
    bool showIndicators = true;
    [SerializeField]
    bool GenerateScrap = true;

    void Start()
    {
        if (GenerateScrap)
        {
            StartCoroutine(GenerateScrapOverTime());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isGamePaused = !_isGamePaused;
            SetGamePause(_isGamePaused);
        }
    }
    
    private IEnumerator GenerateScrapOverTime()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(1);
            if (!Inventory.Instance) continue;
            Inventory.Instance.AddScrap(10);
        }
    }

    public void SetGamePause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1; 
    }
}