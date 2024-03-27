using Character;
using Train;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Engine _engine;
    private Player _player;
    public Vector3 offset;
    
    void Start()
    {
        _engine = FindFirstObjectByType<Engine>();
        _player = FindFirstObjectByType<Player>();
    }
    
    void Update()
    {
        if (!_engine)
        {
            _engine = FindFirstObjectByType<Engine>();
        }
        else
        {
            if (!_player)
            {
                return;
            }
            // Find the middle between the player and the cart
            //var target = (_engine.transform.position + _player.transform.position) / 2;
            var target = _engine.transform.position;
            transform.position = target + offset;
            transform.LookAt(target);
        }
    }
}
