using System;
using GamePlayLogic;
using GarlicStudios.Online.Data;
using GarlicStudios.Online.Managers;
using PG_Physics.Wheel;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace DefaultNamespace
{
    public class LocalPlayer : MonoBehaviourPun , IPunInstantiateMagicCallback,IComparable<LocalPlayer> , IPunOwnershipCallbacks
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
            SetOnlinePlayer(OnlinePlayer);
            OnlinePlayer.SetPhotonView(photonView);
            OnlineGameManager.LocalPlayers.Add(photonView.Owner.ActorNumber,this);
            Debug.Log($"{gameObject.name} is instantiated with viewID {photonView.Owner.ActorNumber}");
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

        public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
        {
            throw new NotImplementedException();
        }

        public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
        {
            Debug.Log($"{OnlinePlayer.ActorNumber} is now owner of {gameObject.name} photonViewId: {photonView.ViewID}");
        }

        public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
        {
            Debug.LogError("OnOwnershipTransferFailed");
        }
    }
}