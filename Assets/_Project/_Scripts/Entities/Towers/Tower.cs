using UnityEngine;

public class Tower : Unit
{
    private IAttackMethod _attackMethod;

    protected override void Awake()
    {
        base.Awake();
        _attackMethod = GetComponent<IAttackMethod>();
    }

    private void Update()
    {
        //if (!_isAttacking)
        //    FindTarget();
    }

    private void FindTarget()
    {
        float distanceCounter = 100f;
        Unit closetUnit = default;
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
                        closetUnit = unit;
                    }
                }
            }
        }

        if(closetUnit != default)
        {
            _attackMethod.ExecuteAttack(closetUnit);
        }

        //RaycastHit2D[] unitsFound = new RaycastHit2D[8];
        //int amount = Physics2D.CircleCast(transform.position, _agroRange, Vector2.right, _unitFilter, unitsFound, Mathf.Epsilon);


        // If any units are found, see which enemy is the closest;
        // if this is the same target as last time, don't bother getting the Unit component
        //if (amount > 0)
        //{
        //    float closestDistance = float.MaxValue;
        //    int closestEnemy = 0;

        //    for (int i = 0; i < amount; i++)
        //    {
        //        RaycastHit2D unit = unitsFound[i];

        //        if (unit.transform.gameObject.CompareTag(_tagToAttack) && unit.distance < closestDistance)
        //        {
        //            closestDistance = unit.distance;
        //            closestEnemy = i;
        //        }
        //    }

        //    Transform target = unitsFound[closestEnemy].transform;

        //    if (!Vector3.Equals(_unitBeingAttacked.transform.position, target.position))
        //    {
        //        _unitBeingAttacked = target.gameObject.GetComponent<Unit>();
        //    }

            //_target.DealDamage(_atkDamage);
        //}
    }
    private void CheckForResourceNode()
    {

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, _agroRange);
    }
}
