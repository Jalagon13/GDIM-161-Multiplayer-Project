using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ScavengerAttack : MonoBehaviour, IAttackMethod
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

    public void ExecuteAttack(Unit unit)
    {
        StartCoroutine(AttackTarget(unit));
    }

    private IEnumerator AttackTarget(Unit enemyUnit)
    {
        _ctx.UnitBeingAttacked = enemyUnit;
        _ctx.IsAttacking = true;

        yield return new WaitForSeconds(_ctx.AttackSpeed);

        _ctx.UnitBeingAttacked.DealDamage(_ctx.AttackDamage);

        if (_ctx.UnitBeingAttacked.CurrentHP > 0)
        {
            StartCoroutine(AttackTarget(enemyUnit));
        }
        else
        {
            _ctx.IsAttacking = false;
            _ctx.UnitBeingAttacked.GetComponent<NetworkObject>().Despawn(true);
            yield break;
        }
    }
}
