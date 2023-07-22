using System;
using System.Collections.Generic;
using DefaultNamespace;
using Photon.Pun;
using Temp;
using UnityEngine;

namespace GamePlayLogic
{
    public class OnlineGameManager : MonoBehaviourPun
    {
        public static event Action OnEndGame;
        
        [SerializeField] private ScoreManager _scoreManager;
        [SerializeField] private RestFallHandler _restFallHandler;

        public static readonly Dictionary<int, LocalPlayer> LocalPlayers = new();
        public static bool IsGameRunning { get; private set; }
        private void Awake()
        {
            _restFallHandler.OnRestCarEvent += OnPlayerFall;
            TimeManager.OnTimeEnd += EndGame;
            IsGameRunning  = true;
        }

        private void EndGame()
        {
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(EndGame_RPC), RpcTarget.AllViaServer);
        }
        
        [PunRPC]
        private void EndGame_RPC()
        {
            IsGameRunning = false;
            Debug.Log("End Game"); 
            OnEndGame?.Invoke();
        }

        private void OnDestroy()
        {
            _restFallHandler.OnRestCarEvent -= OnPlayerFall;
            TimeManager.OnTimeEnd -= EndGame;
        }

        private void OnPlayerFall(LocalPlayer localPlayer)
        {
            _scoreManager.AddScore(localPlayer.KnockBackHandler.LeastAttackPlayerId);
        }
    }
}