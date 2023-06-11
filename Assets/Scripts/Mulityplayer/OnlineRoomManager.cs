using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private bool[] _carAvailabilityList;
        
        [SerializeField] private OnlineRoomUIHandler _uiHandler;
        
        public static Dictionary<int, OnlinePlayer> ConnectedPlayers { get; private set; }
        
        public OnlinePlayer Player { get; private set; }
        
        public OnlinePlayer MasterClient { get; private set; }
        
        public bool IsAllReady => ConnectedPlayers.All(player => player.Value.IsReady);

        private void Awake()
        {
            ConnectedPlayers = new Dictionary<int, OnlinePlayer>();
        }

        public void OnCharacterSelect(int carIndex)
        {
            UpdatePlayerReadyList(PhotonNetwork.LocalPlayer.ActorNumber,carIndex,true);
        }

        private void UpdatePlayerReadyList(int playerId,int carIndex, bool isReady)
        {
            photonView.RPC(UPDATE_READY_LIST,RpcTarget.AllViaServer,playerId,isReady);
            photonView.RPC(UPDATE_CAR_AVAILABILITY_LIST,RpcTarget.AllViaServer,carIndex,!isReady);
        }

        #region RPC

        [PunRPC]
        private void UpdatePlayerReadyList_RPC(int playerId, bool isReady)
        {
            if (!ConnectedPlayers.TryGetValue(playerId,out var player))
            {
                Debug.LogError("can not find player");
                return;
            }
            
            Debug.Log("Update ready list");
            player.SetReadyStatus(isReady);
            OnPlayerListUpdateEvent?.Invoke();
        }

        [PunRPC]
        private void UpdateCarAvailability_RPC(int carIndex, bool carStatus)
        {
            Debug.Log("Update car status");
            _carAvailabilityList[carIndex] = carStatus;
            _uiHandler.SetCarIsTaken(carIndex,carStatus);
        }
        
        [PunRPC]
        private void SendCarData_RPC(bool[] carData)
        {
            Debug.Log("Receive car data");
            _carAvailabilityList  = carData;

            for (int i = 0; i < _carAvailabilityList.Length; i++)
                _uiHandler.SetCarIsTaken(i,_carAvailabilityList[i]);
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
            _carAvailabilityList  = new bool[NUMBER_OF_CARS];

            for (int i = 0; i < _carAvailabilityList.Length; i++)
                _carAvailabilityList[i] = true;

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
                Debug.LogError("Player already isn the room");
                return;
            }

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                photonView.RPC(SEND_CAR_DATA,RpcTarget.AllViaServer,_carAvailabilityList);
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