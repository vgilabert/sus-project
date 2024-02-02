using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Cart cart;
    private PlayerStats player;
    public Vector3 offset;
    
    void Start()
    {
        cart = FindFirstObjectByType<Cart>();
        player = FindFirstObjectByType<PlayerStats>();
    }
    
    void Update()
    {
        if (!cart)
        {
            cart = FindFirstObjectByType<Cart>();
        }
        else
        {
            // Find the middle between the player and the cart
            var target = (cart.transform.position + player.transform.position) / 2;
            transform.position = target + offset;
            transform.LookAt(target);
        }
    }
}
