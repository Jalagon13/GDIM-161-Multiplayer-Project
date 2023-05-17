using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private string _mainMenuName = "MainMenu";
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;
    [SerializeField] private Button _quitButton;

    private void Awake()
    {
        _hostButton.onClick.AddListener(() =>
        {
            //NetworkManager.Singleton.StartHost();
            gameObject.SetActive(false);
        });

        _clientButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        _quitButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(_mainMenuName);
        });
    }

}
