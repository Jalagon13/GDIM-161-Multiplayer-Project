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
        if (!_isAttacking || Vector3.Distance(transform.position, _unitBeingAttacked.transform.position) > _agroRange)
            FindTarget();
    }

    public bool IsRed()
    {
        return _isRed;
    }

    private void FindTarget()
    {
        float closest2Base = 100f;
        if(_isRed)
            closest2Base *= -1;
        Unit unit2Attack = default;
        var colliders = Physics2D.OverlapCircleAll(transform.position, _agroRange);

        // loop through all Units in colliders to find the closetUnit
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out Unit unit))
            {
                //if (unit is Tower && unit != this) return;

                if (unit.CompareTag(_tagToAttack) && Vector3.Distance(transform.position, unit.transform.position) <= _agroRange)
                {
                    // isRed looking for biggest x, isblue looking for smallest x
                    if ((_isRed && unit.transform.position.x > closest2Base) || (!_isRed && unit.transform.position.x < closest2Base))
                    {
                        closest2Base = unit.transform.position.x;
                        unit2Attack = unit;
                    }
                }
            }
        }

        if(unit2Attack != default)
        {
            _unitBeingAttacked = unit2Attack;
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
