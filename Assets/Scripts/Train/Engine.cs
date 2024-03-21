using Dreamteck.Splines;
using Train.Upgrades;

namespace Train
{
    public class Engine : TrainPart
    {
        private SplineFollower _follower;
        public void Initialize(SplineComputer spline, EngineUpgrade upgrade)
        {
            transform.position = spline.EvaluatePosition(0);
            _follower = GetComponent<SplineFollower>();
            _follower.spline = spline;
            _follower.followSpeed = upgrade.speed;
        }
    }
}
