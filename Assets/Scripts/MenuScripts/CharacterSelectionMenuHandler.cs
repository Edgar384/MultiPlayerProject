using System;
using System.Collections.Generic;
using Managers;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionMenuHandler : MonoBehaviour
{
    public event Action<CarSelectionStatus> OnCarSelected;

    [SerializeField] private List<PlayerInRoomUI> _playersInRoomUI;
    [SerializeField] GameObject _carSelectionPreview;
    [SerializeField] CarSelectionStatus[] _cars;
    [SerializeField] Button _selecteCarButton;
    
    private int _selectedCharacterIndex;

    private void OnEnable()
    {
        ResetCarsStatus();
        _carSelectionPreview.SetActive(true);
        _selectedCharacterIndex = 0;

        OnlineGameManager.OnPlayerEnteredRoomEvent += AddPlayer;
    }

    private void OnDisable()
    {
        _carSelectionPreview.SetActive(false);
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
            _selecteCarButton.interactable = true;

        else
            _selecteCarButton.interactable=false;
    }

    public void CancleSelect()
    {
        _cars[_selectedCharacterIndex].gameObject.SetActive(false);
    }

    public void ConfirmSelection()
    {
        _cars[_selectedCharacterIndex].ChangeCarAvailability(true);
        OnCarSelected?.Invoke(_cars[_selectedCharacterIndex]);
    }
}
