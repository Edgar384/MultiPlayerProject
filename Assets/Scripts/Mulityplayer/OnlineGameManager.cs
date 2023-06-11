using System;
using Photon.Pun;
using Photon.Realtime;
using SpawnSystem;
using UnityEditor;
using UnityEngine;

public class OnlineGameManager : MonoBehaviourPunCallbacks
{
    public static event Action OnConnectedToMasterEvent;
    public static event Action<Player> OnMasterClientSwitchedEvent;

    private const string GAME_STARTED_RPC = nameof(GameStarted);
    private const string COUNTDOWN_STARTED_RPC = nameof(CountdownStarted);
    
    [SerializeField] private float _timeLeftForStartGame = 0;

    private bool _isCountingForStartGame;

    private SpawnManager _spawnManager;

    private bool _isGameStarted;

    private void Awake()
    {
        
        //PlayerSettings.fullScreenMode = FullScreenMode.Windowed;

        DontDestroyOnLoad(gameObject);

        _spawnManager = new SpawnManager();
    }

    private void Start()
    {
        _isGameStarted = false;
    }

    private void Update()
    {
        if (!_isGameStarted)
        {
            if (_isCountingForStartGame)
            {
                _timeLeftForStartGame -= Time.deltaTime;
                if (_timeLeftForStartGame <= 0)
                {
                    _isCountingForStartGame = false;
                    if (PhotonNetwork.IsMasterClient)
                    {
                        photonView.RPC(GAME_STARTED_RPC, RpcTarget.AllViaServer);
                    }
                }
            }  
        }
    }

#if UNITY_EDITOR
    [ContextMenu("ConnectedToMaster")]
    public void ConnectedToMaster()
    {
        PhotonNetwork.NickName = "nickname";
        Debug.Log("Player nickname is " + PhotonNetwork.NickName);
        PhotonNetwork.ConnectUsingSettings();
    }

#endif
    
    public void ConnectedToMaster(string nickname)
    {
        PhotonNetwork.NickName = nickname;
        Debug.Log("Player nickname is " + PhotonNetwork.NickName);
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
    
    #region GameManagnet

    public void StartGameCountdown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(COUNTDOWN_STARTED_RPC,
                RpcTarget.AllViaServer);
        }
    }
    
    public void LoadGameLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

    #endregion
    
    #region RPCS

    [PunRPC]
    void CountdownStarted()
    {
        _isCountingForStartGame = true;
    }
    
    [PunRPC]
    void GameStarted()
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
}