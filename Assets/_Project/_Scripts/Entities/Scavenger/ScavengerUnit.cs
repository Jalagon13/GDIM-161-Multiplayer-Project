using Unity.Netcode;
using UnityEngine;

public class ScavengerUnit : NetworkBehaviour
{
    [SerializeField] private PathObject _path;
    [SerializeField] private float _speed;

    private static string RED_TEAM = "Red";
    private static string BLUE_TEAM = "Blue";
    private string _tagToAttack;
    private bool _isAttacking;
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private Vector2 _moveDirection;
    private Vector2 _offSet;

    public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }
    public float Speed { get { return _speed; } set { _speed = value; } }

    private void Awake()
    {
        _offSet = Random.insideUnitCircle * 0.75f;
        transform.position = _path.StartPosition + _offSet;

        _tagToAttack = gameObject.CompareTag(BLUE_TEAM) ? RED_TEAM : BLUE_TEAM;
    }

    public override void OnNetworkSpawn()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();


        CalcMoveDirection();
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _moveDirection * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(_tagToAttack)) return;
        if (collision.TryGetComponent(out ScavengerUnit scavengerUnit))
        {
            collision.enabled = false;
            scavengerUnit.IsAttacking = true;
            scavengerUnit.Speed = 0f;

            _collider.enabled = false;
            _isAttacking = true;
            _speed = 0f;
        }
    }

    private void CalcMoveDirection()
    {
        _moveDirection = ((_path.EndPosition + _offSet) - (Vector2)transform.position).normalized;
    }
}
