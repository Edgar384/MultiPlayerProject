using System;
using System.Collections;
using GarlicStudios.Online.Managers;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineManager : MonoBehaviourPunCallbacks
{
    public static OnlineManager Instance;
    public static event Action OnConnectedToMasterEvent;
    public static event Action<Player> OnMasterClientSwitchedEvent;

    private const string COUNTDOWN_STARTED_RPC = nameof(CountdownStarted);
    
    [SerializeField] private float _timeLeftForStartGame = 0;
    
    private bool _isCountingForStartGame;
    

    private bool _isGameStarted;

    private void Awake()
    {
        
        if(Instance is null)
        {
           DontDestroyOnLoad(gameObject);
           Instance = this;
        }
        else
            Destroy(gameObject);
        PhotonNetwork.AutomaticallySyncScene  = true;
    }

    private void Start()
    {
        _isGameStarted = false;
        CanvasManager.Instance.OnlineMenuManager.OnOnlineCanvasDisabled += DisconnectPlayer;
        EnterNameHandler.OnNicknameEntered += ConnectedToMaster;
    }

    private void OnDestroy()
    {
        CanvasManager.Instance.OnlineMenuManager.OnOnlineCanvasDisabled -= DisconnectPlayer;
        EnterNameHandler.OnNicknameEntered -= ConnectedToMaster;
    }

    // private void Update()
    // {
    //     if (!_isGameStarted)
    //     {
    //         if (_isCountingForStartGame)
    //         {
    //             _timeLeftForStartGame -= Time.deltaTime;
    //             if (_timeLeftForStartGame <= 0)
    //             {
    //                 _isCountingForStartGame = false;
    //                 if (PhotonNetwork.IsMasterClient)
    //                 {
    //                     photonView.RPC(GAME_STARTED_RPC, RpcTarget.AllViaServer);
    //                 }
    //             }
    //         }  
    //     }
    // }

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

    public void StartGameCountdown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(COUNTDOWN_STARTED_RPC,
                RpcTarget.AllViaServer);
        }
    }
    
    public static void LoadGameLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
    
    
    public static void LoadLoadingLevel()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(2);
    }
    #endregion
    
    #region RPCS

    [PunRPC]
    private void CountdownStarted()
    {
        _isCountingForStartGame = true;
    }
    
    [PunRPC]
    private void GameStarted()
    {
        _isCountingForStartGame = false;
        _isGameStarted = true;
        Debug.Log("Game Started!!! WHOW");
    }
    
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_timeLeftForStartGame);
        }
        else if (stream.IsReading)
        {
            _timeLeftForStartGame = (float)stream.ReceiveNext();
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        OnlineRoomManager.ClearData();
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(1))
        {
            OnlineManager.Instance = null;
            Destroy(this.gameObject);
            SceneManager.LoadScene(0);
        }
    }
}