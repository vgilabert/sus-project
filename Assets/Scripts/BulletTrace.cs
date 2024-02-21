using UnityEngine;

public class LineTrace : MonoBehaviour
{
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _progress;
    
    [SerializeField] private float _speed = 40f;

    private void Start()
    {
        _startPosition = transform.position;
    }
    
    private void Update()
    {
        _progress += Time.deltaTime * _speed;
        transform.position = Vector3.Lerp(_startPosition, _targetPosition, _progress);
    }
    
    public void SetTargetPosition(Vector3 position)
    {
        _targetPosition = position;
    }
}
