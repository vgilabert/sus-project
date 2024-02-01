using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class StatusIndicator : MonoBehaviour
{
    [SerializeField]
    private RectTransform healthBarRect;
    [SerializeField]
    private Text healthText;
    [SerializeField]
    private Slider slider;
    // Start is called before the first frame update

    void Start()
    {
        if (healthBarRect == null)
        {
            Debug.LogError("No health Bar");
        }
        if (healthText == null)
        {
            Debug.LogError("No health Text");
        }
    }

    public void SetMaxHealth(float health)
    {
        if (slider == null)
            Debug.LogError("Slider not found!");
        else
            slider.maxValue = health;
        slider.value = health;
    }
    
    public void SetHealth(float health)
    {
        slider.value = health;
    }

    // Update is called once per frame
    void Update()
    {
        Camera camera = Camera.main;
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }
}
