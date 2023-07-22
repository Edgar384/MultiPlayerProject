using System;
using System.Collections.Generic;
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
        [SerializeField] private SpawnPoint[] _spawnPoints;
        
        private int _spawnPointCount = 0;
        private Dictionary<int,SpawnPoint> _spawnPointDictionary;
        
        private void Awake()
        {
            _spawnPointDictionary = new Dictionary<int, SpawnPoint>();

            foreach (var spawnPoint in _spawnPoints)
                RegisterSpawnPoint(spawnPoint);
        }

        private void Start()
        {
            photonView.RPC(nameof(AskForRandomSpawnPoint),RpcTarget.MasterClient,OnlineRoomManager.Player.PhotonData.ActorNumber);
        }
        
        [PunRPC]
        public void SpawnPlayer(float x,float y,float z)
        {
            Vector3 spawnPoint = new Vector3(x,y,z);
            
            PhotonNetwork.Instantiate(OnlineRoomManager.Player.PlayerData.PreFabName,
                spawnPoint,
                quaternion.identity);
        }
        
        [PunRPC]
        private void AskForRandomSpawnPoint(int playerId)
        {
            List<SpawnPoint> availableSpawnPoints = new List<SpawnPoint>();

            foreach (var keyValuePair in _spawnPointDictionary)
            {
                if (!keyValuePair.Value.IsTaken)
                    availableSpawnPoints.Add(keyValuePair.Value);
            }

            if (availableSpawnPoints.Count != 0)
            {
                var chosenSpawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
                
                photonView.RPC(nameof(SetSpawnPointStatus_RPC),RpcTarget.AllViaServer,chosenSpawnPoint.ID);
                photonView.RPC(nameof(SpawnPlayer),OnlineRoomManager.ConnectedPlayers[playerId].PhotonData,chosenSpawnPoint.GetPosition.x,chosenSpawnPoint.GetPosition.y,chosenSpawnPoint.GetPosition.z);
            }
        }
        
        [PunRPC]
        private void SetSpawnPointStatus_RPC(int spawnPointId)
        {
            _spawnPointDictionary[spawnPointId].SetSpawnPointToTaken();
            Debug.Log($"SpawnPoint {spawnPointId} is taken");
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