using System;

namespace Train.Upgrades
{
    [Serializable]
    public class TurretUpgrade : Upgrade
    {
        public int Damage;

        public float TimeBetweenShots;
        public float MaxDistance;
        public float MinDistance;
    }
}
