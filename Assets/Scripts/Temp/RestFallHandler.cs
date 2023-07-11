using System;
using DefaultNamespace;
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
            if (other.gameObject.TryGetComponent(out LocalPlayer player))
            {
                RestCar(player);
            }
        }
    }
}
