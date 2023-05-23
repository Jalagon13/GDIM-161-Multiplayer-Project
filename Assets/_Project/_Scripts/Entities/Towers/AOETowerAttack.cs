using System.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// For now TowerAttack is exactly the same functionality as the ScavengerAttack 
/// but this may change if we want to add different functionality for the tower or for
/// when we add different animations for the tower
/// </summary>

public class AOETowerAttack : MonoBehaviour, IAttackMethod
{
    [SerializeField] private float _blastRange;

    private Unit _ctx;
    private int _blastDamage;
    private string _tagToAttack;

    private void Awake()
    {
        _ctx = GetComponent<Unit>();
        _blastDamage = (int)(_ctx.AttackDamage * 0.75);
        if (_ctx.tag == "Blue")
            _tagToAttack = "Red";
        else
            _tagToAttack = "Blue";
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
        Vector3 targetPos = _ctx.UnitBeingAttacked.transform.position;

        var colliders = Physics2D.OverlapCircleAll(targetPos, _blastRange);
        Debug.Log(colliders);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out Unit unit))
            {
                if (unit != null && unit.CompareTag(_tagToAttack) &&
                        unit != _ctx.UnitBeingAttacked )//&& Vector3.Distance(targetPos, unit.transform.position) < _blastRange)
                    unit.DealDamage(_blastDamage);
            }
        }

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
