using System;
using DefaultNamespace;
using DefaultNamespace.SciptableObject.PlayerData;
using GamePlayLogic;
using Photon.Pun;
using Photon.Realtime;

namespace GarlicStudios.Online.Data
{
    public class OnlinePlayer
    {
        public event Action<int,PlayerData> OnPlayerDataChanged;
        public event  Action<int,bool> OnPlayerReadyChanged;

        private readonly Player _photonData;
        
        private PlayerData _playerData;

        private  PhotonView _photonView;
        
        public bool IsMasterClient => _photonData.IsMasterClient;
        public int ActorNumber { get;}
        public string UserId { get;}
        public string NickName { get;}
        public bool IsReady { get; private set; }

        public PhotonView PhotonView => _photonView;

        public Player PhotonData => _photonData;

        public PlayerData PlayerData => _playerData;

        public OnlinePlayer(Player photonDataData)
        {
            _photonData = photonDataData;
            ActorNumber = photonDataData.ActorNumber;
            UserId = photonDataData.UserId;
            NickName = photonDataData.NickName;
            IsReady = false;
            
        }

        public void SetPlayerData(PlayerData playerData)
        {
            _playerData = playerData;
            OnPlayerDataChanged?.Invoke(ActorNumber,_playerData);
        }

        public void SetMasterClient()
        {
            
        }

        public void SetPhotonView(PhotonView photonView)
        {
            _photonView = photonView;
        }

        public void SetReadyStatus(bool isReady)
        {
            IsReady = isReady;
        }
    }
}