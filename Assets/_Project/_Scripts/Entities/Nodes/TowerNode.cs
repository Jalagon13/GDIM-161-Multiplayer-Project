using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerNode : NetworkBehaviour, IPointerClickHandler
{
    public static event Action<TowerNode> NodeClickEvent;

    [SerializeField] private GameObject _blueTowerPrefab;
    [SerializeField] private GameObject _redTowerPrefab;
    [SerializeField] private GameObject _neutralSprite;
    [SerializeField] private GameObject _redSprite; 
    [SerializeField] private GameObject _blueSprite; 

    private Tower _tower;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !IsOccupied())
        {
            NodeClickEvent?.Invoke(this);
        }
    }

    public void SpawnTower()
    {
        SpawnTowerServerRpc(new ServerRpcParams { Receive = new ServerRpcReceiveParams { SenderClientId = OwnerClientId } });
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnTowerServerRpc(ServerRpcParams serverRpcParams)
    {
        GameObject towerGo = Instantiate(serverRpcParams.Receive.SenderClientId == 0 ? _blueTowerPrefab : _redTowerPrefab, transform.position, Quaternion.identity);
        towerGo.GetComponent<NetworkObject>().Spawn(true);
        Occupy(towerGo.GetComponent<Tower>());
    }

    public void Occupy(Tower tower)
    {
        Debug.Log(tower.gameObject.tag);

        if (tower.gameObject.CompareTag("Red"))
        {
            _neutralSprite.SetActive(false);
            _redSprite.SetActive(true);
            _blueSprite.SetActive(false);
        }
        else if(tower.gameObject.CompareTag("Blue"))
        {
            _neutralSprite.SetActive(false);
            _redSprite.SetActive(false);
            _blueSprite.SetActive(true);
        }

        _tower = tower;
        _tower.OnDestroyed += OnTowerDestroyedEventHandler;
    }

    private void OnTowerDestroyedEventHandler(bool _)
    {
        _neutralSprite.SetActive(true);
        _redSprite.SetActive(false);
        _blueSprite.SetActive(false);

        _tower.OnDestroyed -= OnTowerDestroyedEventHandler;
        _tower = null;
    }

    public bool IsOccupied()
    {
        return _tower != null;
    }
}
