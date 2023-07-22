using System;
using DefaultNamespace;
using Photon.Pun;
using UnityEngine;

namespace Temp
{
    public class RestFallHandler : MonoBehaviourPun
    {
        [SerializeField] private ParticleSystem _fallParticleSystem;
        [SerializeField] private ParticleSystem[] _respawnParticleSystems;
        public event Action<LocalPlayer> OnRestCarEvent;

        [SerializeField] private Transform[] _resetPos;

        private void RestCar(LocalPlayer player)
        {
            OnRestCarEvent?.Invoke(player);
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out LocalPlayer player))
            {
                _fallParticleSystem.transform.position = player.transform.position;
                _fallParticleSystem.Play();
                
                if (!PhotonNetwork.IsMasterClient)
                    return;

                int index = UnityEngine.Random.Range(0, _resetPos.Length);
                
                var resetPos = _resetPos[index].position;
                
                photonView.RPC(nameof(RestPlayerVFX_RPC),RpcTarget.AllViaServer,resetPos.x,resetPos.y,resetPos.z,index);
                player.photonView.RPC("RestPlayer_RPC", player.OnlinePlayer.PhotonData,resetPos.x,resetPos.y,resetPos.z);
                RestCar(player);
            }
        }


        [PunRPC]
        private void RestPlayerVFX_RPC(float x, float y, float z,int vfxIndex)
        {
            var resetPos = new Vector3(x, y, z);
            _respawnParticleSystems[vfxIndex].transform.position = resetPos;
            _respawnParticleSystems[vfxIndex].Play();
        }

    }
}