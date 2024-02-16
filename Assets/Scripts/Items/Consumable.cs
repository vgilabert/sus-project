using System;
using UnityEngine;

namespace Items
{
    
    public abstract class IConsumable
    {
        public string name;
        public  int amount;

        protected ItemFlow itemFlow;
        
        public static Action<IConsumable> OnConsumed;

        public IConsumable(int amount = 1)
        {
            this.amount = amount;
        }
        
        public void Activate()
        {
            Init();
            if (itemFlow != null)
            {
                Debug.Log("Starting flow for " + name);
                itemFlow.StartFlow(_success => OnConsumed?.Invoke(this));
            }
            else
            {
                Debug.Log("Consuming " + name);
                OnConsumed?.Invoke(this);
            }
            
        }

        protected virtual void Init(){}
    }
}