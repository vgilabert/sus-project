using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class TrainCar : MonoBehaviour
{
    [SerializeField] SplineContainer track;

    void FixedUpdate()
    {
        var native = new NativeSpline(track.Spline);
        float distance = SplineUtility.GetNearestPoint(native, (float3)transform.position, out float3 nearest, out float t);
        transform.position = nearest;
        
        Vector3 forward = track.EvaluateTangent(t);
        Vector3 up = track.EvaluateUpVector(t);

        var remappedForward = new Vector3(0, 1, 0);
        var remappedUp = new Vector3(0, 0, 1);
        var axisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));
        
        transform.rotation = Quaternion.LookRotation(forward, up) * axisRemapRotation;
        //transform.rotation = Quaternion.LookRotation(forward, up);
    }
}
