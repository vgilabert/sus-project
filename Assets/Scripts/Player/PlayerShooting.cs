using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

namespace Player
{
    public class PlayerShooting : MonoBehaviour
    {
        [SerializeField] private Gun gun;
    
        private bool IsShooting {get; set; }
        
        public void Awake()
        {
            gun = GetComponentInChildren<Gun>();
        }
        
        public void FixedUpdate()
        {
            if (IsShooting)
            {
                gun.Shoot();
            }
        }
        
        public void Shoot(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsShooting = true;
            }
            else if (context.canceled)
            {
                IsShooting = false;
            }
        }
    }
}
