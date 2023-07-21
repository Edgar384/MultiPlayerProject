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

        private void Awake()
        {
            _restFallHandler.OnRestCarEvent += OnPlayerFall;
            TimeManager.OnTimeEnd += EndGame;
        }

        private void OnDestroy()
        {
        }

        private void EndGame()
        {
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(EndGame_RPC), RpcTarget.AllViaServer);
        }
        
        [PunRPC]
        private void EndGame_RPC()
        {
            Debug.Log("End Game"); 
            OnEndGame?.Invoke();
        }

        private void OnPlayerFall(LocalPlayer localPlayer)
        {
            _scoreManager.AddScore(localPlayer.KnockBackHandler.LeastAttackPlayerId);
        }

        private void OnDisable()
        {
            LocalPlayers.Clear();
        }
        
    }
}