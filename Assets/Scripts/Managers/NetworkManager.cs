using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : Mirror.NetworkManager {
    [Scene] [SerializeField] string _menuScene;
    [SerializeField] int _minPlayer = 2;

    [Header("Lobby")]
    [SerializeField] LobbyPlayer _roomPlayerPrefab;

    [Header("Game")]
    [SerializeField] GamePlayer _gamePlayerPrefab;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<LobbyPlayer> LobbyPlayers { get; } = new List<LobbyPlayer>();
    public List<GamePlayer> GamePlayers { get; } = new List<GamePlayer>();

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
            LobbyPlayer roomPlayerInstance = Instantiate(_roomPlayerPrefab);
            roomPlayerInstance.IsLeader = LobbyPlayers.Count == 0;
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        if(conn.identity != null) {
            LobbyPlayer player = conn.identity.GetComponent<LobbyPlayer>();
            LobbyPlayers.Remove(player);
            NotifyPlayersOfReadyState();
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer() {
        LobbyPlayers.Clear();
        base.OnStopServer();
    }

    public void NotifyPlayersOfReadyState() {
        foreach (LobbyPlayer player in LobbyPlayers) player.HandleReadyToStart(IsReadyToStart());
    }

    private bool IsReadyToStart() {
        if (numPlayers < _minPlayer) return false;
        return !LobbyPlayers.Any(player => !player.IsReady);
    }

    public void StartGame() {
        if(SceneManager.GetActiveScene().path == _menuScene && IsReadyToStart()) ServerChangeScene("SampleScene");
    }

    public override void ServerChangeScene(string newSceneName) {
        if(SceneManager.GetActiveScene().path == _menuScene) {
            for(int i = LobbyPlayers.Count - 1; i >=0; i--) {
                NetworkConnectionToClient conn = LobbyPlayers[i].connectionToClient;
                GamePlayer gamePlayerInstance = Instantiate(_gamePlayerPrefab);
                gamePlayerInstance.SetDisplayName(LobbyPlayers[i].DisplayName);
                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
    }
}
