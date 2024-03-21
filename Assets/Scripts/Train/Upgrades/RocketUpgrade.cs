using System;
using UnityEngine;

namespace Train.Upgrades
{
    [CreateAssetMenu(menuName = "Upgrade/Rocket")]
    public class RocketUpgrades : ScriptableObject
    {
        public RocketUpgrade[] upgrades;
    }
    
    [Serializable]
    public class RocketUpgrade : TurretUpgrade
    {
        public float explosionRadius;
        public float flightDuration;
    }
}