using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class UnitAttack : MonoBehaviour, IAttackMethod
{
    [SerializeField] private AudioClip _attackSound;

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
        AudioManager.Instance.PlayClip(_attackSound, false, false);

        _ctx.UnitBeingAttacked.DealDamage(_ctx.AttackDamage);

        if (_ctx.UnitBeingAttacked.CurrentHP.Value > 0)
        {
            yield return new WaitForSeconds(_ctx.AttackSpeed);
            StartCoroutine(AttackTarget());
        }
        else
        {
            if(_ctx.UnitBeingAttacked != null)
                StartCoroutine(Kill());
            yield break;
        }
    }

    public IEnumerator Kill()
    {
        _ctx.UnitBeingAttacked.Animator.SetTrigger("dies");
        yield return new WaitForSeconds(0.8f);

        _ctx.UnitBeingAttacked.GetComponent<NetworkObject>().Despawn(true);
        _ctx.IsAttacking = false;
    }
}
