using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(IAttackMethod))]
public class Scavenger : Unit
{
    [Header("Ground Unit Stuff")]
    [SerializeField] private PathObject _path;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _spawnOffset; // how far from the center of the spawn point the unit spawns

    private Rigidbody2D _rb;
    private Vector2 _moveDirection;
    private Vector2 _offSet;
    private IAttackMethod _attackMethod;

    protected override void Awake()
    {
        base.Awake();

        _offSet = Random.insideUnitCircle * _spawnOffset;
        transform.position = _path.StartPosition + _offSet;
        _attackMethod = GetComponent<IAttackMethod>();
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
    }

    private void FixedUpdate()
    {
        if (!_isAttacking)
            _rb.MovePosition(_rb.position + _moveDirection * _moveSpeed * Time.deltaTime);
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
                    _attackMethod.ExecuteAttack(unit);
                    return;
                }
            }
        }
    }

    private void CalcMoveDirection()
    {
        _moveDirection = (_path.EndPosition + _offSet - (Vector2)transform.position).normalized;
    }
}
