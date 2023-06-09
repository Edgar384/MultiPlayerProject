using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.CharcterSelect;
using GarlicStudios.Online.Data;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GarlicStudios.Online.Managers
{
    public class OnlineRoomManager : MonoBehaviourPunCallbacks
    {
        private const string UPDATE_READY_LIST = nameof(UpdatePlayerReadyListRFC);
        
        public static event Action<Player> OnPlayerEnteredRoomEvent;
        public static event Action<Player> OnPlayerLeftRoomEvent;
        public static event Action<Player> OnMasterClientSwitchedEvent;
        public static event Action OnCreatedRoomEvent;
        public static event Action OnJoinRoomEvent;

        [SerializeField] private OnlineCharacterSelect _characterSelect;

        private bool[] _readyList;
        
        public Dictionary<int, OnlinePlayer> ConnectedPlayers { get; private set; }
        
        public OnlinePlayer Player { get; private set; }
        
        public OnlinePlayer MasterClient { get; private set; }
        
        public bool IsAllReady => _readyList.All(VARIABLE => VARIABLE);

        private void Awake()
        {
            ConnectedPlayers = new Dictionary<int, OnlinePlayer>();
        }
        
        private void UpdatePlayerReadyList(int playerId, bool isReady)
        {
            object[] objects = new object[]
            {
                playerId,
                isReady
            };
            
            photonView.RPC(UPDATE_READY_LIST,RpcTarget.AllBuffered,objects);
        }

        #region RPC

        [PunRPC]
        private void UpdatePlayerReadyListRFC(int playerId, bool isReady)
        {
            if (!ConnectedPlayers.TryGetValue(playerId,out var player))
            {
                Debug.LogError("can not find player");
                return;
            }
            
            player.SetReadyStatus(isReady);
        }

        #endregion
        

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            Debug.Log("Leave Room");
        }

        #region PunCallbacks

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            _readyList = new bool[PhotonNetwork.CurrentRoom.MaxPlayers];
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
                Debug.LogError("Player already isn the room");
                return;
            }

            var onlinePlayer = new OnlinePlayer(newPlayer);
            onlinePlayer.OnPlayerReadyChanged += UpdatePlayerReadyList;
            
            ConnectedPlayers.Add(newPlayer.ActorNumber, onlinePlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (!ConnectedPlayers.TryGetValue(otherPlayer.ActorNumber, out var player))
            {
                Debug.LogError("can not find player");
                return;
            }

            player.OnPlayerReadyChanged -= UpdatePlayerReadyList;
            
            ConnectedPlayers.Remove(otherPlayer.ActorNumber);
        }
        
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            //need to add logic
        }

        #endregion
    }
}