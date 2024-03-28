using Dreamteck.Splines;
using Train.Upgrades;

namespace Train
{
    public class Engine : TrainPart
    {
        private EngineUpgrade CurrentEngineUpgrade => Upgrades[Level] as EngineUpgrade;

        private SplineFollower _follower;
        public SplineFollower Follower => _follower;

        private void Awake()
        {
            _follower = GetComponent<SplineFollower>();
        }

        public void Initialize(EngineUpgrade[] engineUpgrade)
        {
            Upgrades = engineUpgrade as EngineUpgrade[];
            Level = 0;
            ApplyUpgrade();
        }

        protected override void ApplyUpgrade()
        {
            base.ApplyUpgrade();
            
            _follower.followSpeed = CurrentEngineUpgrade.speed;
        }
    }
}
