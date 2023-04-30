using Unity.Netcode;
using UnityEngine;

public abstract class Unit : NetworkBehaviour
{
    [SerializeField] private protected PathObject _path;
    [SerializeField] private protected float _speed;
    [SerializeField] private protected int _cost;
    [SerializeField] private protected int _maxHP;
    [SerializeField] private protected int _atk;

    private static protected string RED_TEAM = "Red";
    private static protected string BLUE_TEAM = "Blue";
    private protected string _tagToAttack;
    private protected bool _isAttacking;
    private protected int _currentHP;
    private protected Rigidbody2D _rb;
    private protected Collider2D _collider;
    private protected Vector2 _moveDirection;
    private protected Vector2 _offSet;
    private protected Unit _enemy; // unit being attacked

    public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }
    public float Speed { get { return _speed; } set { _speed = value; } }

    private void Awake()
    {
        _offSet = Random.insideUnitCircle * 0.75f;
        transform.position = _path.StartPosition + _offSet;

        _tagToAttack = gameObject.CompareTag(BLUE_TEAM) ? RED_TEAM : BLUE_TEAM;
        _currentHP = _maxHP;
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
        if (IsAttacking || !collision.CompareTag(_tagToAttack)) return; // ignore if already attacking or is supposed to be ignored
        if (collision.TryGetComponent(out Unit unit))
        {
            collision.enabled = false;
            unit.IsAttacking = true;
            unit.Speed = 0f;

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
