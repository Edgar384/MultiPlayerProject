using System;
using System.Collections.Generic;
using System.Linq;
using GarlicStudios.Online.Data;
using GarlicStudios.Online.Managers;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OnlineRoomUIHandler : MonoBehaviour
{
    public event Action<int> OnCarSelected;

    [SerializeField] private List<PlayerInRoomUI> _playersInRoomUI;
    [SerializeField] GameObject _carSelectionPreview;
    [SerializeField] CarSelectionStatus[] _cars;
    [FormerlySerializedAs("_selecteCarButton")] [SerializeField] Button _readyUp;
    [SerializeField] Button _start;

    [SerializeField] private OnlineRoomManager _onlineRoomManager;
    
    private int _selectedCharacterIndex;

    private void OnEnable()
    {
        ResetCarsStatus();
        _carSelectionPreview.SetActive(true);
        _selectedCharacterIndex = 0;
        OnlineRoomManager.OnPlayerListUpdateEvent  += UpdatePlayerUI;
        OnCarSelected += _onlineRoomManager.OnCharacterSelect;
        UpdatePlayerUI();
        _start.interactable = false;
    }

    private void OnDisable()
    {
        OnCarSelected -= _onlineRoomManager.OnCharacterSelect;
        OnlineRoomManager.OnPlayerListUpdateEvent  -= UpdatePlayerUI;
        _carSelectionPreview.SetActive(false);
    }

    private void Update()
    {
        if (_onlineRoomManager.IsAllReady && PhotonNetwork.IsMasterClient)
        {
            _start.interactable = true;
            Debug.Log("Ready to start the game!!!");
        }
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
        int numberOfPlayerUIElements = _playersInRoomUI.Count;

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
            _cars[i].ChangeCarAvailability(false);
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

    private void CheckCarAvailability()
    {
        if(_cars[_selectedCharacterIndex].CheckIfCarIsFree())
            _readyUp.interactable = true;

        else
            _readyUp.interactable=false;
    }

    public void CancleSelect()
    {
        _cars[_selectedCharacterIndex].gameObject.SetActive(false);
    }

    public void ConfirmSelection()
    {
        _cars[_selectedCharacterIndex].ChangeCarAvailability(true);
        OnCarSelected?.Invoke(_selectedCharacterIndex);
    }
}