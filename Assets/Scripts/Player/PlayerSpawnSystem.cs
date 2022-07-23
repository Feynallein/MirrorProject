using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class PlayerSpawnSystem : NetworkBehaviour {
    [SerializeField] GameObject _playerPrefab;

    static List<Transform> _spawnPoints = new List<Transform>();
    int _nextIndex = 0;

    public static void AddSpawnPoint(Transform transform) {
        _spawnPoints.Add(transform);
        _spawnPoints = _spawnPoints.OrderBy(Matrix4x4 => Matrix4x4.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform) => _spawnPoints.Remove(transform);

    public override void OnStartServer() => NetworkManager.OnServerReadied += SpawnPlayer;

    [ServerCallback]
    private void OnDestroy() => NetworkManager.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn) {
        Transform spawnPoint = _spawnPoints.ElementAtOrDefault(_nextIndex);
        if(spawnPoint == null) {
            Debug.Log("Error!");
            return;
        }

        GameObject playerInstance = Instantiate(_playerPrefab, _spawnPoints[_nextIndex].position, _spawnPoints[_nextIndex].rotation);
        NetworkServer.Spawn(playerInstance, conn);

        _nextIndex++;
    }
}
