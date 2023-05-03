using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Scavenger : Unit
{
    [SerializeField] private PathObject _path;
    [SerializeField] private float _speed;

    private Unit _unitBeingAttacked;
    private Rigidbody2D _rb;
    private Vector2 _moveDirection;
    private Vector2 _offSet;

    protected override void Awake()
    {
        base.Awake();

        _offSet = Random.insideUnitCircle * 0.75f;
        transform.position = _path.StartPosition + _offSet;
    }

    public override void OnNetworkSpawn()
    {
        _rb = GetComponent<Rigidbody2D>();

        CalcMoveDirection();
    }

    private void Update()
    {
        if (!_isAttacking)
            FindTarget();

        if (_unitBeingAttacked == null)
        {
            _isAttacking = false;
            StopAllCoroutines();
        }
    }

    private void FindTarget()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, _agroRange);

        foreach (Collider2D collider in colliders)
        {
            if(collider.TryGetComponent(out Unit unit))
            {
                if (unit.CompareTag(_tagToAttack) && Vector3.Distance(transform.position , unit.transform.position) < _agroRange)
                {
                    StartCoroutine(AttackTarget(unit));
                    return;
                }
            }
        }
    }

    private IEnumerator AttackTarget(Unit enemyUnit)
    {
        _unitBeingAttacked = enemyUnit;
        _isAttacking = true;

        yield return new WaitForSeconds(_atkSpeed);

        _unitBeingAttacked.DealDamage(_atkDamage);

        if (_unitBeingAttacked.CurrentHP > 0)
        {
            StartCoroutine(AttackTarget(_unitBeingAttacked));
        }
        else
        {
            _isAttacking = false;
            _unitBeingAttacked.GetComponent<NetworkObject>().Despawn(true);
            yield break;
        }
    }

    private void FixedUpdate()
    {
        if (!_isAttacking)
            _rb.MovePosition(_rb.position + _moveDirection * _speed * Time.deltaTime);
    }

    private void CalcMoveDirection()
    {
        _moveDirection = (_path.EndPosition + _offSet - (Vector2)transform.position).normalized;
    }
}
