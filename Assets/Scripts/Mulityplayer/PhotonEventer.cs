using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonEventer : MonoBehaviourPunCallbacks
{
    #region Events
//may need to be ststic
    public event Action OnConnectedToMasterEvent;
    public event Action<List<RoomInfo>> OnRoomListUpdateEvent;
    public event Action OnCreatedRoomEvent;
    public event Action<Player> OnPlayerEnteredRoomEvent;
    public event Action<Player> OnPlayerLeftRoomEvent;
    public event Action<short,string> OnCreateRoomFailedEvent;
    
    #endregion
    
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("<color=#00ff00>We are connected!</color>");
        PhotonNetwork.JoinLobby();
        OnConnectedToMasterEvent?.Invoke();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Got room list");
        base.OnRoomListUpdate(roomList);
        OnRoomListUpdateEvent?.Invoke(roomList);
    }
    
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        OnCreatedRoomEvent?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room!");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        OnPlayerEnteredRoomEvent?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        OnPlayerLeftRoomEvent?.Invoke(otherPlayer);
       
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError("Create failed..." + Environment.NewLine + message);
        OnCreateRoomFailedEvent?.Invoke(returnCode, message);
    }

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
}