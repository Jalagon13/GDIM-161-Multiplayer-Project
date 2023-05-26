using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private string _redWinName = "RedWin";
    [SerializeField] private string _blueWinName = "BlueWin";
    [SerializeField] private Base _blueBase;
    [SerializeField] private Base _redBase;
    [SerializeField] private GameObject _lobbyPanel;
    private bool _gameStarted = false;


    private void FixedUpdate()
    {
        if (!_gameStarted)
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length >= 2)
            {
                _gameStarted = true;
                _lobbyPanel.SetActive(false);

            }
        }
        else
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
}
