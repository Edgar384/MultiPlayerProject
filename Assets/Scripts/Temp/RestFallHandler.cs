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
                Debug.Log("RestCar");
                var resetPos = _resetPos[UnityEngine.Random.Range(0, _resetPos.Length)];

                _respawnParticleSystems[player.OnlinePlayer.PlayerData.CharacterID].transform.position = resetPos.position;
                _respawnParticleSystems[player.OnlinePlayer.PlayerData.CharacterID].Play();
                
                if (!PhotonNetwork.IsMasterClient)
                    return;
                player.photonView.RPC("RestPlayer_RPC", player.OnlinePlayer.PhotonData,resetPos.position.x,resetPos.position.y,resetPos.position.z);
                RestCar(player);
            }
        }
    }
}