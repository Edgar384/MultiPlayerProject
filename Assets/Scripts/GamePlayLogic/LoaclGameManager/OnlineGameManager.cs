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

        private void OnDisable()
        {
            LocalPlayers.Clear();
        }
    }
}