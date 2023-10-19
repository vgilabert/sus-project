using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour
{
    [SerializeField]
    private RectTransform healthBarRect;
    [SerializeField]
    private Text healthText;
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

    public void SetHealth(int _cur, int _max)
    {
        float _value = (float)_cur / _max;
        healthBarRect.localScale = new Vector3(_value, healthBarRect.localScale.y , healthBarRect.localScale.z);
        //healthBarRect.GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, _value);
        healthText.text = _cur + "/" + _max + " HP";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
