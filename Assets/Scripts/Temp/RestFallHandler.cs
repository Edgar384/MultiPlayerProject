using System;
using DefaultNamespace;
using Photon.Pun;
using UnityEngine;

namespace Temp
{
    public class RestFallHandler : MonoBehaviourPun
    {
        public event Action<LocalPlayer> OnRestCarEvent;

        [SerializeField] private Transform _resetPos;

        private void RestCar(LocalPlayer player)
        {
            OnRestCarEvent?.Invoke(player);
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            if (other.gameObject.TryGetComponent(out LocalPlayer player))
            {
                Debug.Log("RestCar");
                player.photonView.RPC("RestPlayer_RPC", player.OnlinePlayer.PhotonData,_resetPos.position.x,_resetPos.position.y,_resetPos.position.z);
                RestCar(player);
            }
        }
    }
}