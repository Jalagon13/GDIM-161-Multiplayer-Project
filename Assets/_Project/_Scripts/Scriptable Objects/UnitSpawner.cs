using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Spawner", menuName = "Units")]
public class UnitSpawner : ScriptableObject
{
    [SerializeField] private GameObject[] _scavengerUnits;
    [SerializeField] private GameObject[] _canagoreUnits;
    [SerializeField] private GameObject[] _tankerUnits;

    public GameObject SpawnUnit(string unit, char path)
    {
        // get the corresponding units
        GameObject[] units;
        switch (unit)
        {
            case "scavenger":
                units = _scavengerUnits;
                break;
            case "canagore":
                units = _canagoreUnits;
                break;
            case "tanker":
                units = _tankerUnits;
                break;

            default:
                Debug.Log(unit + "unit not found");
                return null;
        }
        // get unit with specified path
        switch (path)
        {
            case 'u':
                return units[0];
            case 'm':
                return units[1];
            case 'd':
                return units[2];
            default:
                return null;
        }
    }
}
