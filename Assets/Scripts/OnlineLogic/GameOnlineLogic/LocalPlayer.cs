using PG_Physics.Wheel;
using UnityEngine;

namespace DefaultNamespace
{
    public class LocalPlayer : MonoBehaviour
    {
        [SerializeField] private PlayerCarInput _playerCarInput;
        [SerializeField] private KnockBackHandler _knockBackHandler;
        //add ability handler

        private void OnValidate()
        {
            _knockBackHandler ??= GetComponent<KnockBackHandler>();
            _playerCarInput ??= GetComponent<PlayerCarInput>();
        }
    }
}