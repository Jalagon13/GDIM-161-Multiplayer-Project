using UnityEngine;

public class TowerNode : MonoBehaviour
{
    private Tower _tower;

    public void Occupy(Tower tower)
    {
        _tower = tower;
        _tower.OnDestroyed += OnTowerDestroyedEventHandler;
    }

    private void OnTowerDestroyedEventHandler(bool _)
    {
        _tower.OnDestroyed -= OnTowerDestroyedEventHandler;
        _tower = null;
    }

    public bool IsOccupied()
    {
        return _tower != null;
    }
}
