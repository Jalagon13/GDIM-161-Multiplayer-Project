using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class Unit : NetworkBehaviour
{
    [Header("Unit base parameters")]
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private Animator _animator;
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
    protected NetworkVariable<int> _currentHP = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    protected Unit _unitBeingAttacked;

    public int Cost { get { return _cost; } }
    public Unit UnitBeingAttacked { get { return _unitBeingAttacked; } set { _unitBeingAttacked = value; } }
    public float AttackSpeed { get { return _atkSpeed; } }
    public int AttackDamage { get { return _atkDamage; } }
    public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }
    public NetworkVariable<int> CurrentHP { get { return _currentHP; } }
    public Animator Animator { get { return _animator; } }

    protected virtual void Awake()
    {
        _unitLayer.value = 6; // six is the "Unit" Layer
        _unitFilter.SetLayerMask(_unitLayer);
        _tagToAttack = gameObject.CompareTag(BLUE_TEAM) ? RED_TEAM : BLUE_TEAM;
    }

    public override void OnNetworkSpawn()
    {
        _currentHP.OnValueChanged += (int perviousInt, int newInt) =>
        {
            UpdateHP();
        };

        _currentHP.Value = _maxHP;
    }

    public void DealDamage(int damage)
    {
        _currentHP.Value -= damage;
        _animator.SetTrigger("tookDamage");
        UpdateHP();
    }

    private void UpdateHP()
    {
        if (_healthBar != null)
        {
            // toggle healthbar to show if damage has been taken
            if (_currentHP.Value < _maxHP)//!_healthBar.gameObject.activeInHierarchy && 
                _healthBar.gameObject.SetActive(true);
            else
                _healthBar.gameObject.SetActive(false);

            _healthBar.UpdateFill(_currentHP.Value, _maxHP);
        }
    }
}
