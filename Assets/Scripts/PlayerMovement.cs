using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public bool useMouse = true;
    
    private Vector2 MoveDirection {get; set;}
    private Vector2 AimDirection {get; set;}
    private Vector2 MousePosition {get; set;}

    public void FixedUpdate()
    {
        UpdatePosition();
        UpdateRotation();
    }

    private void UpdatePosition()
    {
        if (MoveDirection.magnitude < 0.1f) return;
        Vector2 velocity = MoveDirection * (speed * Time.deltaTime);
        transform.position += new Vector3(velocity.x, 0, velocity.y);
    }
    
    private void UpdateRotation()
    {
        if (useMouse)
        {
            if (Camera.main)
            {
                Ray ray = Camera.main.ScreenPointToRay(MousePosition);
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                plane.Raycast(ray, out var distance);
                Vector3 point = ray.GetPoint(distance);
                var direction = new Vector3(point.x, 0, point.z) - transform.position;
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg, Vector3.up);
            }
        }
        else
        {
            if (AimDirection.magnitude < 0.1f) return;
        
            float angle = Mathf.Atan2(AimDirection.x, AimDirection.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        MoveDirection = context.ReadValue<Vector2>();
        MoveDirection.Normalize();
    }
    
    public void Aim(InputAction.CallbackContext context)
    {
        if (useMouse)
        {
            MousePosition = context.ReadValue<Vector2>();
            Debug.Log(MousePosition);
            MousePosition.Normalize();
        }
        else
        {
            AimDirection = context.ReadValue<Vector2>();
            Debug.Log(AimDirection);
            AimDirection.Normalize();
        }
        
    }
    
    
}
