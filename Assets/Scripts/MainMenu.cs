using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    [SerializeField] NetworkLobbyManager networkManager;
    [Header("UI")]
    [SerializeField] GameObject _landingPagePanel;

    public void HostLobby() {
        networkManager.StartHost();
        _landingPagePanel.SetActive(false);
    }
}
