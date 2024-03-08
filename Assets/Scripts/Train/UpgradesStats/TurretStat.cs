using UnityEngine;

namespace Train.UpgradesStats
{
    [CreateAssetMenu(fileName = "TurretStat", menuName = "Train/Upgrades/TurretStat")]
    public class TurretStat : ScriptableObject
    {
        public int scrapCost;
        public int powerCost;

        public int damage;
        public int magazineSize;

        public float timeBetweenShots;
        public bool piercingAmmo;
        public float maxDistance;
        public float minDistance;
        
        public float explosionRadius;
        public float flightDuration;
    }
}
