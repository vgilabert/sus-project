using UnityEngine;

namespace Train.UpgradesStats
{
    public class TurretStat : ScriptableObject
    {
        public int cost;
        public int damage;
        public float timeBetweenShots;
        public float explosionRadius;
        public bool piercingAmmo;
        public float maxDistance;
        public float minDistance;
    }
}
