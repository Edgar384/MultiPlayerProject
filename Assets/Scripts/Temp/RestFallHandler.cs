using DefaultNamespace;
using UnityEngine;

public class RestFallHandler : MonoBehaviour
{
    [SerializeField] private Transform _resetPos;

    private void RestCar(LocalPlayer player)
    {
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
