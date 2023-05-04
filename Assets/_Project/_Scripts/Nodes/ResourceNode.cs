using UnityEngine;
using System;

public class ResourceNode : MonoBehaviour
{
    // bool indicates if the node is gained by the red team or else the blue team
    public static event Action<bool> OnResourceNodeObtained;
    public static event Action<bool> OnResourceNodeLost;

    // Assumes that the map only allows for each team to have one tower near each resource node; confirm with design team
    private Tower[] _towers = new Tower[2];

    private void OnTriggerEnter2D(Collider2D other)
    {
        Tower tower;

        if (other.TryGetComponent<Tower>(out tower))
        {
            if (_towers[0] == null)
            {
                _towers[0] = tower;
                OnResourceNodeObtained?.Invoke(tower.IsRed());
            }
            else if (_towers[0].IsRed() != tower.IsRed())
            {
                _towers[1] = tower;
                OnResourceNodeLost?.Invoke(!tower.IsRed());
            }

            tower.OnDestroyed += OnTowerDestroyedEventHandler;
        }
    }

    private void OnTowerDestroyedEventHandler(bool isRed)
    {
        for (int i = 0; i < 2; i++)
        {
            if (_towers[i].IsRed() != isRed)
            {
                _towers[0] = _towers[i];
                _towers[1] = null;
                OnResourceNodeLost?.Invoke(isRed);
                return;
            }
        }
    }
}
