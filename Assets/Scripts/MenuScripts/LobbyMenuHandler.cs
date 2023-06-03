using System;
using TMPro;
using UnityEngine;

public class LobbyMenuHandler : MonoBehaviour
{
    public event Action<string> OnJoinedRoom;
    public event Action<string> OnRoomCreated;
    public event Action OnLeftLobby;

    [SerializeField] private TMP_InputField _roomName;

    public void CreateRoom()
    {
        OnRoomCreated?.Invoke(_roomName.text);
    }

    public void JoinRoom()
    {
        OnJoinedRoom?.Invoke(_roomName.text);
    }
}
