using UnityEngine;

public class Tower : Unit
{
    private void CheckForResourceNode()
    {
        
    }

    private void FindTarget()
    {
        RaycastHit2D[] unitsFound = new RaycastHit2D[8];
        int amount = Physics2D.CircleCast(transform.position, _agroRange, Vector2.right, _unitFilter, unitsFound, Mathf.Epsilon);

        // If any units are found, see which enemy is the closest;
        // if this is the same target as last time, don't bother getting the Unit component
        if (amount > 0)
        {
            float closestDistance = float.MaxValue;
            int closestEnemy = 0;

            for (int i = 0; i < amount; i++)
            {
                RaycastHit2D unit = unitsFound[i];

                if (unit.transform.gameObject.CompareTag(_tagToAttack) && unit.distance < closestDistance)
                {
                    closestDistance = unit.distance;
                    closestEnemy = i;
                }
            }

            Transform target = unitsFound[closestEnemy].transform;

            if (!Vector3.Equals(_unitBeingAttacked.transform.position, target.position))
            {
                _unitBeingAttacked = target.gameObject.GetComponent<Unit>();
            }

            //_target.DealDamage(_atkDamage);
        }
    }

    private void Update()
    {
        FindTarget();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, _agroRange);
    }
}
