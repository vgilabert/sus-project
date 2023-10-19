using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform Player;

    float camOffsetZ;
    float camOffsetX;

    public float height = 13f;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;

    void Start()
    {
        Player = GameObject.FindWithTag("Player").transform;
        camOffsetZ = gameObject.transform.position.z - Player.position.z;
        camOffsetX = gameObject.transform.position.x - Player.position.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Vector3 newPos = Player.position + camOffsetZ
        if(Player != null)
        {
            Vector3 m_cameraPos = new Vector3(Player.position.x + camOffsetX, height, Player.position.z +camOffsetZ);

            transform.position = Vector3.Lerp(transform.position, m_cameraPos, SmoothFactor);
        }
        
    }
}
