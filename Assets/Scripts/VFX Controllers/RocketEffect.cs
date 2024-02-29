using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VFX_Controllers
{
    public class RocketEffect : MonoBehaviour
    {
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _progress;
        private float _flightDuration;
        
        public float height = 3f;

        private void Start()
        {
            _startPosition = transform.position;
        }
    
        private void Update()
        {
            UpdateRocketPosition();
        }
    
        public void SetTargetPosition(Vector3 position)
        {
            _targetPosition = position;
        }
        
        public void SetFlightDuration(float duration)
        {
            _flightDuration = duration;
        }
    
        private void UpdateRocketPosition()
        {
            _progress += Time.deltaTime / _flightDuration;
            transform.position = Parabola(_startPosition, _targetPosition, height, _progress / _flightDuration);
            if (_progress >= _flightDuration)
            {
                Explode();
            }
        }
    
        private void Explode()
        {
            Destroy(gameObject);
        }
    
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }
    
    }
}
