using System;
using System.Collections.Generic;
using DefaultNamespace;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Temp;
using UnityEngine;

namespace GamePlayLogic
{
    public class OnlineGameManager : MonoBehaviourPun
    {
        [SerializeField] private ScoreManager _scoreManager;
        [SerializeField] private RestFallHandler _restFallHandler;

        public static readonly Dictionary<int, LocalPlayer> LocalPlayers = new();

        private void Awake()
        {
            _restFallHandler.OnRestCarEvent += OnPlayerFall;
            PhotonNetwork.NetworkingClient.EventReceived += EndGame;
        }

        private void OnDestroy()
        {
            PhotonNetwork.NetworkingClient.EventReceived += EndGame;
        }

        private void EndGame(EventData obj)
        {
            if (obj.Code == Consts.EndGameCode)
                Debug.Log("End Game");
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