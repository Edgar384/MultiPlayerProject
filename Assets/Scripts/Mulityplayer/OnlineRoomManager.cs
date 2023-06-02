using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Managers
{
    public class OnlineRoomManager : MonoBehaviourPunCallbacks
    {
        public static event Action OnCreatedRoomEvent;
        public static event Action OnJoinRoomEvent;
        public static event Action<short,string> OnCreateRoomFailedEvent;
        public static event Action<List<RoomInfo>> OnRoomListUpdateEvent;
        
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
        
        
        public void CreateRoom(string roomName)
        {
            RoomOptions roomOptions = new RoomOptions()
            {
                MaxPlayers = 4,
                PlayerTtl = 10000
            };

            PhotonNetwork.CreateRoom(roomName,roomOptions);
        }
        
        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        #region CallBackEvent

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log($"Joined Room {PhotonNetwork.CurrentRoom.Name}");
            OnJoinRoomEvent?.Invoke();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            Debug.LogError("Join failed..." + Environment.NewLine + message);
        }
        
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            OnCreatedRoomEvent?.Invoke();
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.LogError("Create failed..." + Environment.NewLine + message);
            OnCreateRoomFailedEvent?.Invoke(returnCode, message);
        }
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("Got room list");
            base.OnRoomListUpdate(roomList);
            OnRoomListUpdateEvent?.Invoke(roomList);
        }

        #endregion
    }
}