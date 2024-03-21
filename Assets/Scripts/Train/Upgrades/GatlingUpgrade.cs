using System;
using UnityEngine;

namespace Train.Upgrades
{
    [CreateAssetMenu(menuName = "Upgrade/Gatling")]
    public class GatlingUpgrades : ScriptableObject
    {
        public GatlingUpgrade[] upgrades;
    }
    
    [Serializable]
    public class GatlingUpgrade : TurretUpgrade
    {
        public bool piercingAmmo;
    }
}