using UnityEngine;

namespace Train.UpgradesStats
{
    [CreateAssetMenu(fileName = "TurretStat", menuName = "Train/Upgrades/TurretStat")]
    public class TurretStat : ScriptableObject
    {
        public int cost;
        public int powerCost;

        public int damage;
        public float timeBetweenShots;
        public bool piercingAmmo;
        public float maxDistance;
        public float minDistance;
        
        public float explosionRadius;
        public float flightDuration;
    }
}
