using System;
using System.Collections;

namespace Items
{
    public class RepairKitFlow : ConsumableFlow
    {
        public static Action<float, Action<bool>> OnRepairKitUsed;
        
        private const float RepairPercentage = 0.1f;
        
        public override void StartFlow(Action<bool> callback)
        {
            StartCoroutine(Flow(callback));
        }

        IEnumerator Flow(Action<bool> callback)
        {
            OnRepairKitUsed?.Invoke(RepairPercentage, callback);
            yield return null;
        }
    }
}
