﻿using GamePlayLogic;
using GarlicStudios.Online.Data;
using PG_Physics.Wheel;
using Photon.Pun;
using UnityEngine;

namespace DefaultNamespace
{
    public class LocalPlayer : MonoBehaviourPun , IPunInstantiateMagicCallback
    {
        [SerializeField] private PlayerCarInput _playerCarInput;
        [SerializeField] private KnockBackHandler _knockBackHandler; 
        //add ability handler
        public OnlinePlayer OnlinePlayer { get; private set; }
        
        public ScoreHandler ScoreHandler { get; private set; }

        public int PlayerId => OnlinePlayer.ActorNumber;

        public KnockBackHandler KnockBackHandler => _knockBackHandler;

        private void Awake()
        {
            ScoreHandler = new ScoreHandler();
        }

        public void SetOnlinePlayer(OnlinePlayer onlinePlayer)
        {
            OnlinePlayer = onlinePlayer;
        }

        private void OnValidate()
        {
            _knockBackHandler ??= GetComponent<KnockBackHandler>();
            _playerCarInput ??= GetComponent<PlayerCarInput>();
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            Debug.Log($"{gameObject.name} is instantiated with viewID {photonView.ViewID}");
            OnlineGameManager.LocalPlayers.Add(photonView.ViewID,this);
        }
    }
}