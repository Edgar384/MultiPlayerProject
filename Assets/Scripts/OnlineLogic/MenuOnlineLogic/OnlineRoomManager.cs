using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.SciptableObject.PlayerData;
using GamePlayLogic;
using GarlicStudios.Online.Data;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GarlicStudios.Online.Managers
{
    public class OnlineRoomManager : MonoBehaviourPunCallbacks
    {
        private const int NUMBER_OF_CARS = 4;
        private const string UPDATE_READY_LIST = nameof(UpdatePlayerReadyList_RPC);
        private const string SEND_CAR_DATA = nameof(SendCarData_RPC);
        
        public static event Action OnPlayerListUpdateEvent;
        public static event Action<Player> OnPlayerEnteredRoomEvent;
        public static event Action<Player> OnPlayerLeftRoomEvent;
        public static event Action<Player> OnMasterClientSwitchedEvent;
        public static event Action OnCreatedRoomEvent;
        public static event Action OnJoinRoomEvent;
        public static event Action OnSendPLayerData_RPC;
        [SerializeField] private PlayerData[] _playerDatas;

        public static Dictionary<PlayerData, bool> NewCarAvailabilityList;
        
        [SerializeField] private OnlineRoomUIHandler _uiHandler;//need to remove
        public static PlayerData[] PlayersData { get; private set; }
        public static Dictionary<int, OnlinePlayer> ConnectedPlayers { get; private set; }
        
        public static OnlinePlayer Player { get; private set; }
        
        public static OnlinePlayer MasterClient { get; private set; }
        
        public static bool IsAllReady => ConnectedPlayers.All(player => player.Value.IsReady);

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            PlayersData = new PlayerData[NUMBER_OF_CARS];

            for (int i = 0; i < _playerDatas.Length; i++)
                PlayersData[i] = _playerDatas[i];

            ConnectedPlayers = new Dictionary<int, OnlinePlayer>();
        }

        public void OnCharacterSelect(int carIndex, bool isReady)
        {
            photonView.RPC(UPDATE_READY_LIST,RpcTarget.AllViaServer,PhotonNetwork.LocalPlayer.ActorNumber,carIndex,isReady);
        }

        #region RPC

        [PunRPC]
        private void UpdatePlayerReadyList_RPC(int playerId,int carIndex ,bool isReady)
        {
            if (!ConnectedPlayers.TryGetValue(playerId,out var player))
            {
                Debug.LogError("can not find player");
                return;
            }
            
            Debug.Log("Update ready list");
            player.SetReadyStatus(isReady);

            player.SetPlayerData(isReady ? _playerDatas[carIndex] : null);
            Debug.Log("Update car status");
            NewCarAvailabilityList[_playerDatas[carIndex]] = isReady;
            OnPlayerListUpdateEvent?.Invoke();
        }
        
        [PunRPC]
        private void SendCarData_RPC(bool[] carData)
        {
            Debug.Log("Receive car data");
            
            NewCarAvailabilityList = new Dictionary<PlayerData, bool>();
            
            for (int i = 0; i < carData.Length; i++)
                NewCarAvailabilityList.Add(_playerDatas[i], carData[i]);
            
            OnPlayerListUpdateEvent?.Invoke();
        }

        #endregion
        

        public static void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            Debug.Log("Leave Room");
        }

        #region PunCallbacks

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
                      
            NewCarAvailabilityList = new Dictionary<PlayerData, bool>();

            if (PhotonNetwork.IsMasterClient)
            {
                foreach (var playerData in _playerDatas)
                    NewCarAvailabilityList.Add(playerData,false);
            }
            
            ConnectedPlayers.Add(PhotonNetwork.LocalPlayer.ActorNumber,new OnlinePlayer(PhotonNetwork.LocalPlayer));
            MasterClient  = ConnectedPlayers[PhotonNetwork.MasterClient.ActorNumber];
            Player  = MasterClient;
            OnCreatedRoomEvent?.Invoke();
        }
        
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log($"Joined Room {PhotonNetwork.CurrentRoom.Name}");
            OnJoinRoomEvent?.Invoke();
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (ConnectedPlayers.TryGetValue(newPlayer.ActorNumber, out var player))
            {
                Debug.LogError("PhotonData already isn the room");
                return;
            }

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                var carData = NewCarAvailabilityList.Values.ToArray();
                
                var players = ConnectedPlayers.Values.ToArray();
                
                int[] idsData = new [] {-1,-1,-1,-1};
                bool[] readyData = new bool[players.Length];
                
                for (int i = 0; i < players.Length; i++)
                {
                    readyData[i] = players[i].IsReady;

                    if (players[i].PlayerData != null)
                        idsData[i] = players[i].PlayerData.CharacterID;
                }
            
                photonView.RPC(nameof(SendPLayerData_RPC),newPlayer,idsData,readyData);
                
                photonView.RPC(SEND_CAR_DATA, newPlayer, carData);
            }
            
            var onlinePlayer = new OnlinePlayer(newPlayer);
            ConnectedPlayers.Add(newPlayer.ActorNumber, onlinePlayer);
            OnPlayerListUpdateEvent?.Invoke();
            photonView.RPC(nameof(UpdateUI), newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (!ConnectedPlayers.TryGetValue(otherPlayer.ActorNumber, out var player))
            {
                Debug.LogError("can not find player");
                return;
            }

            ConnectedPlayers.Remove(otherPlayer.ActorNumber);
            if (OnlineGameManager.LocalPlayers.ContainsKey(otherPlayer.ActorNumber))
                OnlineGameManager.LocalPlayers.Remove(otherPlayer.ActorNumber);
            
            OnPlayerListUpdateEvent?.Invoke();
        }
        
        [PunRPC]
        private void UpdateUI()
        {
            OnSendPLayerData_RPC?.Invoke();
        } 

        
        [PunRPC]
        private void SendPLayerData_RPC(int[] charcterId,bool[] isReday)
        {
            ConnectedPlayers = new Dictionary<int, OnlinePlayer>();
            var players = PhotonNetwork.PlayerList;

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    continue;
                
                var onlinePLayer = new OnlinePlayer(players[i]);
                onlinePLayer.SetReadyStatus(isReday[i]);
                ConnectedPlayers.Add(players[i].ActorNumber, onlinePLayer);

                if (charcterId[i] != -1)
                    onlinePLayer.SetPlayerData(_playerDatas[charcterId[i]]);
            }
            
            var onlinePlayer = new OnlinePlayer(PhotonNetwork.LocalPlayer);
            ConnectedPlayers.Add(PhotonNetwork.LocalPlayer.ActorNumber, onlinePlayer);
            
            Player = ConnectedPlayers[PhotonNetwork.LocalPlayer.ActorNumber];
            MasterClient = ConnectedPlayers[PhotonNetwork.MasterClient.ActorNumber];
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            //need to add logic
        }

        #endregion
    }
}