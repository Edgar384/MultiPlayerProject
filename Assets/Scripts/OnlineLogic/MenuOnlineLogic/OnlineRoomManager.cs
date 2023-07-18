using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.SciptableObject.PlayerData;
using GarlicStudios.Online.Data;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GarlicStudios.Online.Managers
{
    public class OnlineRoomManager : MonoBehaviourPunCallbacks
    {
        private const int NUMBER_OF_CARS = 5;
        private const string UPDATE_READY_LIST = nameof(UpdatePlayerReadyList_RPC);
        private const string UPDATE_CAR_AVAILABILITY_LIST = nameof(UpdateCarAvailability_RPC);
        private const string SEND_CAR_DATA = nameof(SendCarData_RPC);
        
        public static event Action OnPlayerListUpdateEvent;
        public static event Action<Player> OnPlayerEnteredRoomEvent;
        public static event Action<Player> OnPlayerLeftRoomEvent;
        public static event Action<Player> OnMasterClientSwitchedEvent;
        public static event Action OnCreatedRoomEvent;
        public static event Action OnJoinRoomEvent;

        [SerializeField] private PlayerData[] _playerDatas;

        public static Dictionary<PlayerData, bool> NewCarAvailabilityList;
        public static Dictionary<int, PlayerData> PlayerDatas;
        public static bool[] CarAvailabilityList;
        
        [SerializeField] private OnlineRoomUIHandler _uiHandler;//need to remove
        
        public static Dictionary<int, OnlinePlayer> ConnectedPlayers { get; private set; }
        
        public static OnlinePlayer Player { get; private set; }
        
        public static OnlinePlayer MasterClient { get; private set; }
        
        public static bool IsAllReady => ConnectedPlayers.All(player => player.Value.IsReady);

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ConnectedPlayers = new Dictionary<int, OnlinePlayer>();
            PlayerDatas = new Dictionary<int, PlayerData>();
            for (int i = 0; i < _playerDatas.Length; i++)
            {
                PlayerDatas.Add(i, _playerDatas[i]);
            }
        }

        public void OnCharacterSelect(int carIndex, bool isReady)
        {
            if (!ConnectedPlayers.TryGetValue(PhotonNetwork.LocalPlayer.ActorNumber, out var player))
                throw  new Exception("Can not find player");
            
            UpdatePlayerReadyList(PhotonNetwork.LocalPlayer.ActorNumber,carIndex, isReady);
        }

        private void UpdatePlayerReadyList(int playerId,int carIndex, bool isReady)
        {
            photonView.RPC(UPDATE_READY_LIST,RpcTarget.AllViaServer,playerId,carIndex,isReady);
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
            player.SetPlayerData(_playerDatas[carIndex]);
            Debug.Log("Update car status");
            CarAvailabilityList[carIndex] = !isReady;
            OnPlayerListUpdateEvent?.Invoke();
        }

        [PunRPC]
        private void UpdateCarAvailability_RPC(int carIndex, bool carStatus)
        {
           
        }
        
        [PunRPC]
        private void SendCarData_RPC(bool[] carData)
        {
            Debug.Log("Receive car data");
            CarAvailabilityList  = carData;
            NewCarAvailabilityList = new Dictionary<PlayerData, bool>();
            for (int i = 0; i < carData.Length; i++)
            {
                NewCarAvailabilityList.Add(_playerDatas[i], carData[i]);
            }
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
            CarAvailabilityList  = new bool[NUMBER_OF_CARS];

            for (int i = 0; i < CarAvailabilityList.Length; i++)
                CarAvailabilityList[i] = true;

            ConnectedPlayers = new Dictionary<int, OnlinePlayer>();
            ConnectedPlayers.Add(PhotonNetwork.LocalPlayer.ActorNumber,new OnlinePlayer(PhotonNetwork.LocalPlayer));
            MasterClient  = ConnectedPlayers[PhotonNetwork.MasterClient.ActorNumber];
            Player  = MasterClient;
            OnCreatedRoomEvent?.Invoke();
        }
        
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log($"Joined Room {PhotonNetwork.CurrentRoom.Name}");
            GetPlayers();
            Player = ConnectedPlayers[PhotonNetwork.LocalPlayer.ActorNumber];
            MasterClient = ConnectedPlayers[PhotonNetwork.MasterClient.ActorNumber];
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
                photonView.RPC(SEND_CAR_DATA, newPlayer, CarAvailabilityList);
            }

            var onlinePlayer = new OnlinePlayer(newPlayer);
            ConnectedPlayers.Add(newPlayer.ActorNumber, onlinePlayer);
            OnPlayerListUpdateEvent?.Invoke();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (!ConnectedPlayers.TryGetValue(otherPlayer.ActorNumber, out var player))
            {
                Debug.LogError("can not find player");
                return;
            }

            ConnectedPlayers.Remove(otherPlayer.ActorNumber);
            OnPlayerListUpdateEvent?.Invoke();
        }
        
       
        private static void GetPlayers()
        {
            ConnectedPlayers = new Dictionary<int, OnlinePlayer>();
            var players = PhotonNetwork.PlayerList;
            
            foreach (var player in players)
            {
                ConnectedPlayers.Add(player.ActorNumber, new OnlinePlayer(player));
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            //need to add logic
        }

        #endregion
    }
}