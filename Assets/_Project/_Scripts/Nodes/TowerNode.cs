using UnityEngine;
using Unity.Netcode;

public class TowerNode : NetworkBehaviour
{
    private NetworkVariable<bool> _occupied = new NetworkVariable<bool>(false);

    public void Occupy()
    {
        //_occupied = true;
    }

    public bool IsOccupied()
    {
        return false;
        //return _occupied;
    }
}
