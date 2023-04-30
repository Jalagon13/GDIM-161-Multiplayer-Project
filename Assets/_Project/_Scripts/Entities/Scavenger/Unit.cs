using Unity.Netcode;
using UnityEngine;

public abstract class Unit : NetworkBehaviour
{
    [SerializeField] private protected PathObject _path;
    [SerializeField] private protected float _speed;
    [SerializeField] private protected float _atkSpeed; // time it takes to perform an attack
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
    private protected float _timeAttacked; // time since last attack

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
        if (!_isAttacking) // Keep moving if not attacking
            _rb.MovePosition(_rb.position + _moveDirection * _speed * Time.deltaTime);
        else if (_enemy == null) // Move on if there isn't an enemy
            ContinueMoving();
        else if (_atkSpeed <= _timeAttacked) // Attack once "recharged"
        {
            Attack();
            _timeAttacked = 0; // reset time since last attack
        }
        else
            _timeAttacked += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsAttacking || !collision.CompareTag(_tagToAttack)) return; // ignore if already attacking or is supposed to be ignored
        if (collision.TryGetComponent(out Unit unit))
        {
            _enemy = unit;
            //_collider.enabled = false;
            _isAttacking = true;
            _timeAttacked = _atkSpeed;
        }
    }

    private void CalcMoveDirection()
    {
        _moveDirection = (_path.EndPosition + _offSet - (Vector2)transform.position).normalized;
    }

    private void ContinueMoving()
    {
        _enemy = null;
        _isAttacking = false;
        _collider.enabled = true; // disable if we want units to gang up on others instead of 1v1 combat
        _timeAttacked = 0;
    }

    private void Attack()
    {
        if(_enemy == null) // Move on if there isn't an enemy
        {
            ContinueMoving();
            return;
        }
        //Debug.Log((gameObject.CompareTag(BLUE_TEAM) ? BLUE_TEAM : RED_TEAM) + " attacking " + _tagToAttack);
        _enemy._currentHP -= _atk;
        //Debug.Log((gameObject.CompareTag(BLUE_TEAM) ? BLUE_TEAM : RED_TEAM) + " HP:" + _currentHP);

        if (_enemy._currentHP <= 0)
        {
            // kill unit and let enemy keep moving
            Destroy(_enemy.gameObject);
            ContinueMoving();
        }
    }
}
