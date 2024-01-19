using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    public class Rocket : IProjectile
    {
        private Vector3 Target { get; set; }

        private float timeElapsed;
        private Vector3 start;
        
        private float height = 10;
        
        private float t;

        private void Start()
        {
            start = transform.position;
            height = 10;
            t = 0;
        }

        public override void Update()
        {
            UpdateRocketPosition();
        }
        
        private void UpdateRocketPosition()
        {
            timeElapsed += Time.deltaTime;
            timeElapsed %= Speed;
            transform.position = Parabola(start, Target, height, timeElapsed / Speed);
            if (timeElapsed >= Speed)
            {
                Explode();
            }
        }

        private void Explode()
        {
            Destroy(gameObject);
        }

        protected override void OnHit(RaycastHit hit)
        {
        
        }
    
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }
    }
}
