using System;
using System.Collections.Generic;
using DefaultNamespace.SciptableObject.PlayerData;
using Photon.Pun;
using UnityEngine;

namespace DefaultNamespace.CharcterSelect
{
    [Serializable]
    public class OnlineCharacterSelect : IPunObservable
    {
        public static event Action<int> OnCharacterConfirm;
        public static event Action<int> OnCancelCharacterConfirm;
        public static event Action OnNextCharacter;
        public static event Action OnPreviosCharacter;
        
        
        [SerializeField] private List<CharacterHolder> _characterHolders = new List<CharacterHolder>();

        private bool[] _sendData;
        
        private int _currentCharacterIndex;

        public CharacterHolder CurrentCharacterHolder => _characterHolders[_currentCharacterIndex];

        public OnlineCharacterSelect()
        {
            _sendData = new bool[_characterHolders.Count];
            
            for (int i = 0; i < _sendData.Length; i++)
                _sendData[i] = true;
        }

        public bool TryToConfirmCharacter(out PlayerData playerData)
        {
            if (_characterHolders[_currentCharacterIndex].IsAvailable)
            {
                playerData = _characterHolders[_currentCharacterIndex].GetPlayerData();
                _sendData[_currentCharacterIndex] = _characterHolders[_currentCharacterIndex].IsAvailable;
                OnCharacterConfirm?.Invoke(_currentCharacterIndex);
                return true;
            }

            playerData = null;
            return false;
        }

        public void NextCharacter()
        {
            if (_currentCharacterIndex == _characterHolders.Count - 1)
                _currentCharacterIndex = 0;
            else
                _currentCharacterIndex++;
            
            OnNextCharacter?.Invoke();
        }

        public void PreviosCharacter()
        {
            if (_currentCharacterIndex == 0)
                _currentCharacterIndex = _characterHolders.Count  - 1;
            else
                _currentCharacterIndex--;
            
            OnPreviosCharacter?.Invoke();
        }

        public void CancelCharacterSelect()
        {
            _characterHolders[_currentCharacterIndex].Free();
            _sendData[_currentCharacterIndex] = _characterHolders[_currentCharacterIndex].IsAvailable;
            OnCancelCharacterConfirm?.Invoke(_currentCharacterIndex);
        }

        private void UpdateCharecterList_RPC(int id, bool isAvailable)
        {
            
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_sendData);
            }

            if (stream.IsReading)
            {
                _sendData = (bool[]) stream.ReceiveNext();
            }
        }
    }

    [Serializable]
    public class CharacterHolder
    {
        [SerializeField] private int _id;
        [SerializeField] private PlayerData _playerData;
        
        public bool IsAvailable { get; private set; }

        public int Id => _id;

        public PlayerData GetPlayerData()
        {
            if (!IsAvailable) throw new Exception("Character is not available");
            
            IsAvailable = false;
            return _playerData;
        }

        public void Free()=>
            IsAvailable = true;
    }
}