using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject Train;

    
    public float camOffsetX;
    public float camOffsetZ;

    public float height = 13f;

    [Range(0.01f, 1.0f)]
    public float SmoothFactorFollow = 0.5f;
    
    public bool lookAtPlayer = false;
    
    [Range(0.01f, 1.0f)]
    public float SmoothFactorLookAt = 0.5f;

    Vector3 center;

    void Start()
    {
        Train = GameObject.FindWithTag("Loco");
        center = Train.GetComponent<MyTrain>().GetCenter();
        //camOffsetZ = gameObject.transform.position.z - center.z;
        //camOffsetX = gameObject.transform.position.x - center.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Vector3 newPos = Player.position + camOffsetZ
        if(Train)
        {
            Vector3 m_cameraPos = new Vector3(center.x + camOffsetX, height, center.z + camOffsetZ);

            transform.position = Vector3.Slerp(transform.position, m_cameraPos, SmoothFactorFollow);
        }
        LookAtPlayer();

        center = Train.GetComponent<MyTrain>().GetCenter();
    }
    
    void LookAtPlayer()
    {
        if (lookAtPlayer)
        {
            Quaternion rotation = Quaternion.LookRotation(center - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, SmoothFactorLookAt);
        }
    }
}