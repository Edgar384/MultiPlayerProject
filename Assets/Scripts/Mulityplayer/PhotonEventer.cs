using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonEventer : MonoBehaviourPunCallbacks
{
    #region Events
//may need to be ststic
    public static event Action OnConnectedToMasterEvent;
    public static event Action<Player> OnPlayerEnteredRoomEvent;
    public static event Action<Player> OnPlayerLeftRoomEvent;
    
    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("<color=#00ff00>We are connected!</color>");
        PhotonNetwork.JoinLobby();
        OnConnectedToMasterEvent?.Invoke();
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
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        Debug.Log("Masterclient has been switched \n " +
                  "Masterclient is now actor number: " + newMasterClient.ActorNumber);
    }

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
}