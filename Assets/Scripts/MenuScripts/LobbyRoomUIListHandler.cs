using System;
using System.Collections.Generic;
using GarlicStudios.Online.Managers;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using UnityEngine;

namespace DefaultNamespace.MenuScripts
{
    public class LobbyRoomUIListHandler : MonoBehaviour
    {
        public event Action OnRoomListVisualUpdated;
        private List<RoomInfoDisplayer> _roomInfoDisplayers;
        private List<RoomInfo> _roomInfos;
        [SerializeField] private GameObject _roomPrefab;
        [SerializeField] private Transform _roomListParent;

        public int GetRoomCount => _roomInfoDisplayers.Count;
        private void Awake()
        {
            _roomInfos = new List<RoomInfo>();  
            _roomInfoDisplayers = new List<RoomInfoDisplayer>();
            OnlineLobbyManager.OnRoomListUpdateEvent += UpdateRoomUI;
        }

        public void UpdateRoomUI(List<RoomInfo> roomList)
        {
            //if (roomList == null || roomList.Count==0) { return; }
            foreach (RoomInfo roomInfo in roomList) 
            {
                if(roomInfo.RemovedFromList)
                    _roomInfos.Remove(roomInfo);

                else
                    _roomInfos.Add(roomInfo);
            }

            foreach (RoomInfoDisplayer roomDisplayer in _roomInfoDisplayers)
            {
                Destroy(roomDisplayer.gameObject);
            }

            _roomInfoDisplayers.Clear();

            for (int i = 0; i < _roomInfos.Count; i++)
            {
                if(i<=_roomInfoDisplayers.Count)
                {
                    var roomObject = Instantiate(_roomPrefab, _roomListParent).GetComponent<RoomInfoDisplayer>();
                    roomObject.SetRoomInfo(_roomInfos[i]);
                    roomObject.OnJoinRoom += OnlineLobbyManager.JoinRoom;
                    _roomInfoDisplayers.Add(roomObject);
                    continue;
                }
                _roomInfoDisplayers[i].SetRoomInfo(_roomInfos[i]);
            }
            OnRoomListVisualUpdated?.Invoke();
        }

        public RoomInfoDisplayer GetRoom(int roomIndex)
        {
            if (_roomInfoDisplayers.Count >= roomIndex)
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

            for (int i = 0; i < _roomInfoDisplayers.Count; i++)
            {
                _roomInfoDisplayers[i].OnJoinRoom -= OnlineLobbyManager.JoinRoom;
            }
        }
    }
}