using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject _scavengerUnit;

    private DefaultControls _defaultControls;
    private GameObject _unit;

    private void Awake()
    {
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
        _defaultControls.Player.SpawnUnit.started += SpawnUnit;
    }

    private void SpawnUnit(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        SpawnUnitServerRpc(new ServerRpcParams { Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId} });
    }

    [ServerRpc]
    private void SpawnUnitServerRpc(ServerRpcParams serverRpcParams)
    {
        Debug.Log($"SpawnUnitServerRpc Callback - SenderClientId: {serverRpcParams.Receive.SenderClientId}");
        _unit = Instantiate(_scavengerUnit);
        _unit.GetComponent<NetworkObject>().Spawn(true);

        if (serverRpcParams.Receive.SenderClientId == 0)
            _unit.GetComponent<ScavengerUnit>().InitializeBlueUnit();
        else
            _unit.GetComponent<ScavengerUnit>().InitializeRedUnit();

        
    }
}
