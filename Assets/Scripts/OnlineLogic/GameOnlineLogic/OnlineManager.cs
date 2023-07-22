using System;
using System.Collections;
using GamePlayLogic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class OnlineManager : MonoBehaviourPunCallbacks
{
    public static event Action OnConnectedToMasterEvent;
    public static event Action<Player> OnMasterClientSwitchedEvent;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        PhotonNetwork.AutomaticallySyncScene  = true;
    }

    private void Start()
    {
        CanvasManager.Instance.OnlineMenuManager.OnOnlineCanvasDisabled += DisconnectPlayer;
        EnterNameHandler.OnNicknameEntered += ConnectedToMaster;
    }

    private void OnDestroy()
    {
        CanvasManager.Instance.OnlineMenuManager.OnOnlineCanvasDisabled -= DisconnectPlayer;
        EnterNameHandler.OnNicknameEntered -= ConnectedToMaster;
    }

#if UNITY_EDITOR
    [ContextMenu("ConnectedToMaster")]
    public void ConnectedToMaster()
    {
        PhotonNetwork.NickName = "nickname";
        Debug.Log("PhotonData nickname is " + PhotonNetwork.NickName);
        PhotonNetwork.ConnectUsingSettings();
    }

#endif
    
    public void ConnectedToMaster(string nickname)
    {
        PhotonNetwork.NickName = nickname;
        Debug.Log("PhotonData nickname is " + PhotonNetwork.NickName);
        PhotonNetwork.ConnectUsingSettings();
    }
    
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("<color=#00ff00>We are connected!</color>");
        PhotonNetwork.JoinLobby();
        OnConnectedToMasterEvent?.Invoke();
    }
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        OnMasterClientSwitchedEvent?.Invoke(newMasterClient);
        Debug.Log("Master client has been switched \n " +
                  "Master client is now actor number: " + newMasterClient.ActorNumber);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public void DisconnectPlayer()
    {
        if (PhotonNetwork.IsConnected == true)
        {
            Debug.Log("Starting Disconnect. . .");
            StartCoroutine(Disconnect());
        }
    }

    private IEnumerator Disconnect()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
            Debug.Log("Disconnecting. . .");
        }
        Debug.Log("DISCONNECTED!");
    }

    #region GameManagnet

    public static void LoadGameLevel()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(1);
    }
    
    
    public static void LoadLoadingLevel()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(2);
    }
    #endregion
}