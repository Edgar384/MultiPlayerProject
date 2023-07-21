using GarlicStudios.Online.Managers;
using Photon.Pun;
using UnityEngine;

namespace PG_Physics.Wheel
{
    public class KnockBackHandler : MonoBehaviourPun
    {
        private const string ADD_KNOCKBACK_RPC = nameof(AddKnockBack_RPC);
        private const string ON_KNOCKBACK_EVENT_RPC = nameof(OnKnockBackEvent_RPC);

        [SerializeField] private ParticleSystem _particleSystem;
        
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _knockBackForceMultiplier;
        [SerializeField] private float _magnitudeTrchholl;

        [SerializeField] private float _timeToClearAttackedPlayer;
        private float _timeToClearAttackedPlayerCounter;

        public Rigidbody Rigidbody => _rigidbody;

        private int _leastAttackPlayerId;

        private bool _isKnockBackMode;

        public int LeastAttackPlayerId => _leastAttackPlayerId;

        private void Awake()
        {
            _leastAttackPlayerId = -1;
            _timeToClearAttackedPlayerCounter = _timeToClearAttackedPlayer;
            _isKnockBackMode = false;
        }

        private void OnValidate()
        {
            _rigidbody ??= GetComponent<Rigidbody>();
        }

        private void Update()
        {
            _timeToClearAttackedPlayerCounter -= Time.deltaTime;
            
            if (_timeToClearAttackedPlayerCounter <= 0)
            {
                _isKnockBackMode = false;
                _leastAttackPlayerId = -1;
                _timeToClearAttackedPlayerCounter = _timeToClearAttackedPlayer;
            }

        }

        [PunRPC]
        private void OnKnockBackEvent_RPC(int attackPlayerId,int hitedPlayerId)
        {
            Debug.Log($"{attackPlayerId} hit {hitedPlayerId} and add a knock back");
        }
        
        [PunRPC]
        private void AddKnockBack_RPC(float x,float y , float z ,int attackPlayerId)
        {
            _isKnockBackMode = true;
            Debug.Log($"Add a knock back from {attackPlayerId}");
            _rigidbody.AddForce(Vector3.up * _knockBackForceMultiplier, ForceMode.Impulse);
            _rigidbody.AddForce(new Vector3(x,y,z) * _knockBackForceMultiplier, ForceMode.Impulse);
        }
        
        [PunRPC]
        private void UpdateLastHitPlayer(int  playerId)=>
            _leastAttackPlayerId = playerId;

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.TryGetComponent<KnockBackHandler>(out var car)) return;
            
            if(_rigidbody.velocity.magnitude < _magnitudeTrchholl) return;
            
            _particleSystem.transform.position = other.contacts[0].point;
            _particleSystem.Play();
            
            if (photonView.IsMine)
            {
                int hitedPlayerId = car.photonView.Controller.ActorNumber;

                if (!OnlineRoomManager.ConnectedPlayers.TryGetValue(hitedPlayerId, out var player))
                {
                    Debug.LogError("Can not find player");
                    return;
                }
                
                if (_rigidbody.velocity.magnitude < other.rigidbody.velocity.magnitude) return;
                
                int attackPlayerId = photonView.Controller.ActorNumber;
                
                player.PhotonView.RPC(nameof(UpdateLastHitPlayer),RpcTarget.AllViaServer,attackPlayerId);
                
                if (_isKnockBackMode)
                    return;

                if(_rigidbody.velocity.magnitude < _magnitudeTrchholl) return;
                _isKnockBackMode = true;
                var velocity = _rigidbody.velocity;

                player.PhotonView.RPC(ADD_KNOCKBACK_RPC,player.PhotonData,velocity.x,velocity.y,velocity.z,attackPlayerId);
                photonView.RPC(ON_KNOCKBACK_EVENT_RPC,RpcTarget.AllViaServer,attackPlayerId,hitedPlayerId);
            }
        }
    }
}