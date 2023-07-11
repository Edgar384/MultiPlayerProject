using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LobbyMenuHandler : MonoBehaviour
{
    public event Action<string> OnNicknameEntered;
    public event Action<string> OnJoinedRoom;
    public event Action<string> OnRoomCreated;
    public event Action OnLeftLobby;

    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private EventSystem _eventSystem;

    [Header("Name")]
    [SerializeField] private GameObject _playerNameImage;
    [SerializeField] private TMP_InputField _playerNameInput;

    [Header("Rooom Join")]
    [SerializeField] private GameObject _joinRoomImage;

    [Header("Rooom Creation")]
    [SerializeField] private GameObject _createRoomImage;
    [SerializeField] private TMP_InputField _roomNameInput;
    
    private readonly Color grayColor = new Color(159, 159, 159, 1);
    private readonly Color normalColor = new Color(255, 255, 255, 1);
    public void CreateRoom()
    {
        OnRoomCreated?.Invoke(_roomNameInput.text);
    }

    public void JoinRoom()
    {
        OnJoinedRoom?.Invoke(_roomNameInput.text);
    }

    private void ChangeLobbyVisual(bool isNewPlayer)
    {
        if (isNewPlayer)
            ChangeToNewPlayerVisuals();

        else
            ChangeToOldPlayerVisuals();
    }

    private void ChangeToNewPlayerVisuals()
    {
        _eventSystem.firstSelectedGameObject = _playerNameImage;
    }

    private void ChangeToOldPlayerVisuals()
    {

    }
}
