using System;
using System.Collections.Generic;
using DefaultNamespace;
using Photon.Pun;
using UnityEngine;

namespace GamePlayLogic
{
    public class ScoreManager : MonoBehaviourPun
    {
        public event Action<int> OnPlayerWon;

        private const string ADD_SCORE_TO_PLAYER = nameof(AddScoreToPlayer);
        [SerializeField] private int _numberOfPointToWin;
        
        private void Start()
        {
            foreach (var player in OnlineGameManager.LocalPlayers)
                player.Value.ScoreHandler.ResetScore();
        }


        public void AddScore(int playerId)
        {
            photonView.RPC(ADD_SCORE_TO_PLAYER, RpcTarget.AllViaServer, playerId);
        }


        [PunRPC]
        private void AddScoreToPlayer(int playerId)
        {
            if (OnlineGameManager.LocalPlayers.TryGetValue(playerId, out var player))
            {
                player.ScoreHandler.AddScore(1);
                Debug.Log($"Add score to player {playerId} new score {player.ScoreHandler.Score}");
            }
        }
    }
}