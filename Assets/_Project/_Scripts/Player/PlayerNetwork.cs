using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private RectTransform _bluePanel;
    [SerializeField] private RectTransform _redPanel;
    [SerializeField] private GameObject _scavengerBlueUnit;
    [SerializeField] private GameObject _scavengerRedUnit;
    [SerializeField] private GameObject _blueTower;
    [SerializeField] private GameObject _redTower;
    [SerializeField] private LayerMask _towerNodeLayer;
    [SerializeField] private int _currentScrapBank = 250;
    [SerializeField] private int _passiveScrapRate = 10; // Rate for scraps
    [SerializeField] private int _secondsPerRate = 1; // increments SP by rate every _secondsPerRate

    private DefaultControls _defaultControls;
    private TextMeshProUGUI _scrapBankText;
    private TextMeshProUGUI _scrapRateText;
    private TowerNode _selectedNode;

    private void Awake()
    {
        // if (!isOwner) Destroy(this);

        _defaultControls = new DefaultControls();
    }

    private void OnEnable()
    {
        _defaultControls.Enable();
        StartCoroutine(ScavengeScrapPoints());
    }

    private void OnDisable()
    {
        _defaultControls.Disable();
        StopAllCoroutines();
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

    public void SpawnScavenger() // hooked up to UI buttons
    {
        if (!IsOwner) return;

        if(_currentScrapBank >= 25)
        {
            SpawnUnitServerRpc(new ServerRpcParams { Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId } });
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
    }

    private void InitializeAsRedTeam()
    {
        _redPanel.gameObject.SetActive(true);
        _bluePanel.gameObject.SetActive(false);
        _scrapBankText = _redPanel.GetChild(1).GetComponent<TextMeshProUGUI>();
        _scrapRateText = _redPanel.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    private void UpdateUI()
    {
        _scrapBankText.text = $"Scrap Bank: {_currentScrapBank}";
        _scrapRateText.text = $"Scrap Rate: {_passiveScrapRate}";
    }

    //private void SpawnUnit(InputAction.CallbackContext context)
    //{
    //    if (!IsOwner) return;

    //    SpawnUnitServerRpc(new ServerRpcParams { Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId} });
    //}

    //private void SpawnOpponentUnit(InputAction.CallbackContext context)
    //{
    //    // temp method for spawning opponent without another game instance, for testing only
    //    if (!IsOwner) return;

    //    SpawnOpponentUnitServerRpc(new ServerRpcParams { Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId } });
    //}

    [ServerRpc]
    private void SpawnUnitServerRpc(ServerRpcParams serverRpcParams)
    {
        //Debug.Log($"SpawnUnitServerRpc Callback - SenderClientId: {serverRpcParams.Receive.SenderClientId}");
        Instantiate(serverRpcParams.Receive.SenderClientId == 0 ? _scavengerBlueUnit : _scavengerRedUnit).GetComponent<NetworkObject>().Spawn(true);
    }

    //[ServerRpc]
    //private void SpawnOpponentUnitServerRpc(ServerRpcParams serverRpcParams)
    //{
    //    // temp method for spawning opponent without another game instance, for testing only
    //    //Debug.Log($"SpawnUnitServerRpc Callback - SenderClientId: {serverRpcParams.Receive.SenderClientId}");
    //    Instantiate(serverRpcParams.Receive.SenderClientId == 1 ? _scavengerBlueUnit : _scavengerRedUnit).GetComponent<NetworkObject>().Spawn(true);
    //}

    private void OnClick(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        print("eh");

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
