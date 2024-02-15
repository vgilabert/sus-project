using System;
using UnityEngine;

public class ItemFlow : MonoBehaviour
{
    public virtual void StartFlow(Action<bool> callback)
    {
        callback?.Invoke(true);
    }
}
