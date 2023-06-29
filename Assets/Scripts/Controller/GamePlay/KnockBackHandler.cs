using System;
using System.Security.Cryptography;
using GarlicStudios.Online.Managers;
using Photon.Pun;
using UnityEngine;

namespace PG_Physics.Wheel
{
    public class KnockBackHandler : MonoBehaviourPun
    {
        private const string REGISTER_KNOCKBACK_RPC = nameof(RegisterKnockBack_RPC);
        private const string ADD_KNOCKBACK_RPC = nameof(AddKnockBack_RPC);
        private const string ON_KNOCKBACK_EVENT_RPC = nameof(OnKnockBackEvent_RPC);
        
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _knockBackForceMultiplier;
        [SerializeField] private float _magnitudeTrchholl;


        private void OnValidate()
        {
            _rigidbody ??= GetComponent<Rigidbody>();
        }
        
        [PunRPC]
        private void RegisterKnockBack_RPC(int attackPlayerId, Vector3 velocity, int hitedPlayerId)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            if (!OnlineRoomManager.ConnectedPlayers.TryGetValue(hitedPlayerId, out var player))
            {
                Debug.LogError("Can not find player");
                return;
            }
            
            photonView.RPC(ADD_KNOCKBACK_RPC,player.PhotonData,velocity);
            photonView.RPC(ON_KNOCKBACK_EVENT_RPC,RpcTarget.AllViaServer,attackPlayerId,hitedPlayerId);
        }
        
        [PunRPC]
        private void OnKnockBackEvent_RPC(int attackPlayerId,int hitedPlayerId)
        {
            Debug.Log($"{attackPlayerId} hit {hitedPlayerId} and add a knock back");
        }
        
        [PunRPC]
        private void AddKnockBack_RPC(Vector3 velocity)
        {
            _rigidbody.AddForce(Vector3.up * _knockBackForceMultiplier / 4, ForceMode.Impulse);
            _rigidbody.AddForce(velocity, ForceMode.Impulse);
            Debug.Log("Add knockBack");
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (!photonView.IsMine) return;
            
            Debug.Log(_rigidbody.velocity.magnitude);

            if(_rigidbody.velocity.magnitude < _magnitudeTrchholl) return;
            
            if (!other.gameObject.TryGetComponent<KnockBackHandler>(out var car)) return;
            
            if (!(_rigidbody.velocity.magnitude > car._rigidbody.velocity.magnitude)) return;
            
            var velocityMagnitude =  _rigidbody.velocity * _knockBackForceMultiplier;
            
            photonView.RPC(REGISTER_KNOCKBACK_RPC,RpcTarget.MasterClient,
                parameters: new object[] { photonView.Controller.ActorNumber , velocityMagnitude ,car.photonView.Controller.ActorNumber});
        }
    }
}