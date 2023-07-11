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
        
        private Dictionary<int, LocalPlayer> Players { get; set; }
        
        private void Start()
        {
            Players = OnlineGameManager.LocalPlayers;
        }


        public void AddScore(int playerId)
        {
            photonView.RPC(ADD_SCORE_TO_PLAYER, RpcTarget.AllViaServer, playerId);
        }


        [PunRPC]
        private void AddScoreToPlayer(int playerId)
        {
            if (Players.TryGetValue(playerId, out var player))
                player.ScoreHandler.AddScore(1);

            if (!PhotonNetwork.IsMasterClient) return;
            
            if(CheckIfAPlayerWon(out var winnerId))
                OnPlayerWon?.Invoke(winnerId);
        }
        
        
        private bool CheckIfAPlayerWon(out int playerId)
        {
            //go over all the Players and check if one of the player get the _numberOfPointToWin
            foreach (var player in Players)
            {
                if (player.Value.ScoreHandler.Score >= _numberOfPointToWin)
                {
                    playerId = player.Key;
                    return true;
                }
            }
            playerId = -1;
            return  false;
        }
    }
}