using UnityEngine;

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

    public void ExecuteAttack(Unit unit)
    {
        Debug.Log($"Tower ExecuteAttack callback on {unit.name}");
    }
}
