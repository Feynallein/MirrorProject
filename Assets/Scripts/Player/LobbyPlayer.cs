using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class LobbyPlayer : NetworkBehaviour {
    [Header("UI")]
    [SerializeField] GameObject _lobbyUI;
    [SerializeField] TMP_Text[] _playerNames = new TMP_Text[4];
    [SerializeField] TMP_Text[] _readyPlayerNames = new TMP_Text[4];
    [SerializeField] Button _startGameButton;

    bool _isLeader;
    NetworkManager _room;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    public bool IsLeader {
        set { 
            _isLeader = value;
            _startGameButton.gameObject.SetActive(value); // Makes button is only available for the leader
        } 
    }

    private NetworkManager Room {
        get {
            if(_room == null) _room = Mirror.NetworkManager.singleton as NetworkManager;
            return _room;
        }
    }

    public override void OnStartAuthority() {
        CmdSetDisplayName(PlayerNameInput.DisplayName);
        _lobbyUI.SetActive(true);
    }

    public override void OnStartClient() {
        Room.LobbyPlayers.Add(this);
        UpdateDisplay();
    }

    public override void OnStopClient() {
        Room.LobbyPlayers.Remove(this);
        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    void UpdateDisplay() {
        if(!hasAuthority) {
            foreach(LobbyPlayer player in Room.LobbyPlayers) {
                if (player.hasAuthority) {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for(int i = 0; i < _playerNames.Length; i++) {
            _playerNames[i].text = "Waiting for players...";
            _readyPlayerNames[i].text = "";
        }

        for(int i = 0; i < Room.LobbyPlayers.Count; i++) {
            _playerNames[i].text = Room.LobbyPlayers[i].DisplayName;
            _readyPlayerNames[i].text = Room.LobbyPlayers[i].IsReady ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart) {
        if (!_isLeader) return;
        _startGameButton.interactable = readyToStart;
    }

    [Command]
    void CmdSetDisplayName(string displayName) => DisplayName = displayName;

    [Command]
    public void CmdReadyUp() {
        IsReady = !IsReady;
        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame() {
        if (Room.LobbyPlayers[0].connectionToClient != connectionToClient) return;
        Room.StartGame();
    }

}
