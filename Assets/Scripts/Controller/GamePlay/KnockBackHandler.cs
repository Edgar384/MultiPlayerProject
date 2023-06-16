using System;
using GarlicStudios.Online.Managers;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace PG_Physics.Wheel
{
    public class KnockBackHandler : MonoBehaviourPun
    {
        private const string KNOCKBACK_RPC = nameof(AddKnockBack_RPC);
        
        [SerializeField] private Rigidbody _rigidbody;
        [FormerlySerializedAs("_knockBackForce")] [SerializeField] private float _knockBackForceMultiplier;

        public int PlayerID => photonView.ViewID;//may need to chanage
        
        private void OnValidate()
        {
            _rigidbody ??= GetComponent<Rigidbody>();
        }
        
        [PunRPC]
        private void AddKnockBack_RPC(int attackPlayerId, Vector3 velociety, int hitedPlayerId)
        {
            OnlineRoomManager.ConnectedPlayers.TryGetValue(hitedPlayerId, out var player);
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent<KnockBackHandler>(out var car))
            {
                if (_rigidbody.velocity.magnitude > car._rigidbody.velocity.magnitude)
                {
                    var velocityMagnitude = (car.transform.position - transform.position).normalized * _rigidbody.velocity.magnitude * _knockBackForceMultiplier;
                   photonView.RPC(KNOCKBACK_RPC,RpcTarget.MasterClient,car.photonView.ViewID,velocityMagnitude,PlayerID);
                 
                }
                // else
                // {
                //     var velociety = (transform.position - car.transform.position).normalized;
                //     //velociety = new Vector3(velociety.x, 0, velociety.z);
                //
                //     AddKnockBack_RPC(velociety, );
                // }
            }
        }
    }
}