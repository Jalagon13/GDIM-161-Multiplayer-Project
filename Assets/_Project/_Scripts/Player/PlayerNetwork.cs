using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private RectTransform _bluePanel;
    [SerializeField] private RectTransform _redPanel;
    [SerializeField] private RectTransform _spectatorPanel;
    [SerializeField] private UnitSpawner _scavengerBlueUnit;
    [SerializeField] private UnitSpawner _scavengerRedUnit;
    [SerializeField] private LayerMask _towerNodeLayer;
    [SerializeField] private int _currentScrapBank = 250;
    [SerializeField] private int _passiveScrapRate = 10; // Rate for scraps
    [SerializeField] private int _resourceNodeScrapIncrease = 10; // How much the scrap rate increases when a resource node is obtained
    [SerializeField] private float _secondsPerRate = 3; // increments SP by rate every _secondsPerRate
    [SerializeField] private int _currentSeconds = 300;

    private DefaultControls _defaultControls;
    private TextMeshProUGUI _scrapBankText;
    private TextMeshProUGUI _scrapRateText;
    private TextMeshProUGUI _countDownText;
    private GameObject _scavengerSpawner;
    private RectTransform _playerPanel;
    private string _selectedUnit2Spawn;
    private bool _gameStarted = false;
    

    private void Awake()
    {
        // if (!isOwner) Destroy(this);

        _defaultControls = new DefaultControls();
    }

    private void FixedUpdate()
    {
        if (!_gameStarted)
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length >= 2)
            {
                _gameStarted = true;
                if (OwnerClientId >= 2)
                    _spectatorPanel.gameObject.SetActive(true);
                else
                {
                    _playerPanel.gameObject.SetActive(true);
                    StartCoroutine(ScavengeScrapPoints());
                    StartCoroutine(CountDownTimer());
                    UpdateUI();
                }
            }
        }
    }

        private void OnEnable()
    {
        _defaultControls.Enable();
        

        ResourceNode.OnResourceNodeObtained += OnResourceNodeObtainedEventHandler;
        ResourceNode.OnResourceNodeLost += OnResourceNodeLostEventHandler;
        TowerNode.NodeClickEvent += TrySpawnTower;

    }

    private void OnDisable()
    {
        _defaultControls.Disable();
        StopAllCoroutines();

        ResourceNode.OnResourceNodeObtained -= OnResourceNodeObtainedEventHandler;
        ResourceNode.OnResourceNodeLost -= OnResourceNodeLostEventHandler;
        TowerNode.NodeClickEvent -= TrySpawnTower;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _defaultControls = new DefaultControls();
        //_defaultControls.Player.Click.started += OnClick;

        if (OwnerClientId == 0)
            InitializeAsBlueTeam();
        else if (OwnerClientId == 1)
            InitializeAsRedTeam();
        else
            _playerPanel = _spectatorPanel;
    }

    private void InitializeAsBlueTeam()
    {
        _scrapBankText = _bluePanel.GetChild(1).GetComponent<TextMeshProUGUI>();
        _scrapRateText = _bluePanel.GetChild(2).GetComponent<TextMeshProUGUI>();
        _scavengerSpawner = _bluePanel.GetChild(3).gameObject;
        _countDownText = _bluePanel.GetChild(4).GetComponent<TextMeshProUGUI>();
        _playerPanel = _bluePanel;
    }

    private void InitializeAsRedTeam()
    {
        _scrapBankText = _redPanel.GetChild(1).GetComponent<TextMeshProUGUI>();
        _scrapRateText = _redPanel.GetChild(2).GetComponent<TextMeshProUGUI>();
        _scavengerSpawner = _redPanel.GetChild(3).gameObject;
        _countDownText = _redPanel.GetChild(4).GetComponent<TextMeshProUGUI>();
        _playerPanel = _redPanel;
    }

        private void TrySpawnTower(TowerNode towerNode)
    {
        if (!IsOwner) return;

        if (_currentScrapBank >= 250)
        {
            _currentScrapBank -= 250;
            UpdateUI();
            towerNode.SpawnTower();
        }
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

    private IEnumerator ScavengeScrapPoints()
    {
        yield return new WaitForSeconds(_secondsPerRate);
        _currentScrapBank += _passiveScrapRate;
        UpdateUI();
        StartCoroutine(ScavengeScrapPoints());
    }

    private IEnumerator CountDownTimer()
    {
        yield return new WaitForSeconds(1);

        if(_currentSeconds <= 0)
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("MainMenu");
        }

        _currentSeconds--;
        UpdateUI();

        StartCoroutine(CountDownTimer());
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

        int cost = _scavengerBlueUnit.Cost(_selectedUnit2Spawn);
        if (_currentScrapBank >= cost)
        {
            SpawnUnitServerRpc(_selectedUnit2Spawn, path[0], new ServerRpcParams { Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId } });
            _currentScrapBank -= cost;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        _scrapBankText.text = $"Scrap Bank: {_currentScrapBank}";
        _scrapRateText.text = $"Scrap Rate: {_passiveScrapRate}";
        _countDownText.text = $"Timer: {_currentSeconds}";
    }

    [ServerRpc]
    private void SpawnUnitServerRpc(string unit, char path, ServerRpcParams serverRpcParams)
    {
        //Debug.Log($"SpawnUnitServerRpc Callback - SenderClientId: {serverRpcParams.Receive.SenderClientId}");
        Instantiate(serverRpcParams.Receive.SenderClientId == 0 ? _scavengerBlueUnit.SpawnUnit(unit, path) : _scavengerRedUnit.SpawnUnit(unit, path)).GetComponent<NetworkObject>().Spawn(true);
    }
}
