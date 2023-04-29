using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject _scavengerUnit;

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
    }

    private void SpawnUnit(InputAction.CallbackContext context)
    {
        Instantiate(_scavengerUnit).GetComponent<NetworkObject>().Spawn(true);
    }
}
