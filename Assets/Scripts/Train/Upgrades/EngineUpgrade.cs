using System;
using UnityEngine;

namespace Train.Upgrades
{
    [CreateAssetMenu(menuName = "Upgrade/Engine")]
    public class EngineUpgrades : ScriptableObject
    {
        public EngineUpgrade[] upgrades;
    }
    
    [Serializable]
    public class EngineUpgrade : Upgrade
    {
        public float maxHealth;
        public int maxPower;
        public int maxWagonNumber;
        public int speed;
    }
}