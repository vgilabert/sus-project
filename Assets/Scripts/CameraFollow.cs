using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform Player;

    
    public float camOffsetX;
    public float camOffsetZ;

    public float height = 13f;

    [Range(0.01f, 1.0f)]
    public float SmoothFactorFollow = 0.5f;
    
    public bool lookAtPlayer = false;
    
    [Range(0.01f, 1.0f)]
    public float SmoothFactorLookAt = 0.5f;

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
        if(Player)
        {
            Vector3 m_cameraPos = new Vector3(Player.position.x + camOffsetX, height, Player.position.z + camOffsetZ);

            transform.position = Vector3.Slerp(transform.position, m_cameraPos, SmoothFactorFollow);
        }
        LookAtPlayer();
    }
    
    void LookAtPlayer()
    {
        if (lookAtPlayer)
        {
            Quaternion rotation = Quaternion.LookRotation(Player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, SmoothFactorLookAt);
        }
    }
}