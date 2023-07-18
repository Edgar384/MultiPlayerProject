using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GarlicStudios.Online.Data;
using GarlicStudios.Online.Managers;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class OnlineRoomUIHandler : MonoBehaviour
{
    public event Action<int,bool> OnCharacterSelected;
    public event Action OnEnteredRoom;

    [SerializeField] private List<PlayerInRoomUI> _playersInRoomUI;
    [SerializeField] private CarPreviewHandler _carPreviewHandler;
    [SerializeField] private Button _readyUp;
    [SerializeField] private Button _start;

    [SerializeField] private OnlineRoomManager _onlineRoomManager;

    [SerializeField] private CharacterSelectionUI[] _characters = new CharacterSelectionUI[4];
    private int _selectedCharacterIndex;

    private void OnEnable()
    {
        _selectedCharacterIndex = 0;
        SetFirstSelectedObject();
        OnlineRoomManager.OnPlayerListUpdateEvent  += UpdatePlayerUI;
        CanvasManager.Instance.PlayerController.UI.Back.performed += OnlineMenuManager.Instance.ReturnToLobby;
        CanvasManager.Instance.PlayerController.UI.Confirm.performed += SelectCharacter;
        CanvasManager.Instance.PlayerController.UI.Navigate.performed += ChangeCarPreviewCallBack;
        OnCharacterSelected += _onlineRoomManager.OnCharacterSelect;
        StartCoroutine(ChangeCarPreviewOnEnter());
        _start.interactable = false;
    }

    private void OnDisable()
    {
        OnCharacterSelected -= _onlineRoomManager.OnCharacterSelect;
        CanvasManager.Instance.PlayerController.UI.Back.performed -= OnlineMenuManager.Instance.ReturnToLobby;
        CanvasManager.Instance.PlayerController.UI.Confirm.performed -= SelectCharacter;
        CanvasManager.Instance.PlayerController.UI.Navigate.performed -= ChangeCarPreviewCallBack;
        OnlineRoomManager.OnPlayerListUpdateEvent  -= UpdatePlayerUI;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && OnlineRoomManager.IsAllReady)
            _start.interactable = true;
    }

    public void AddPlayer(OnlinePlayer onlinePlayer)
    {
        foreach (var playerInRoomUI in _playersInRoomUI)
        {
            if (playerInRoomUI.isActiveAndEnabled) 
                continue;
            
            playerInRoomUI.gameObject.SetActive(true);
            playerInRoomUI.Init(onlinePlayer);
            break;
        }
    }

    private void UpdatePlayerUI()
    {
        int numberOfPlayersInRoom = OnlineRoomManager.ConnectedPlayers.Count;
        var playerArray = OnlineRoomManager.ConnectedPlayers.Values.ToArray();

        foreach (var selectionUI in _characters)
            selectionUI.ChangeCharacterAvailability(true,"");

        for (int i = 0; i < numberOfPlayersInRoom; i++)
        {
            if (playerArray[i].PlayerData != null)
                _characters[playerArray[i].PlayerData.CharacterID].ChangeCharacterAvailability(false, playerArray[i].PhotonData.NickName);
        }

        SetFirstSelectedObject();
    }
    
    private void SetPlayerUiReadyStatus(OnlinePlayer player,bool  isReady)
    {
        foreach (var playerInRoomUI in _playersInRoomUI)
        {
            if (playerInRoomUI.ID == player.ActorNumber)
            {
                playerInRoomUI.SetReadyStatus(isReady);
                break;
            }
        }
    }

    private void SetFirstSelectedObject()
    {
        for (int i = 0; i < _characters.Length; i++)
        {
            if (_characters[i].CheckIfCharacterIsFree())
            {
                CanvasManager.Instance.EventSystem.SetSelectedGameObject(_characters[i].gameObject);
                return;
            }
        }
    }

    private void SelectCharacter(CallbackContext callbackContext)
    {
        if(CanvasManager.Instance.EventSystem.currentSelectedGameObject.TryGetComponent<CharacterSelectionUI>(out CharacterSelectionUI currentCharacterOnHover))
        {
            if (currentCharacterOnHover.CheckIfCharacterIsFree())
            {
                currentCharacterOnHover.ChangeCharacterAvailability(false,PhotonNetwork.NickName);
                OnCharacterSelected?.Invoke(currentCharacterOnHover.CharacterID,true);
            }

            else
            {
                currentCharacterOnHover.ChangeCharacterAvailability(true, PhotonNetwork.NickName);
                OnCharacterSelected?.Invoke(currentCharacterOnHover.CharacterID,false);
            }
        }
    }

    private void PlayerEnterRoomCharactersRefresh(bool isReady,int playerID, string playerNickname)
    {
        if (!isReady)
        {
            _characters[playerID].ChangeCharacterAvailability(false, playerNickname);
        }
    }

    private void ChangeCarPreviewCallBack(CallbackContext callbackContext)
    {
        StartCoroutine(ChangeCarPreview());
    }

    private IEnumerator ChangeCarPreview()
    {
        yield return new WaitForEndOfFrame();
        if (CanvasManager.Instance.EventSystem.currentSelectedGameObject != _characters[_selectedCharacterIndex].gameObject)
        {
            CanvasManager.Instance.EventSystem.currentSelectedGameObject.TryGetComponent<CharacterSelectionUI>(out CharacterSelectionUI currentCharacterOnHover);
            _selectedCharacterIndex = currentCharacterOnHover.CharacterID;
            _carPreviewHandler.ChangeCarPreview(_selectedCharacterIndex);
        }
    }

    private IEnumerator ChangeCarPreviewOnEnter()
    {
        yield return new WaitForEndOfFrame();
        if (CanvasManager.Instance.EventSystem.currentSelectedGameObject == _characters[_selectedCharacterIndex].gameObject)
        {
            CanvasManager.Instance.EventSystem.currentSelectedGameObject.TryGetComponent<CharacterSelectionUI>(out CharacterSelectionUI currentCharacterOnHover);
            _selectedCharacterIndex = currentCharacterOnHover.CharacterID;
            _carPreviewHandler.ChangeCarPreview(_selectedCharacterIndex);
        }
    }
}