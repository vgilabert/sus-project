using System;
using UnityEngine;

namespace Items
{
    public class ConsumableFlow : MonoBehaviour
    {
        public virtual void StartFlow(Action<bool> callback)
        {
            callback?.Invoke(true);
        }
    }
}
