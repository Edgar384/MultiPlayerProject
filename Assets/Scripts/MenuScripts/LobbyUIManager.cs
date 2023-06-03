using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

namespace DefaultNamespace.MenuScripts
{
    public class LobbyUIManager : MonoBehaviour
    {
        private List<RoomInfoDisplayer> _roomInfoDisplayers;
        
        [SerializeField] private GameObject _roomPrefab;
        [SerializeField] private Transform _roomListParent;

        public void UpdateRoomUI(List<RoomInfo> roomList)
        {
            foreach (var room in roomList)
            {
                var roomObject = Instantiate(_roomPrefab, _roomListParent).GetComponent<RoomInfoDisplayer>();
                
                roomObject.SetRoomInfo(room);
                //roomObject.OnJoinRoom += JoinRoom; //need to fix
                _roomInfoDisplayers.Add(roomObject);
                
                Debug.Log($"Room add to room list room name: {room.Name}");
            }
        }
    }
}