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
        [SerializeField] private ScoreManager _scoreManager;
        [SerializeField] private RestFallHandler _restFallHandler;

        public static readonly Dictionary<int, LocalPlayer> LocalPlayers = new();

        private void Awake()
        {
            _restFallHandler.OnRestCarEvent += OnPlayerFall;
        }

        private void OnPlayerFall(LocalPlayer localPlayer)
        {
            _scoreManager.AddScore(localPlayer.KnockBackHandler.LeastAttackPlayerId);
        }
        
        [PunRPC]
        public void AddLocalPlayer_RPC(LocalPlayer localPlayer)
        {
            photonView.RPC(nameof(AddLocalPlayer), RpcTarget.AllViaServer, localPlayer);
        }
        
        private void AddLocalPlayer(LocalPlayer  localPlayer)
        {
            LocalPlayers.Add(localPlayer.PlayerId, localPlayer);
        }

        private void OnDisable()
        {
            LocalPlayers.Clear();
        }
    }
}