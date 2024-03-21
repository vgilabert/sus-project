using Dreamteck.Splines;
using Train.Upgrades;

namespace Train
{
    public class Engine : TrainPart
    {
        private SplineFollower _follower;
        
        private EngineUpgrade[] EngineUpgrades => (EngineUpgrade[]) Upgrades;
        
        public void Initialize(SplineComputer spline, EngineUpgrade[] upgrades)
        {
            Upgrades = upgrades;
            transform.position = spline.EvaluatePosition(0);
            _follower = GetComponent<SplineFollower>();
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
