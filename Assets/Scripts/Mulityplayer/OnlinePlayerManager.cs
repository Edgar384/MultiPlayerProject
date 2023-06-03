using System;
using Photon.Pun;
using UnityEngine;

namespace Managers
{
    public class OnlinePlayerManager : MonoBehaviourPun
    {
        [SerializeField] private CharacterSelectionMenuHandler _characterSelectionUI;
        
        private void Awake()
        {
            OnlineGameManager.OnPlayerEnteredRoomEvent += OnPlayerJoined;
            OnlineGameManager.OnPlayerLeftRoomEvent += OnPlayerLeftRoom;
        }

        private void OnPlayerLeftRoom(OnlinePlayer obj)
        {
            _characterSelectionUI.AddPlayer(obj);
        }

        private void OnPlayerJoined(OnlinePlayer obj)
        {
            throw new NotImplementedException();
        }
    }
}