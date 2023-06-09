using System;
using System.Collections.Generic;
using DefaultNamespace;
using GarlicStudios.Online.Data;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpawnSystem
{
    [Serializable]
    public class SpawnManager
    {
        //private const string ASK_FOR_RANDOM_SPAWN_POINT_RPC = nameof(AskForRandomSpawnPoint);
        //private const string SPAWN_PLAYER_CLIENT_RPC = nameof(SpawnPlayer);
        private const string LOCAL_PLAYER_PREFAB_NAME = "NetworkPlayer";

        [SerializeField] private SpawnPoint[] _spawnPoints;
        
        private Dictionary<int,SpawnPoint> _spawnPointDictionary;


        public void Init()
        {
            _spawnPointDictionary = new Dictionary<int, SpawnPoint>();

            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                _spawnPointDictionary.Add(_spawnPoints[i].ID, _spawnPoints[i]);
                _spawnPoints[i].Init(i);
            }
        }
        
        [PunRPC]
        public LocalPlayer SpawnPlayer(OnlinePlayer onlinePlayer)
        {
            SpawnPoint spawnPoint = AskForRandomSpawnPoint();
           
            var localPlayer = PhotonNetwork.Instantiate(onlinePlayer.PlayerData.PreFabName,
                spawnPoint.transform.position,
                spawnPoint.transform.rotation).GetComponent<LocalPlayer>();
            
            spawnPoint.SetSpawnPointToTaken();
            localPlayer.Init(onlinePlayer);

            return localPlayer;
        }
        
        private SpawnPoint AskForRandomSpawnPoint()
        {
            List<SpawnPoint> availableSpawnPoints = new List<SpawnPoint>();

            foreach (var keyValuePair in _spawnPointDictionary)
            {
                if (!keyValuePair.Value.IsTaken)
                    availableSpawnPoints.Add(keyValuePair.Value);
            }
            
            SpawnPoint chosenSpawnPoint =
                availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
            chosenSpawnPoint.SetSpawnPointToTaken();
            
            return chosenSpawnPoint;
        }
    }
}