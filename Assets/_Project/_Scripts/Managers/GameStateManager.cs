using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private string _redWinName = "RedWin";
    [SerializeField] private string _blueWinName = "BlueWin";
    [SerializeField] private Base _blueBase;
    [SerializeField] private Base _redBase;

    private void FixedUpdate()
    {
        // sends both to win screen when base goes down
        if (_blueBase == null)
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(_redWinName);
        }
        else if (_redBase == null)
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(_blueWinName);
        }

    }
}
