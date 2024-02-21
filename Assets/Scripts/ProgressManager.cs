using Utils;

public class ProgressManager : MonoSingleton<ProgressManager>
{
    private double _progress = 0;
    public double GetProgress() => _progress;

    public void UpdateProgress(double progress)
    {
        _progress = progress;
    }

    private void FixedUpdate()
    {
        if (_progress >= 1)
        {
            GameManager.Instance.SetGamePause(true);
        }
    }
}
