using Unity.Netcode;
using UnityEngine;

public abstract class Unit : NetworkBehaviour
{
    [Header("Unit base parameters")]
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] protected int _maxHP;
    [SerializeField] protected int _atkDamage;
    [SerializeField] protected float _atkSpeed; // time it takes to perform an attack
    [SerializeField] protected float _agroRange;
    [SerializeField] protected int _cost;

    protected static string RED_TEAM = "Red";
    protected static string BLUE_TEAM = "Blue";
    protected ContactFilter2D _unitFilter = new();
    protected LayerMask _unitLayer;
    protected string _tagToAttack;
    protected bool _isAttacking;
    protected int _currentHP;
    protected Unit _unitBeingAttacked;

    public Unit UnitBeingAttacked { get { return _unitBeingAttacked; } set { _unitBeingAttacked = value; } }
    public float AttackSpeed { get { return _atkSpeed; } }
    public int AttackDamage { get { return _atkDamage; } }
    public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }
    public int CurrentHP { get { return _currentHP; } }

    protected virtual void Awake()
    {
        _unitLayer.value = 6; // six is the "Unit" Layer
        _unitFilter.SetLayerMask(_unitLayer);
        _tagToAttack = gameObject.CompareTag(BLUE_TEAM) ? RED_TEAM : BLUE_TEAM;
        _currentHP = _maxHP;

        UpdateHP();
    }

    public void DealDamage(int damage)
    {
        _currentHP -= damage;

        UpdateHP();
    }

    private void UpdateHP()
    {
        if (_healthBar != null)
            _healthBar.UpdateFill(_currentHP, _maxHP);
    }
}
