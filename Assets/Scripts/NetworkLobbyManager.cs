using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkLobbyManager : NetworkManager {
    [Scene] [SerializeField] string _menuScene;
    [SerializeField] int _minPlayer = 2;

    [Header("Room")]
    [SerializeField] InLobbyPlayer _roomPlayerPrefab;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<InLobbyPlayer> Players { get; } = new List<InLobbyPlayer>();

    public override void OnStartServer() {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }

    public override void OnStartClient() {
        GameObject[] spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");
        foreach (GameObject prefab in spawnablePrefabs) NetworkClient.RegisterPrefab(prefab);
    }

    public override void OnStartHost() {
        base.OnStartHost();
    }

    public override void OnClientConnect() {
        base.OnClientConnect();
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect() {
        base.OnClientDisconnect();
        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn) {
        if(numPlayers >= maxConnections) {
            conn.Disconnect();
            return;
        }

        if(SceneManager.GetActiveScene().path != _menuScene) {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        if(SceneManager.GetActiveScene().path == _menuScene) {
            InLobbyPlayer roomPlayerInstance = Instantiate(_roomPlayerPrefab);
            roomPlayerInstance.IsLeader = Players.Count == 0;
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        if(conn.identity != null) {
            InLobbyPlayer player = conn.identity.GetComponent<InLobbyPlayer>();
            Players.Remove(player);
            NotifyPlayersOfReadyState();
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer() {
        Players.Clear();
        base.OnStopServer();
    }

    public void NotifyPlayersOfReadyState() {
        foreach (InLobbyPlayer player in Players) player.HandleReadyToStart(IsReadyToStart());
    }

    private bool IsReadyToStart() {
        if (numPlayers < _minPlayer) return false;
        return !Players.Any(player => !player.IsReady);
    }
}
