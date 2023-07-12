using System;
using DefaultNamespace;
using Photon.Pun;
using UnityEngine;

namespace Temp
{
    public class RestFallHandler : MonoBehaviour
    {
        public event Action<LocalPlayer> OnRestCarEvent;

        [SerializeField] private Transform _resetPos;

        private void RestCar(LocalPlayer player)
        {
            OnRestCarEvent?.Invoke(player);
            player.transform.position = _resetPos.position;
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            if (other.gameObject.TryGetComponent(out LocalPlayer player))
            {
                Debug.Log("RestCar");
                player.KnockBackHandler.Rigidbody.velocity = Vector3.zero;
                player.transform.rotation = Quaternion.Euler(Vector3.zero);
                RestCar(player);
            }
        }
    }
}