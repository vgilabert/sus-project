using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAim))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GunController))]
public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    PlayerMovement playerMovement;
    PlayerAim playerAim;

    GunController guncontroller;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAim = GetComponent<PlayerAim>();
        guncontroller = GetComponent<GunController>();
        rb = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + playerMovement.velocity * Time.fixedDeltaTime);
        //playerAim.Aim();
    }

    /*private void Update()
    {

        if (Input.GetMouseButton(0))
        {
            guncontroller.Shoot();
        }
    }*/

    public void OnShoot(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            guncontroller.Shoot();
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