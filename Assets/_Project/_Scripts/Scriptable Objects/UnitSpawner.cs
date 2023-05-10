using UnityEngine;

[CreateAssetMenu(fileName = "New Spawner", menuName = "Units")]
public class UnitSpawner : ScriptableObject
{
    [SerializeField] private GameObject _unitUp;
    [SerializeField] private GameObject _unitMid;
    [SerializeField] private GameObject _unitDown;

    public GameObject GetUnit(char c)
    {
        if (c == 'u')
            return _unitUp;
        else if (c == 'm')
            return _unitMid;
        else if (c == 'd')
            return _unitDown;
        else
            return null;
    }
}
