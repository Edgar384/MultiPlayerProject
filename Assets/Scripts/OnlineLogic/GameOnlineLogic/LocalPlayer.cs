using GamePlayLogic;
using GarlicStudios.Online.Data;
using GarlicStudios.Online.Managers;
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
        
        [PunRPC]
        private void RestPlayer_RPC(Vector3 pos)
        {
            transform.position = pos;
            _knockBackHandler.Rigidbody.velocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            OnlinePlayer = OnlineRoomManager.ConnectedPlayers[photonView.Owner.ActorNumber];
            Debug.Log($"{gameObject.name} is instantiated with viewID {photonView.Owner.ActorNumber}");
            OnlineGameManager.LocalPlayers.Add(photonView.Owner.ActorNumber,this);
        }
    }
}