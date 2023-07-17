using System;
using System.Collections.Generic;
using System.Linq;
using GarlicStudios.Online.Data;
using GarlicStudios.Online.Managers;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class OnlineRoomUIHandler : MonoBehaviour
{
    public event Action<int> OnCarSelected;

    [SerializeField] private List<PlayerInRoomUI> _playersInRoomUI;
    [SerializeField] private GameObject _carSelectionPreview;
    [SerializeField] private CarSelectionStatus[] _cars;
    [SerializeField] private Button _readyUp;
    [SerializeField] private Button _start;

    [SerializeField] private OnlineRoomManager _onlineRoomManager;

    [SerializeField] private CharacterSelectionUI[] _characters = new CharacterSelectionUI[4];
    private int _selectedCharacterIndex;

    private void OnEnable()
    {
        ResetCarsStatus();
        _carSelectionPreview.SetActive(true);
        _selectedCharacterIndex = 0;
        _cars[_selectedCharacterIndex].gameObject.SetActive(true);
        OnlineRoomManager.OnPlayerListUpdateEvent  += UpdatePlayerUI;
        CanvasManager.Instance.PlayerController.UI.Back.performed += OnlineMenuManager.Instance.ReturnToLobby;
        CanvasManager.Instance.PlayerController.UI.Confirm.performed += SelectCharacter;
        OnCarSelected += _onlineRoomManager.OnCharacterSelect;
        UpdatePlayerUI();
        _start.interactable = false;
        SetFirstSelectedObject();
    }

    private void OnDisable()
    {
        OnCarSelected -= _onlineRoomManager.OnCharacterSelect;
        CanvasManager.Instance.PlayerController.UI.Back.performed -= OnlineMenuManager.Instance.ReturnToLobby;
        CanvasManager.Instance.PlayerController.UI.Confirm.performed -= SelectCharacter;
        OnlineRoomManager.OnPlayerListUpdateEvent  -= UpdatePlayerUI;
        _carSelectionPreview.SetActive(false);
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
        
        for (int i = 0; i < numberOfPlayersInRoom; i++)
        {
            _playersInRoomUI[i].Init(playerArray[i]);
            SetPlayerUiReadyStatus(playerArray[i],playerArray[i].IsReady);
        }
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

    private void ResetCarsStatus()
    {
        for (int i = 0; i < _cars.Length; i++)
        {
            _cars[i].ChangeCarAvailability(true);
        }
    }

    public void NextCar()
    {
        _cars[_selectedCharacterIndex].gameObject.SetActive(false);
        _selectedCharacterIndex = (_selectedCharacterIndex + 1) % _cars.Length; //For making a loop
        _cars[_selectedCharacterIndex].gameObject.SetActive(true);
        CheckCarAvailability();
    }

    public void PreviousCar()
    {
        _cars[_selectedCharacterIndex].gameObject.SetActive(false);
        _selectedCharacterIndex--;
        if (_selectedCharacterIndex < 0)
            _selectedCharacterIndex += _cars.Length;
        _cars[_selectedCharacterIndex].gameObject.SetActive(true);
        CheckCarAvailability();
    }

    public void SetCarIsTaken(int carIndex, bool isAvailable)
    {
        _cars[carIndex].ChangeCarAvailability(isAvailable);
    }

    private void CheckCarAvailability()
    {
        _readyUp.interactable = _cars[_selectedCharacterIndex].CheckIfCarIsFree();
    }

    public void CancleSelect()
    {
        _cars[_selectedCharacterIndex].gameObject.SetActive(false);
    }

    public void ConfirmSelection()
    {
        _cars[_selectedCharacterIndex].ChangeCarAvailability(false);
        OnCarSelected?.Invoke(_selectedCharacterIndex);
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
                currentCharacterOnHover.ChangeCharacterAvailability(false,PhotonNetwork.NickName);

            else
                currentCharacterOnHover.ChangeCharacterAvailability(true, PhotonNetwork.NickName);
        }
    }
}