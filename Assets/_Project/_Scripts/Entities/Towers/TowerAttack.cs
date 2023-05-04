using System.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// For now TowerAttack is exactly the same functionality as the ScavengerAttack 
/// but this may change if we want to add different functionality for the tower or for
/// when we add different animations for the tower
/// </summary>

public class TowerAttack : MonoBehaviour, IAttackMethod
{
    private Unit _ctx;

    private void Awake()
    {
        _ctx = GetComponent<Unit>();
    }

    private void Update()
    {
        if (_ctx.UnitBeingAttacked == null)
        {
            _ctx.IsAttacking = false;
            StopAllCoroutines();
        }
    }

    public void ExecuteAttack()
    {
        StartCoroutine(AttackTarget());
    }

    private IEnumerator AttackTarget()
    {
        //_ctx.UnitBeingAttacked = enemyUnit;
        //_ctx.IsAttacking = true;

        yield return new WaitForSeconds(_ctx.AttackSpeed);

        _ctx.UnitBeingAttacked.DealDamage(_ctx.AttackDamage);

        if (_ctx.UnitBeingAttacked.CurrentHP > 0)
        {
            StartCoroutine(AttackTarget());
        }
        else
        {
            _ctx.IsAttacking = false;
            StartCoroutine(_ctx.UnitBeingAttacked.Die());
            yield break;
        }
    }
}
