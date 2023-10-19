using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    Camera viewCamera;
    // Start is called before the first frame update
    void Start()
    {
        viewCamera= Camera.main;
    }

    // Update is called once per frame
    public void Aim(Vector2 mousePosition)
    {
        Ray ray = viewCamera.ScreenPointToRay(mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, point, Color.red);
            Debug.DrawRay(ray.origin,ray.direction * 100,Color.red);
            LookAt(point);
        }
    }

    public void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }


}
