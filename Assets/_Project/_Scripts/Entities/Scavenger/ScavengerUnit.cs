using Unity.Netcode;
using UnityEngine;

public class ScavengerUnit : NetworkBehaviour
{
    [SerializeField] private float _speed;

    private Rigidbody2D _rb;

    public override void OnNetworkSpawn()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + Vector2.right * _speed * Time.deltaTime);
    }
}
