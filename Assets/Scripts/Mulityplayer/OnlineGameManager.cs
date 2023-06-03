using System;
using System.Collections.Generic;
using Managers;
using Photon.Pun;
using Photon.Realtime;
using SpawnSystem;
using UnityEngine;

public class OnlineGameManager : MonoBehaviourPun , IPunObservable
{
    public static event Action OnConnectedToMaster;
    
    private const string GAME_STARTED_RPC = nameof(GameStarted);
    private const string COUNTDOWN_STARTED_RPC = nameof(CountdownStarted);
    private const string UPDATE_PLAYER_READY_STAT_RPC = nameof(OnPlayerSetReadyStat);
    
    [SerializeField] private float _gameCountDownTime;

    private bool _isCountingForStartGame;
    private float _timeLeftForStartGame = 0;

    private SpawnManager _spawnManager;
    private bool _isGameStarted;

    public OnlinePlayer Player { get; private set; }

    public Dictionary<int, OnlinePlayer> ConnectedPlayers { get; private set; }

    private bool _hasGameStarted;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        _spawnManager = new SpawnManager();
        ConnectedPlayers = new Dictionary<int, OnlinePlayer>();

        PhotonEventer.OnPlayerEnteredRoomEvent += PlayerEnteredRoom;
        PhotonEventer.OnPlayerLeftRoomEvent += PlayerLeftRoom;
    }

    private void Start()
    {
        _isGameStarted = false;

        // foreach (var keyValuePair in PhotonNetwork.CurrentRoom.Players)
        // {
        //     ConnectedPlayers.Add(keyValuePair.Key, new OnlinePlayer(keyValuePair.Value));
        // }
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

    private void OnDestroy()
    {
        PhotonEventer.OnPlayerEnteredRoomEvent -= PlayerEnteredRoom;
        PhotonEventer.OnPlayerLeftRoomEvent -= PlayerLeftRoom;
    }

    #region OnlineManager

#if UNITY_EDITOR
    [ContextMenu("ConnectedToMaster")]
    public void ConnectedToMaster()
    {
        PhotonNetwork.NickName = "nickname";
        Debug.Log("Player nickname is " + PhotonNetwork.NickName);
        PhotonNetwork.ConnectUsingSettings();
        
        Player = new OnlinePlayer(PhotonNetwork.LocalPlayer);
    }

#endif
    
    public void ConnectedToMaster(string nickname)
    {
        PhotonNetwork.NickName = nickname;
        Debug.Log("Player nickname is " + PhotonNetwork.NickName);
        PhotonNetwork.ConnectUsingSettings();
    }

    private void PlayerEnteredRoom(Player player)
    {
        ConnectedPlayers.Add(player.ActorNumber, new OnlinePlayer(player));
    }
    
    private void PlayerLeftRoom(Player  player)
    {
        ConnectedPlayers.Remove(player.ActorNumber);
    }

    #endregion

    #region GameManagnet

    public void StartGameCountdown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(COUNTDOWN_STARTED_RPC,
                RpcTarget.AllViaServer, _gameCountDownTime );
        }
    }
    
    public void LoadGameLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

    private void OnPlayerSetReadyStat(int playerId, bool isReady)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            object[] dataToSend = new object[]
            {
                playerId,
                isReady
            };
            
            photonView.RPC(UPDATE_PLAYER_READY_STAT_RPC, RpcTarget.AllViaServer,dataToSend);
        }
    }

    #endregion
    
    #region RPCS

    [PunRPC]
    private void UpdatePlayerReadyList(int playerId, bool isReady)
    {
        
    }
    
    [PunRPC]
    void CountdownStarted(int countdownTime)
    {
        _isCountingForStartGame = true;
        _timeLeftForStartGame = countdownTime;
    }
    
    [PunRPC]
    void GameStarted()
    {
        _hasGameStarted = true;
        _isCountingForStartGame = false;
        _isGameStarted = true;
        Debug.Log("Game Started!!! WHOW");
    }
    
    #endregion
    
    
    // void Start()
    // {
    //     if (PhotonNetwork.IsConnectedAndReady)
    //     {
    //         photonView.RPC(ASK_FOR_RANDOM_SPAWN_POINT_RPC, RpcTarget.MasterClient);
    //         if (PhotonNetwork.IsMasterClient)
    //         {
    //             startGameButtonUI.interactable = true;
    //         }
    //
    //         gameModeText.text = PhotonNetwork.CurrentRoom.CustomProperties[Constants.GAME_MODE].ToString();
    //         foreach (KeyValuePair<int, Player>
    //                      player in PhotonNetwork.CurrentRoom.Players)
    //         {
    //             if (player.Value.CustomProperties
    //                 .ContainsKey(Constants.PLAYER_STRENGTH_SCORE_PROPERTY_KEY))
    //             {
    //                 playersScoreText.text +=
    //                     player.Value.CustomProperties[Constants.PLAYER_STRENGTH_SCORE_PROPERTY_KEY]
    //                         += Environment.NewLine;
    //             }
    //         }
    //     }
    // }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}