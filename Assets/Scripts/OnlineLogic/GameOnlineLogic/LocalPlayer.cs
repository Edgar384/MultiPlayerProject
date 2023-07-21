﻿using System;
using GamePlayLogic;
using GarlicStudios.Online.Data;
using GarlicStudios.Online.Managers;
using PG_Physics.Wheel;
using Photon.Pun;
using UnityEngine;

namespace DefaultNamespace
{
    public class LocalPlayer : MonoBehaviourPun , IPunInstantiateMagicCallback,IComparable<LocalPlayer>
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
        
        [PunRPC]
        private void RestPlayer_RPC(float x,float y, float z)
        {
            transform.position = new Vector3(x,y,z);
            _knockBackHandler.Rigidbody.velocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            OnlinePlayer = OnlineRoomManager.ConnectedPlayers[photonView.Owner.ActorNumber];
            OnlinePlayer.SetPhotonView(photonView);
            Debug.Log($"{gameObject.name} is instantiated with viewID {photonView.Owner.ActorNumber}");
            OnlineGameManager.LocalPlayers.TryAdd(photonView.Owner.ActorNumber, this);
        }

        public int CompareTo(LocalPlayer other)
        {
            if (other == null)
                return -1;

            if (ScoreHandler.Score > other.ScoreHandler.Score)
                return -1;
            if (ScoreHandler.Score < other.ScoreHandler.Score)
                return 1;
            return 0;
        }
    }
}