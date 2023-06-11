using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonEventer : MonoBehaviourPunCallbacks
{
    #region Events
//may need to be ststic
    public static event Action<Player> OnPlayerEnteredRoomEvent;
    public static event Action<Player> OnPlayerLeftRoomEvent;
    public static event Action OnPlayerJoinRoomEvent;
    
    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        OnPlayerJoinRoomEvent?.Invoke();
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
    
   

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
}