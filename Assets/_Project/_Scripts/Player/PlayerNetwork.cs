using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject _scavengerBlueUnit;
    [SerializeField] private GameObject _scavengerRedUnit;

    private DefaultControls _defaultControls;

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
        _defaultControls.Player.SpawnOpponentUnit.started += SpawnOpponentUnit;
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
}
