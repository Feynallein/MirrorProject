using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour {
    [SerializeField] NetworkManager _networkManager;

    [Header("UI")]
    [SerializeField] GameObject _landingPagePanel;
    [SerializeField] TMP_InputField _ipAddressInputField;
    [SerializeField] Button _joinButton;

    private void OnEnable() {
        NetworkManager.OnClientConnected += HandleClientConnected;
        NetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable() {
        NetworkManager.OnClientConnected -= HandleClientConnected;
        NetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby() {
        string ipAddress = _ipAddressInputField.text;
        _networkManager.networkAddress = ipAddress;
        _networkManager.StartClient();
        _joinButton.interactable = false;
    }

    void HandleClientConnected() {
        _joinButton.interactable = true;
        gameObject.SetActive(false);
        _landingPagePanel.SetActive(false);
    }

    void HandleClientDisconnected() {
        _joinButton.interactable = true;
    }
}
