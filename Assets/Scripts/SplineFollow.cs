using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine;

public class SplineFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public SplineContainer spline;
    public float speed = 1f;
    float distancePercentage = 0f;

    float splineLength;

    float height;
    void Start()
    {
        splineLength = spline.CalculateLength();
        height = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        distancePercentage += speed * Time.deltaTime / splineLength;

        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        currentPosition.y += height;

        transform.position = currentPosition;

        if (distancePercentage > 1f)
        {
            distancePercentage = 0f;
        }

        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
        nextPosition.y += height;
        Vector3 direction = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }
}
