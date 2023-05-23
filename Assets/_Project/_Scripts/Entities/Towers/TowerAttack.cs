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
        if (_ctx == null)
        {
            _ctx.IsAttacking = false;
            yield break;
        }
        _ctx.UnitBeingAttacked.DealDamage(_ctx.AttackDamage);

        if (_ctx.UnitBeingAttacked.CurrentHP.Value > 0)
        {
            yield return new WaitForSeconds(_ctx.AttackSpeed);
            StartCoroutine(AttackTarget());
        }
        else
        {
            if (_ctx.UnitBeingAttacked != null)
                StartCoroutine(Kill());
            yield break;
        }
    }

    public IEnumerator Kill()
    {
        if (_ctx.UnitBeingAttacked != null)
        {
            _ctx.UnitBeingAttacked.Animator.SetTrigger("dies");
            yield return new WaitForSeconds(0.8f);

            _ctx.UnitBeingAttacked.GetComponent<NetworkObject>().Despawn(true);
        }
        _ctx.IsAttacking = false;
    }
}
