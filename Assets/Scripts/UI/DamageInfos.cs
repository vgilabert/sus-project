using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class DamageInfos : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    TextMeshPro damagetext;
    void Start()
    {
        damagetext = GetComponent<TextMeshPro>();
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

    public void SetDamage(float damage)
    {
        damagetext.text = damage.ToString();
    }

    public void DestroyText()
    {
        Destroy(gameObject);
    }

}
