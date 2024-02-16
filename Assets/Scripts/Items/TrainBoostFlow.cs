using System;
using System.Collections;
using UnityEngine;

namespace Items
{
    public class TrainBoostFlow : ItemFlow
    {
        public static Action<float, float> OnTrainBoostStart;
        public static Action OnTrainBoostEnd;
        
        private float _boostDuration = 8f;
        private float _damageBoost = 50f;
        private float _fireRateBoost = 30f;
        
        public override void StartFlow(Action<bool> callback)
        {
            StartCoroutine(Flow(callback));
        }

        IEnumerator Flow(Action<bool> callback)
        {
            OnTrainBoostStart?.Invoke(_damageBoost, _fireRateBoost);
            Debug.Log("Train boost started !");
            yield return new WaitForSeconds(_boostDuration);
            OnTrainBoostEnd?.Invoke();
            Debug.Log("Train boost ended !");
            callback?.Invoke(true);
        }
    }
}
