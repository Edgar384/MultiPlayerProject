using GarlicStudios.Online.Managers;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace PG_Physics.Wheel
{
    public class KnockBackHandler : MonoBehaviourPun
    {
        private const string REGISTER_KNOCKBACK_RPC = nameof(RegisterKnockBack_RPC);
        private const string ADD_KNOCKBACK_RPC = nameof(AddKnockBack_RPC);
        
        [SerializeField] private Rigidbody _rigidbody;
        [FormerlySerializedAs("_knockBackForce")] [SerializeField] private float _knockBackForceMultiplier;

        public int PlayerID => photonView.ViewID;//may need to chanage
        
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
            
            photonView.RPC(ADD_KNOCKBACK_RPC,player.PhotonData);
            
        }
        
        private void AddKnockBack_RPC(Vector3 velocity)
        {
            _rigidbody.AddForce(velocity, ForceMode.Impulse);
            Debug.Log("Add knockBack");
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent<KnockBackHandler>(out var car))
            {
                if (_rigidbody.velocity.magnitude > car._rigidbody.velocity.magnitude)
                {
                    var velocityMagnitude = (car.transform.position - transform.position).normalized * _rigidbody.velocity.magnitude * _knockBackForceMultiplier;
                   photonView.RPC(REGISTER_KNOCKBACK_RPC,RpcTarget.MasterClient,car.photonView.ViewID,velocityMagnitude,PlayerID);
                 
                }
                // else
                // {
                //     var velocity = (transform.position - car.transform.position).normalized;
                //     //velocity = new Vector3(velocity.x, 0, velocity.z);
                //
                //     RegisterKnockBack_RPC(velocity, );
                // }
            }
        }
    }
}