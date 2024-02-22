using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]

    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private bool useMouse = true;
    [SerializeField]
    private float smoothInputSpeed = 0.3f;

    private Vector2 currentInputVector;
    private Vector2 currentVelocity;
    
    private Vector2 MoveDirection {get; set;}
    public Vector2 AimDirection {get; private set;}
    private Vector2 MousePosition {get; set;}

    public void FixedUpdate()
    {
        UpdatePosition();
        UpdateRotation();
    }

    private void UpdatePosition()
    {
        currentInputVector = Vector2.SmoothDamp(currentInputVector, MoveDirection, ref currentVelocity, smoothInputSpeed);
        var moveVector = new Vector3(currentInputVector.x, 0, currentInputVector.y) ;
        transform.position += moveVector * speed * Time.deltaTime;
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
        }
        else
        {
            AimDirection = context.ReadValue<Vector2>();
            AimDirection.Normalize();
        }
    }
}
