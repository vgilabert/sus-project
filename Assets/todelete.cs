using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class todelete : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var follower = GetComponent<SplineFollower>();
        follower.onNode += OnNode;
    }
    
    private void OnNode(List<SplineTracer.NodeConnection> nodeConnection)
    {
        Debug.Log("on node");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
