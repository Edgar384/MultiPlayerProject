using System;
using System.Collections.Generic;
using GarlicStudios.Online.Data;
using Photon.Pun;
using Random = UnityEngine.Random;

namespace SpawnSystem
{
    [Serializable]
    public class SpawnManager
    {
        private int _spawnPointCount = 0;
        private Dictionary<int,SpawnPoint> _spawnPointDictionary;

        public SpawnManager()
        {
            _spawnPointDictionary = new Dictionary<int, SpawnPoint>();
        }

        [PunRPC]
        public void SpawnPlayer(OnlinePlayer onlinePlayer)
        {
            
            SpawnPoint spawnPoint = AskForRandomSpawnPoint();

            PhotonNetwork.Instantiate(onlinePlayer.PlayerData.PreFabName,
                spawnPoint.transform.position,
                spawnPoint.transform.rotation);
            
            spawnPoint.SetSpawnPointToTaken();
        }
        
        [PunRPC]
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

        public void RegisterSpawnPoint(SpawnPoint spawnPoint)
        {
            spawnPoint.Init(_spawnPointCount);
            _spawnPointDictionary.Add(spawnPoint.ID, spawnPoint);
            _spawnPointCount++;
        }
    }
}