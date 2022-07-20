using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class GamePlayer : NetworkBehaviour {
    NetworkManager _room;

    [SyncVar]
    string _displayName = "Loading...";

    private NetworkManager Room {
        get {
            if(_room == null) _room = Mirror.NetworkManager.singleton as NetworkManager;
            return _room;
        }
    }

    public override void OnStartClient() {
        DontDestroyOnLoad(gameObject);
        Room.GamePlayers.Add(this);
    }

    public override void OnStopClient() {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName) {
        _displayName = displayName;
    }
}
