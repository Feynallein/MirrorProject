using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] NetworkManager networkManager;
    [Header("UI")]
    [SerializeField] GameObject _landingPagePanel;

    public void HostLobby() {
        networkManager.StartHost();
        _landingPagePanel.SetActive(false);
    }
}
