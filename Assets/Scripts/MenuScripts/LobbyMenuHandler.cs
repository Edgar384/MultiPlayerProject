using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenuHandler : MonoBehaviour
{
    public event Action OnJoinedRoom;
    public event Action OnRoomCreated;
    public event Action OnLeftLobby;
    public event Action OnJoinedLobby;

    public void CreateRoom()
    {
        OnRoomCreated?.Invoke();
    }

    public void JoinRoom()
    {
        OnJoinedRoom?.Invoke();
    }
}
