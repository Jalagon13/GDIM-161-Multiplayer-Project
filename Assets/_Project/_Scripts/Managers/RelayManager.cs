using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RelayManager : MonoBehaviour
{
    [SerializeField] private GameObject _networkManagerUI;
    [SerializeField] private string _mainMenuName = "MainMenu";
    [SerializeField] private TMP_InputField _joinInput;
    [SerializeField] private TextMeshProUGUI _joinText;
    [SerializeField] private TextMeshProUGUI _lobbyText;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Signed in {AuthenticationService.Instance.PlayerId}");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try
        {
            _networkManagerUI.SetActive(false);
            _lobbyText.SetText("Creating game, please wait...");
            _lobbyText.gameObject.SetActive(true);

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            _joinText.text = $"{joinCode}";
            _joinText.gameObject.transform.parent.gameObject.SetActive(true);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            _lobbyText.SetText("Game created!\nHave the other player enter the code below to join!");
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            _lobbyText.SetText("Error creating game, please quit and try again.");
        }
    }

    public async void JoinRelay()
    {
        string joinCode = _joinInput.text;
        Debug.Log(joinCode);
        if (joinCode.Length < 6)
        {
            _lobbyText.SetText("Enter a 6 character alphanumeric code to join a game");
            _lobbyText.gameObject.SetActive(true);
            return;
        }
            try
        {
            _lobbyText.SetText("Connecting to game, please wait...");
            Debug.Log($"Joining relay with {joinCode}");
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            _networkManagerUI.SetActive(false);
            NetworkManager.Singleton.StartClient();
            _lobbyText.SetText("Joined! Waiting for game to begin...");
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            _lobbyText.SetText("Error joining game.\nPlease check you entered the correct code, then try again.");
        }
        _lobbyText.gameObject.SetActive(true);
    }

    public void HideMessage()
    {
        _lobbyText.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(_mainMenuName);
    }
}
