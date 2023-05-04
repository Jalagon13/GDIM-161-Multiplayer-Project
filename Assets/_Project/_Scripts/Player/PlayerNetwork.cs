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
    [SerializeField] private int _passiveScrapRate = 10;

    private DefaultControls _defaultControls;
    private TextMeshProUGUI _scrapBankText;
    private TextMeshProUGUI _scrapRateText;

    private void Awake()
    {
        // if (!isOwner) Destroy(this);

        _defaultControls = new DefaultControls();
    }

    private void OnEnable()
    {
        _defaultControls.Enable();
    }

    private void OnDisable()
    {
        _defaultControls.Disable();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _defaultControls.Debug.SpawnUnit.started += SpawnUnit;
        _defaultControls.Debug.SpawnOpponentUnit.started += SpawnOpponentUnit;
        _defaultControls.Player.Click.started += OnClick;

        if (OwnerClientId == 0)
            InitializeAsBlueTeam();
        else
            InitializeAsRedTeam();

        UpdateUI();
    }

    public void SpawnScavenger()
    {

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

    private void SpawnUnit(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        SpawnUnitServerRpc(new ServerRpcParams { Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId} });
    }

    private void SpawnOpponentUnit(InputAction.CallbackContext context)
    {
        // temp method for spawning opponent without another game instance, for testing only
        if (!IsOwner) return;

        SpawnOpponentUnitServerRpc(new ServerRpcParams { Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId } });
    }

    [ServerRpc]
    private void SpawnUnitServerRpc(ServerRpcParams serverRpcParams)
    {
        //Debug.Log($"SpawnUnitServerRpc Callback - SenderClientId: {serverRpcParams.Receive.SenderClientId}");
        Instantiate(serverRpcParams.Receive.SenderClientId == 0 ? _scavengerBlueUnit : _scavengerRedUnit).GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc]
    private void SpawnOpponentUnitServerRpc(ServerRpcParams serverRpcParams)
    {
        // temp method for spawning opponent without another game instance, for testing only
        //Debug.Log($"SpawnUnitServerRpc Callback - SenderClientId: {serverRpcParams.Receive.SenderClientId}");
        Instantiate(serverRpcParams.Receive.SenderClientId == 1 ? _scavengerBlueUnit : _scavengerRedUnit).GetComponent<NetworkObject>().Spawn(true);
    }

    // Possibly do this by making the tower nodes buttons instead and having them fire an event in some tower node manager to all players
    private void OnClick(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        Vector2 position = context.ReadValue<Vector2>();
        Ray rayToPoint = Camera.main.ScreenPointToRay(position);

        // Needs a way to check if the node is already taken, probably through a tower node interface
        if (Physics.Raycast(rayToPoint, Mathf.Infinity, _towerNodeLayer))
        {
            SpawnTowerServerRpc(new ServerRpcParams { Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId } });
        }
    }

    [ServerRpc]
    private void SpawnTowerServerRpc(ServerRpcParams serverRpcParams)//, TowerNode node)
    {
        Instantiate(serverRpcParams.Receive.SenderClientId == 0 ? _blueTower : _redTower, transform.position,
            Quaternion.identity).GetComponent<NetworkObject>().Spawn(true);

    }
}
