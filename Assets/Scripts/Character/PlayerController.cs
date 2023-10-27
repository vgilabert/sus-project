using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAim))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GunController))]
public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    PlayerAim playerAim;
    bool isFiring; 

    GunController guncontroller;
    
    public float speed = 20f;
    public Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        playerAim = GetComponent<PlayerAim>();
        guncontroller = GetComponent<GunController>();
        rb = GetComponent<Rigidbody>();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        velocity = new Vector3(moveInput.x, 0, moveInput.y) * speed;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z) * (speed * Time.fixedDeltaTime);
        if (isFiring)
        {
            guncontroller.Shoot();
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            isFiring = true;
        }
        if (context.canceled)
        {
            isFiring = false;
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerAim.Aim(context.ReadValue<Vector2>());
            //Debug.Log(context.ReadValue<Vector2>());
        }
    }
}