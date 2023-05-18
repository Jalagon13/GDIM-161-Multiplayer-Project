using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] private Button _scavengerButton;
    [SerializeField] private Button _canagoreButton;
    [SerializeField] private Button _tankerButton;

    private void Awake()
    {
        _scavengerButton.onClick.AddListener(() =>
        {
            _scavengerButton.image.color = Color.yellow;
            _canagoreButton.image.color = Color.white;
            _tankerButton.image.color = Color.white;
        });

        _canagoreButton.onClick.AddListener(() =>
        {
            _scavengerButton.image.color = Color.white;
            _canagoreButton.image.color = Color.yellow;
            _tankerButton.image.color = Color.white;
        });

        _tankerButton.onClick.AddListener(() =>
        {
            _scavengerButton.image.color = Color.white;
            _canagoreButton.image.color = Color.white;
            _tankerButton.image.color = Color.yellow;
        });
    }
}
