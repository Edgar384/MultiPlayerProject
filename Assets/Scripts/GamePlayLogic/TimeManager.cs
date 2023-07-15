using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GamePlayLogic
{
    public class TimeManager : MonoBehaviourPun , IPunObservable
    {

        [SerializeField] private float _time;

        private bool _isTimeEnded;
        
        public static float TimeGame { get; private set; }

        private void Awake()
        {
            _isTimeEnded = false;
            TimeGame = _time;
        }

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            TimeGame -= Time.deltaTime;
            if (TimeGame <= 0 && !_isTimeEnded)
            {
                _isTimeEnded = true;
                TimeGame = 0;
                PhotonNetwork.RaiseEvent(Consts.EndGameCode, null, RaiseEventOptions.Default, SendOptions.SendReliable);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (PhotonNetwork.IsMasterClient)
                stream.SendNext(TimeGame);
            if (stream.IsReading)
                TimeGame = (float)stream.ReceiveNext();
        }
    }
}