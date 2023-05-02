using UnityEngine;
using Unity.Netcode;

public class Tower : NetworkBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _range;
    [SerializeField] private LayerMask _unitLayer;

    private ContactFilter2D _unitFilter = new ContactFilter2D();
    private string _tagToAttack;
    private Unit _target = null;

    private void CheckForResourceNode()
    {
        
    }

    private void Awake()
    {
        _unitFilter.SetLayerMask(_unitLayer);

        // Might want to include this as a function in an interface IAttacker
        string RED_TEAM = "Red";
        string BLUE_TEAM = "Blue";
        _tagToAttack = gameObject.CompareTag(BLUE_TEAM) ? RED_TEAM : BLUE_TEAM;
    }

    private void FindTarget()
    {
        RaycastHit2D[] unitsFound = new RaycastHit2D[8];
        int amount = Physics2D.CircleCast(transform.position, _range, Vector2.right, _unitFilter, unitsFound, Mathf.Epsilon);

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

            if (!Vector3.Equals(_target.transform.position, target.position))
            {
                _target = target.gameObject.GetComponent<Unit>();
            }

            _target.DealDamage(_damage);
        }
    }

    private void Update()
    {
        FindTarget();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, _range);
    }
}
