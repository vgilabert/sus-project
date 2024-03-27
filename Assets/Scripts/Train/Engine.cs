using Dreamteck.Splines;
using Train.Upgrades;

namespace Train
{
    public class Engine : TrainPart
    {
        private SplineFollower _follower;
        public SplineFollower Follower => _follower;
        
        private EngineUpgrade[] EngineUpgrades => (EngineUpgrade[]) Upgrades;

        protected void Awake()
        {
            _follower = GetComponent<SplineFollower>();
        }

        public void Initialize(SplineComputer spline, EngineUpgrade[] upgrades)
        {
            Upgrades = upgrades;
            _follower.spline = spline;
            ApplyUpgrade();
        }
        
        protected override void ApplyUpgrade()
        {
            base.ApplyUpgrade();
            _follower.followSpeed = EngineUpgrades[Level].speed;
        }
    }
}
