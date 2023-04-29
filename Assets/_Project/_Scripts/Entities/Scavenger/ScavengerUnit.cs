using Unity.Netcode;
using UnityEngine;

public class ScavengerUnit : NetworkBehaviour
{
    [SerializeField] private PathObject _path;
    [SerializeField] private float _speed;

    private Vector2 _moveDirection;
    private Rigidbody2D _rb;

    private void Awake()
    {
        transform.position = _path.StartPosition;
    }

    public override void OnNetworkSpawn()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        CalcMoveDirection();
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _moveDirection * _speed * Time.deltaTime);
    }

    private void CalcMoveDirection()
    {
        _moveDirection = (_path.EndPosition - (Vector2)transform.position).normalized;
    }
}
