using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    public class Rocket : IProjectile
    {
        public Vector3 Target { get; set; }

        private float timeElapsed;
        private Vector3 start;

        float Duration;
        

        public void Start()
        {
            start = transform.position;
        }

        public override void Update()
        {
            UpdateRocketPosition();
        }

        public override void Initialize(Gun gun, Transform target = null)
        {
            if (target)
            {
                Target = target.position;
            }
            BaseDamage = gun.Damage;
            Height = gun.RocketHeight;
            Duration = gun.FlyDuration;
        }

        private void UpdateRocketPosition()
        {
            Vector3 previous = transform.position;
            timeElapsed += Time.deltaTime;
            // log every parameters used to compute the parabola
            transform.position = Parabola(start, Target, Height, timeElapsed / Duration);

            // rotate the rocket towards the tangent of the parabola
            Vector3 tangent = transform.position - previous;
            transform.rotation = Quaternion.LookRotation(tangent);

            if (timeElapsed >= Duration) Explode();
        }

        private void Explode()
        {
            Destroy(gameObject);
        }

        protected override void OnHit(RaycastHit hit)
        {
        
        }
    
        static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            Vector3 mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }
    }
}
