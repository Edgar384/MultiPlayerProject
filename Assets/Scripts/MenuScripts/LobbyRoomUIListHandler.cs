using System;
using System.Collections.Generic;
using GarlicStudios.Online.Managers;
using Photon.Realtime;
using UnityEngine;

namespace DefaultNamespace.MenuScripts
{
    public class LobbyRoomUIListHandler : MonoBehaviour
    {
        private List<RoomInfoDisplayer> _roomInfoDisplayers;
        
        [SerializeField] private GameObject _roomPrefab;
        [SerializeField] private Transform _roomListParent;

        public int GetRoomCount => _roomInfoDisplayers.Count;
        private void Awake()
        {
            _roomInfoDisplayers  = new List<RoomInfoDisplayer>();
            OnlineLobbyManager.OnRoomListUpdateEvent += UpdateRoomUI;
        }

        public void UpdateRoomUI(List<RoomInfo> roomList)
        {
            foreach (var room in roomList)
            {
                var roomObject = Instantiate(_roomPrefab, _roomListParent).GetComponent<RoomInfoDisplayer>();
                roomObject.SetRoomInfo(room);
                roomObject.OnJoinRoom += OnlineLobbyManager.JoinRoom;
                _roomInfoDisplayers.Add(roomObject);
                
                Debug.Log($"Room add to room list room name: {room.Name}");
            }
        }

        public RoomInfoDisplayer GetRoom(int roomIndex)
        {
            if (_roomInfoDisplayers.Count < roomIndex)
                return _roomInfoDisplayers[roomIndex];

            throw new Exception("Doesnt have room in this index");
        }

        private void OnDisable()
        {
            foreach (var roomInfoDisplayer in _roomInfoDisplayers)
            {
                roomInfoDisplayer.OnJoinRoom -= OnlineLobbyManager.JoinRoom;
            }
        }

        private void OnDestroy()
        {
            OnlineLobbyManager.OnRoomListUpdateEvent -= UpdateRoomUI;
        }
    }
}