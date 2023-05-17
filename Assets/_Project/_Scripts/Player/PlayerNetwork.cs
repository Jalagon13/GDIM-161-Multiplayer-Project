using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private RectTransform _bluePanel;
    [SerializeField] private RectTransform _redPanel;
    [SerializeField] private UnitSpawner _scavengerBlueUnit;
    [SerializeField] private UnitSpawner _scavengerRedUnit;
    [SerializeField] private GameObject _blueTower;
    [SerializeField] private GameObject _redTower;
    [SerializeField] private LayerMask _towerNodeLayer;
    [SerializeField] private int _currentScrapBank = 250;
    [SerializeField] private int _passiveScrapRate = 10; // Rate for scraps
    [SerializeField] private int _resourceNodeScrapIncrease = 10; // How much the scrap rate increases when a resource node is obtained
    [SerializeField] private float _secondsPerRate = 3; // increments SP by rate every _secondsPerRate

    private DefaultControls _defaultControls;
    private TextMeshProUGUI _scrapBankText;
    private TextMeshProUGUI _scrapRateText;
    private GameObject _scavengerSpawner;
    private TowerNode _selectedNode;
    private string _selectedUnit2Spawn;

    private void Awake()
    {
        // if (!isOwner) Destroy(this);

        _defaultControls = new DefaultControls();
    }

    private void OnEnable()
    {
        _defaultControls.Enable();
        StartCoroutine(ScavengeScrapPoints());

        ResourceNode.OnResourceNodeObtained += OnResourceNodeObtainedEventHandler;
        ResourceNode.OnResourceNodeLost += OnResourceNodeLostEventHandler;
    }

    private void OnResourceNodeObtainedEventHandler(bool isRed)
    {
        if ((isRed && OwnerClientId == 1) || (!isRed && OwnerClientId == 0))
        {
            _passiveScrapRate += _resourceNodeScrapIncrease;
        }
    }

    private void OnResourceNodeLostEventHandler(bool isRed)
    {
        if ((isRed && OwnerClientId == 1) || (!isRed && OwnerClientId == 0))
        {
            _passiveScrapRate -= _resourceNodeScrapIncrease;
        }
    }

    private void OnDisable()
    {
        _defaultControls.Disable();
        StopAllCoroutines();

        ResourceNode.OnResourceNodeObtained -= OnResourceNodeObtainedEventHandler;
        ResourceNode.OnResourceNodeLost -= OnResourceNodeLostEventHandler;
    }

    private IEnumerator ScavengeScrapPoints()
    {
        yield return new WaitForSeconds(_secondsPerRate);
        _currentScrapBank += _passiveScrapRate;
        UpdateUI();
        StartCoroutine(ScavengeScrapPoints());
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        //_defaultControls.Debug.SpawnUnit.started += SpawnUnit;
        //_defaultControls.Debug.SpawnOpponentUnit.started += SpawnOpponentUnit;
        _defaultControls.Player.Click.started += OnClick;

        if (OwnerClientId == 0)
            InitializeAsBlueTeam();
        else
            InitializeAsRedTeam();

        UpdateUI();
    }

    public void ToggleSpawner(string unit)
    {
        bool active = _scavengerSpawner.activeInHierarchy;
        if (!active || (active && unit == _selectedUnit2Spawn)) // toggle if not active or not switching unit selected
            _scavengerSpawner.SetActive(!active);
        _selectedUnit2Spawn = unit;
    }

    public void SpawnUnit(string path) // hooked up to UI buttons
    {
        if (!IsOwner || path.Length > 1) return;

        if(_currentScrapBank >= 25)
        {
            SpawnUnitServerRpc(_selectedUnit2Spawn, path[0], new ServerRpcParams { Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId } });
            _currentScrapBank -= 25;
            UpdateUI();
        }
    }

    private void InitializeAsBlueTeam()
    {
        _bluePanel.gameObject.SetActive(true);
        _redPanel.gameObject.SetActive(false);
        _scrapBankText = _bluePanel.GetChild(1).GetComponent<TextMeshProUGUI>();
        _scrapRateText = _bluePanel.GetChild(2).GetComponent<TextMeshProUGUI>();
        _scavengerSpawner = _bluePanel.GetChild(3).gameObject;
    }

    private void InitializeAsRedTeam()
    {
        _redPanel.gameObject.SetActive(true);
        _bluePanel.gameObject.SetActive(false);
        _scrapBankText = _redPanel.GetChild(1).GetComponent<TextMeshProUGUI>();
        _scrapRateText = _redPanel.GetChild(2).GetComponent<TextMeshProUGUI>();
        _scavengerSpawner = _redPanel.GetChild(3).gameObject;
    }

    private void UpdateUI()
    {
        _scrapBankText.text = $"Scrap Bank: {_currentScrapBank}";
        _scrapRateText.text = $"Scrap Rate: {_passiveScrapRate}";
    }

    [ServerRpc]
    private void SpawnUnitServerRpc(string unit, char path, ServerRpcParams serverRpcParams)
    {
        //Debug.Log($"SpawnUnitServerRpc Callback - SenderClientId: {serverRpcParams.Receive.SenderClientId}");
        Instantiate(serverRpcParams.Receive.SenderClientId == 0 ? _scavengerBlueUnit.SpawnUnit(unit, path) : _scavengerRedUnit.SpawnUnit(unit, path)).GetComponent<NetworkObject>().Spawn(true);
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        Vector2 position = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
        Collider2D towerNodeHit = Physics2D.OverlapPoint(position, _towerNodeLayer);

        if (towerNodeHit != null)
        {
            TowerNode _selectedNode = towerNodeHit.transform.GetComponent<TowerNode>();

            if (!_selectedNode.IsOccupied())
            {
                SpawnTowerServerRpc(new ServerRpcParams { Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId } });
            }
        }
    }

    [ServerRpc]
    private void SpawnTowerServerRpc(ServerRpcParams serverRpcParams)
    {
        Instantiate(serverRpcParams.Receive.SenderClientId == 0 ? _blueTower : _redTower, _selectedNode.transform.position,
            Quaternion.identity).GetComponent<NetworkObject>().Spawn(true);
    }
}
