using System;
using UnityEngine;

namespace PG_Physics.Wheel
{
    public class KnockBackHandler : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _knockBackForce;

        private void OnValidate()
        {
            _rigidbody ??= GetComponent<Rigidbody>();
        }

        private void AddKnockBack(Vector3 dir, float velociety)
        {
            _rigidbody.AddForce(Vector3.up * 4 ,ForceMode.Impulse);
            _rigidbody.AddForce(dir * velociety * _knockBackForce ,ForceMode.Impulse);
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent<KnockBackHandler>(out var car))
            {
                if (_rigidbody.velocity.magnitude > car._rigidbody.velocity.magnitude)
                {
                    var dir = (car.transform.position - transform.position).normalized;
                    //dir = new Vector3(dir.x, 0, dir.z);
                 
                    car.AddKnockBack(dir, _rigidbody.velocity.magnitude);
                }
                // else
                // {
                //     var dir = (transform.position - car.transform.position).normalized;
                //     //dir = new Vector3(dir.x, 0, dir.z);
                //
                //     AddKnockBack(dir, );
                // }
            }
        }
    }
}