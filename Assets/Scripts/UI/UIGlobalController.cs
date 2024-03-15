using UnityEngine;
using UnityEngine.InputSystem;

public class UIGlobalController : MonoBehaviour
{
    [SerializeField]
    private GameObject uiTrain;
    [SerializeField]
    private GameObject uiOverlay;

    
    public void OnTrainMenuOpen(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!uiTrain.activeSelf) {
                GameManager.Instance.SetGamePause(true);
                uiTrain.SetActive(true);
            }
            else
            {
                GameManager.Instance.SetGamePause(false);
                uiTrain.SetActive(false);
            }
            
        }
    }
}
