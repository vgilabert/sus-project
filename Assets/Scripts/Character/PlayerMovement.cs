using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 5f;
    public Vector3 velocity;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    /*void Update()
    {
        Vector3 moveInput  = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        velocity = moveInput.normalized * speed;
    }*/
    
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        velocity = new Vector3(moveInput.x, 0, moveInput.y) * speed;
    }
}
