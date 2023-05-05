using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    [SerializeField] private string _startingLevelName = "TestBattleScene";
    [SerializeField] private string _mainMenuName = "MainMenu";
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private Button _startMenuButton;
    private void Awake()
    {
        _playAgainButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(_startingLevelName);
        });

        _startMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(_mainMenuName);
        });
    }
        
}
