using System;
using DefaultNamespace.SciptableObject.PlayerData;
using Photon.Realtime;

namespace Managers
{
    public class OnlinePlayer
    {
        public event Action<int,PlayerData> OnPlayerDataChanged;

        private Player _player;
        private PlayerData _playerData;
        
        public int ActorNumber { get;}
        public bool IsMasterClient => _player.IsMasterClient;
        public string UserId { get;}
        public string NickName { get;}

        public bool IsReadyAndConnected { get; set; }

        public PlayerData PlayerData => _playerData;

        public OnlinePlayer(Player playerData)
        {
            _player = playerData;
            ActorNumber = playerData.ActorNumber;
            UserId = playerData.UserId;
            NickName = playerData.NickName;
        }

        public void SetPlayerData(PlayerData playerData)
        {
            _playerData = playerData;
            OnPlayerDataChanged?.Invoke(ActorNumber,_playerData);
        }
        

        public void SetMasterClient()
        {
            
        }
    }
}