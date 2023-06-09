using UnityEngine;

[RequireComponent(typeof(IAttackMethod))]
public class AttackerUnit : Unit
{
    [Header("Ground Unit Stuff")]
    [SerializeField] private PathObject _path;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _spawnOffset; // how far from the center of the spawn point the unit spawns
    [SerializeField] private GameObject _unit;
    [SerializeField] private Animator _spriteAnimator;

    private Rigidbody2D _rb;
    private int _moveStep;
    private Vector2 _moveDirection;
    private Vector2 _offSet;
    private IAttackMethod _attackMethod;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _rb = GetComponent<Rigidbody2D>();
        _moveStep = 0;
        _offSet = Random.insideUnitCircle * _spawnOffset;
        transform.position = _path.StartPosition;
        _unit.transform.position = _path.StartPosition +_offSet;
        _attackMethod = GetComponent<IAttackMethod>();

        CalcMoveDirection();
    }

    private void FixedUpdate()
    {
        if (!_isAttacking)
        {
            _spriteAnimator.SetBool("isAttacking", false);
            FindTarget();
            if (((Vector2)transform.position - _path.Destination(_moveStep)).magnitude <= _spawnOffset)
            {
                _moveStep++;
                CalcMoveDirection();
            }

            _rb.MovePosition(_rb.position + _moveDirection * _moveSpeed * Time.deltaTime);
        }
    }

    private void FindTarget()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, _agroRange);

        foreach (Collider2D collider in colliders)
        {
            if(collider.TryGetComponent(out Unit unit))
            {
                float distanceBtwn = Vector3.Distance(transform.position, unit.transform.position);

                if (unit.CompareTag(_tagToAttack) && distanceBtwn < _agroRange)
                {
                    _unitBeingAttacked = unit;
                    _isAttacking = true;
                    _attackMethod.ExecuteAttack();
                    _spriteAnimator.SetBool("isAttacking", true);
                    return;
                }
            }
        }
    }

    private void CalcMoveDirection()
    {
        _moveDirection = (_path.Destination(_moveStep) + _offSet - (Vector2)transform.position).normalized;
    }
}
