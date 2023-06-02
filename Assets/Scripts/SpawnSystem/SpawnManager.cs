using System;
using System.Collections.Generic;
using DefaultNamespace;
using Managers;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpawnSystem
{
    [Serializable]
    public class SpawnManager : IPunObservable
    {
        //private const string ASK_FOR_RANDOM_SPAWN_POINT_RPC = nameof(AskForRandomSpawnPoint);
        //private const string SPAWN_PLAYER_CLIENT_RPC = nameof(SpawnPlayer);
        private const string LOCAL_PLAYER_PREFAB_NAME = "NetworkPlayer";

        [SerializeField] private SpawnPoint[] _spawnPoints;

        private bool[] _spawnPointData;

        public void Init()
        {
            for (int i = 0; i < _spawnPoints.Length; i++)
                _spawnPoints[i].Init(i);


            _spawnPointData = new bool[_spawnPoints.Length];

            for (int i = 0; i < _spawnPointData.Length; i++)
                _spawnPointData[i] = false;
        }
        
        [PunRPC]
        public LocalPlayer SpawnPlayer(OnlinePlayer onlinePlayer)
        {
            SpawnPoint spawnPoint = AskForRandomSpawnPoint();
           
            var localPlayer = PhotonNetwork.Instantiate(onlinePlayer.PlayerData.PreFabName,
                spawnPoint.transform.position,
                spawnPoint.transform.rotation).GetComponent<LocalPlayer>();
            
            localPlayer.Init(onlinePlayer);

            return localPlayer;
        }
        
        private SpawnPoint AskForRandomSpawnPoint()
        {
            List<SpawnPoint> availableSpawnPoints = new List<SpawnPoint>();
            
            for (int i = 0; i < _spawnPointData.Length; i++)
            {
                if (!_spawnPointData[i])
                    availableSpawnPoints.Add(GetSpawnPointByID(i));
            }
            
            SpawnPoint chosenSpawnPoint =
                availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
            chosenSpawnPoint.SetSpawnPointToTaken();
        
            bool[] updateSpawnPointData = new bool[_spawnPoints.Length];
            
            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                updateSpawnPointData[i] = _spawnPoints[i].IsTaken;
            }

            _spawnPointData = updateSpawnPointData;
            
            return chosenSpawnPoint;
        }
        
        private SpawnPoint GetSpawnPointByID(int targetID)
        {
            foreach (SpawnPoint spawnPoint in _spawnPoints)
            {
                if (spawnPoint.ID == targetID)
                    return spawnPoint;
            }

            return null;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_spawnPointData);
            }
            else
            {
                _spawnPointData = stream.ReceiveNext() as bool[];
            }
        }
    }
}