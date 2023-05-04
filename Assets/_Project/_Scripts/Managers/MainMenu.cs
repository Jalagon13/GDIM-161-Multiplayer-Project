using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string _startingLevelName = "TestBattleScene";
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;
    private void Awake()
    {
        _startButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(_startingLevelName);
        });

        _quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
        
}
