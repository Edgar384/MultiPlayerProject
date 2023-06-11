using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SpawnSystem
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField,ReadOnly] private int _id;
        private bool _isTaken;

        public int ID => _id;

        public bool IsTaken => _isTaken;

        private void Awake()
        {
            OnlineGameManager.RegisterSpawnPoint(this);
        }

        public void Init(int id)
        {
            _id = id;
            _isTaken = false;
        }
 
        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position, 0.5f);
        }

        public void SetSpawnPointToTaken() =>
            _isTaken  = true;

        // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        // {
        //     if (stream.IsWriting)
        //     {
        //         stream.SendNext(_isTaken);
        //     }
        //     else if(stream.IsReading)
        //     {
        //         _isTaken = (bool)stream.ReceiveNext();
        //     }
        // }
    }

}