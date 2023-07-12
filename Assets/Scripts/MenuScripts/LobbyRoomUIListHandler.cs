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

        private void Awake()
        {
            _roomInfoDisplayers  = new List<RoomInfoDisplayer>();
        }

        public void UpdateRoomUI(List<RoomInfo> roomList)
        {
            foreach (var room in roomList)
            {
                var roomObject = Instantiate(_roomPrefab, _roomListParent).GetComponent<RoomInfoDisplayer>();
                roomObject.transform.rotation = _roomListParent.rotation;
                roomObject.SetRoomInfo(room);
                roomObject.OnJoinRoom += OnlineLobbyManager.JoinRoom;
                _roomInfoDisplayers.Add(roomObject);
                
                Debug.Log($"Room add to room list room name: {room.Name}");
            }
        }

        private void OnDisable()
        {
            foreach (var roomInfoDisplayer in _roomInfoDisplayers)
            {
                roomInfoDisplayer.OnJoinRoom -= OnlineLobbyManager.JoinRoom;
            }
        }
    }
}