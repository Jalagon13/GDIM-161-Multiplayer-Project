using UnityEngine;
using System;

[RequireComponent(typeof(IAttackMethod))]
public class Tower : Unit
{
    // bool indicates if the tower is from the red team or not
    public event Action<bool> OnDestroyed;

    private IAttackMethod _attackMethod;
    private bool _isRed;

    protected override void Awake()
    {
        base.Awake();
        _attackMethod = GetComponent<IAttackMethod>();

        string redTeam = "Red";
        _isRed = gameObject.CompareTag(redTeam);
    }

    private void Update()
    {
        if (!_isAttacking)
            FindTarget();
    }

    public bool IsRed()
    {
        return _isRed;
    }

    private void FindTarget()
    {
        float distanceCounter = 100f;
        Unit closestUnit = default;
        var colliders = Physics2D.OverlapCircleAll(transform.position, _agroRange);

        // loop through all Units in colliders to find the closetUnit
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out Unit unit))
            {
                float distanceBtwn = Vector3.Distance(transform.position, unit.transform.position);

                if (unit.CompareTag(_tagToAttack) && distanceBtwn < _agroRange)
                {
                    if (distanceBtwn < distanceCounter)
                    {
                        distanceCounter = distanceBtwn;
                        closestUnit = unit;
                    }
                }
            }
        }

        if(closestUnit != default)
        {
            _unitBeingAttacked = closestUnit;
            _isAttacking = true;
            _attackMethod.ExecuteAttack();
        }
    }

    private void CheckForResourceNode()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, _agroRange);
    }

    private void OnDisable()
    {
        OnDestroyed?.Invoke(_isRed);
    }
}
