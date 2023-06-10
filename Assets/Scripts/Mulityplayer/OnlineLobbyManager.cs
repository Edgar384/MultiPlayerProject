using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GarlicStudios.Online.Managers
{
    public class OnlineLobbyManager : MonoBehaviourPunCallbacks
    {
        public static event Action<short,string> OnCreateRoomFailedEvent;
        public static event Action<short,string> OnJoinedRoomFailedEvent;
        public static event Action<List<RoomInfo>> OnRoomListUpdateEvent;

        private List<RoomInfo> RoomList { get; set; }

        private void Awake()
        {
            RoomList = new List<RoomInfo>();
        }

        #region Test

#if UNITY_EDITOR
        [ContextMenu("CreateRoom")]
        public void CreateRoom()
        {
            RoomOptions roomOptions = new RoomOptions()
            {
                MaxPlayers = 4,
                PlayerTtl = 10000
            };

            PhotonNetwork.CreateRoom("TestRoom",roomOptions);
        }
        
        [ContextMenu("JoinRoom")]
        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom("TestRoom");
        }
#endif 

        #endregion
        
        public static void CreateRoom(string roomName)
        {
            RoomOptions roomOptions = new RoomOptions()
            {
                MaxPlayers = 4,
                PlayerTtl = 10000
            };

            PhotonNetwork.CreateRoom(roomName,roomOptions);
        }

        public static void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        #region PunCallbacks
        
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            Debug.LogError("Join failed..." + Environment.NewLine + message);
            OnJoinedRoomFailedEvent?.Invoke(returnCode, message);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.LogError("Create failed..." + Environment.NewLine + message);
            OnCreateRoomFailedEvent?.Invoke(returnCode, message);
        }
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
            foreach (var roomInfo in roomList)
            {
                if (RoomList.Contains(roomInfo))
                    continue;
                
                RoomList.Add(roomInfo);
            }
            
            Debug.Log("Room list UPDATE");
            OnRoomListUpdateEvent?.Invoke(roomList);
        }

        #endregion
    }
}