using System;
using System.Collections;
using UnityEngine;

namespace Items
{
    public class RepairKitFlow : ItemFlow
    {
        public static Action<float> OnRepairKitUsed;
        
        private float _repairAmount = 50f;
        
        public override void StartFlow(Action<bool> callback)
        {
            StartCoroutine(Flow(callback));
        }

        IEnumerator Flow(Action<bool> callback)
        {
            OnRepairKitUsed?.Invoke(_repairAmount);
            Debug.Log("Repair kit used !");
            callback?.Invoke(true);
            yield break;
        }
    }
}
