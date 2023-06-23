using System;
using System.Collections.Generic;
using DefaultNamespace;
using GarlicStudios.Online.Data;
using GarlicStudios.Online.Managers;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpawnSystem
{
    [Serializable]
    public class SpawnManager : MonoBehaviourPun
    {
        private const string  SPAWN_PLAYER = nameof(SpawnPlayer);
        private const string  SET_SPAWN_POINT_STATUS = nameof(SetSpawnPointStatus_RPC);
        
        [SerializeField] private SpawnPoint[] _spawnPoints;
        private int _spawnPointCount = 0;
        private Dictionary<int,SpawnPoint> _spawnPointDictionary;
        
        
        private void Awake()
        {
            _spawnPointDictionary = new Dictionary<int, SpawnPoint>();

            foreach (var spawnPoint in _spawnPoints)
            {
                RegisterSpawnPoint(spawnPoint);
            }

            if(OnlineRoomManager.Player is not null) 
            SpawnPlayer(OnlineRoomManager.Player);
        }
        
       
        public void SpawnPlayer(OnlinePlayer onlinePlayer)
        {
            if (onlinePlayer.InScene)
                return;
            
            SpawnPoint spawnPoint = AskForRandomSpawnPoint();

            if (spawnPoint is null)
            {
                Debug.Log("No spawn point available");
                return;
            }
            
            var localPlayer = PhotonNetwork.Instantiate(onlinePlayer.PlayerData.PreFabName,
                spawnPoint.GetPosition,
                quaternion.identity).GetComponent<LocalPlayer>();
            
            onlinePlayer.SetLocalPlayer(localPlayer);
                
            photonView.RPC(SET_SPAWN_POINT_STATUS,RpcTarget.AllViaServer,spawnPoint.ID);
        }
        
        
        private SpawnPoint AskForRandomSpawnPoint()
        {
            List<SpawnPoint> availableSpawnPoints = new List<SpawnPoint>();

            foreach (var keyValuePair in _spawnPointDictionary)
            {
                if (!keyValuePair.Value.IsTaken)
                    availableSpawnPoints.Add(keyValuePair.Value);
            }

            if (availableSpawnPoints.Count == 0)
                return null;
            
            SpawnPoint chosenSpawnPoint =
                availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
            
            return chosenSpawnPoint;
        }
        
        [PunRPC]
        private void SetSpawnPointStatus_RPC(int spawnPointId)
        {
            _spawnPointDictionary[spawnPointId].SetSpawnPointToTaken();
        }

        public void RegisterSpawnPoint(SpawnPoint spawnPoint)
        {
            spawnPoint.Init(_spawnPointCount);
            _spawnPointDictionary.Add(spawnPoint.ID, spawnPoint);
            _spawnPointCount++;
        }

        private void OnDrawGizmos()
        {
            foreach (var spawnPoint in _spawnPoints) 
            {
                spawnPoint.DrawGizmos();
            }
        }
    }
}