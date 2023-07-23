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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out LocalPlayer player))
            {
                _fallParticleSystem.transform.position = player.transform.position;
                _fallParticleSystem.Play();
                GameplayAudioHandler.Instace.PlaySplashSound();
                
                if (!PhotonNetwork.IsMasterClient)
                    return;

                int index = UnityEngine.Random.Range(0, _resetPos.Length);
                
                var resetPos = _resetPos[index].position;
                
                photonView.RPC(nameof(RestPlayerVFX_RPC),RpcTarget.AllViaServer,index);
                player.photonView.RPC("RestPlayer_RPC", player.OnlinePlayer.PhotonData,resetPos.x,resetPos.y,resetPos.z);
                OnRestCarEvent?.Invoke(player);
            }
        }


        [PunRPC]
        private void RestPlayerVFX_RPC(int vfxIndex)
        {
            _respawnParticleSystems[vfxIndex].transform.position = _resetPos[vfxIndex].position;
            _respawnParticleSystems[vfxIndex].Play();
        }

    }
}