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

        [SerializeField] private float _timeToClearAttackedPlayer;
        private float _timeToClearAttackedPlayerCounter;

        public Rigidbody Rigidbody => _rigidbody;

        private int _leastAttackPlayerId;

        public int LeastAttackPlayerId => _leastAttackPlayerId;

        private void Awake()
        {
            _leastAttackPlayerId = -1;
        }

        private void OnValidate()
        {
            _rigidbody ??= GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_leastAttackPlayerId == -1)
            {
                _timeToClearAttackedPlayerCounter = _timeToClearAttackedPlayer;
                return;
            }
            
            _timeToClearAttackedPlayerCounter -= Time.deltaTime;
            
            if (_timeToClearAttackedPlayerCounter <= 0)
            {
                //_leastAttackPlayerId = -1;
                _timeToClearAttackedPlayerCounter = _timeToClearAttackedPlayer;
            }

        }

        [PunRPC]
        private void RegisterKnockBack_RPC(int attackPlayerId,float x , float y ,float z, int hitedPlayerId)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            if (!OnlineRoomManager.ConnectedPlayers.TryGetValue(hitedPlayerId, out var player))
            {
                Debug.LogError("Can not find player");
                return;
            }
            
            photonView.RPC(ADD_KNOCKBACK_RPC,player.PhotonData,x,y,z,attackPlayerId);
            photonView.RPC(ON_KNOCKBACK_EVENT_RPC,RpcTarget.AllViaServer,attackPlayerId,hitedPlayerId);
        }
        
        [PunRPC]
        private void OnKnockBackEvent_RPC(int attackPlayerId,int hitedPlayerId)
        {
            Debug.Log($"{attackPlayerId} hit {hitedPlayerId} and add a knock back");
        }
        
        [PunRPC]
        private void AddKnockBack_RPC(float x,float y , float z ,int attackPlayerId)
        {
            Debug.Log($"Add a knock back from {attackPlayerId}");
            photonView.RPC(nameof(UpdateLastHitPlayer),RpcTarget.AllViaServer,attackPlayerId);
            _rigidbody.AddForce(Vector3.up * _knockBackForceMultiplier, ForceMode.Impulse);
            _rigidbody.AddForce(new Vector3(x,y,z) * _knockBackForceMultiplier, ForceMode.Impulse);
        }
        
        [PunRPC]
        private void UpdateLastHitPlayer(int  playerId)=>
            _leastAttackPlayerId = playerId;

        private void OnCollisionEnter(Collision other)
        {
            if (!photonView.IsMine) return;
            
            if(_rigidbody.velocity.magnitude < _magnitudeTrchholl) return;
            
            if (!other.gameObject.TryGetComponent<KnockBackHandler>(out var car)) return;
            
            if (_rigidbody.velocity.magnitude < car._rigidbody.velocity.magnitude) return;

            var velocity = _rigidbody.velocity;
            photonView.RPC(REGISTER_KNOCKBACK_RPC,RpcTarget.MasterClient,
                parameters: new object[] { photonView.Controller.ActorNumber , velocity.x,velocity.y,velocity.z ,car.photonView.Controller.ActorNumber});
        }
    }
}