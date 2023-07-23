using GarlicStudios.Online.Managers;
using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class OnlineMenuManager : MonoBehaviour
{
    public static OnlineMenuManager Instance;
    public event Action OnOnlineCanvasDisabled;

    [SerializeField] LobbyMenuManager _lobby;
    [SerializeField] OnlineRoomUIHandler _carSelection;

    public LobbyMenuManager Lobby => _lobby;
    public OnlineRoomUIHandler Room => _carSelection;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _lobby.gameObject.SetActive(false);
        _carSelection.gameObject.SetActive(false);
        CanvasManager.Instance.OnPlayerPressedPlay += TurnOnLobby;
        _lobby.OnJoinedRoom += TurnOnRoom;
        _lobby.OnRoomCreated += TurnOnRoom;
    }

    private void OnDestroy()
    {
        CanvasManager.Instance.OnPlayerPressedPlay -= TurnOnLobby;
        _lobby.OnJoinedRoom -= TurnOnRoom;
        _lobby.OnRoomCreated -= TurnOnRoom;
    }

    public void TurnOnLobby(bool isNewPlayer)
    {
        _lobby.gameObject.SetActive(true);
        _carSelection.gameObject.SetActive(false);
        _lobby.ChangeLobbyVisual(isNewPlayer);
    }

    private void TurnOnRoom(string roomName)
    {
        _lobby.gameObject.SetActive(false);
        _carSelection.gameObject.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        _lobby.gameObject.SetActive(false);
        _carSelection.gameObject.SetActive(false);
        OnOnlineCanvasDisabled?.Invoke();
    }

    public void ReturnToMainMenu(CallbackContext callbackContext)
    {
        if (!_lobby.isActiveAndEnabled && !_carSelection.isActiveAndEnabled)
            return;
        CanvasManager.Instance.MenusAudioHandler.PlayButtonClick();
        _lobby.gameObject.SetActive(false);
        _carSelection.gameObject.SetActive(false);
        OnOnlineCanvasDisabled?.Invoke();
    }

    public void ReturnToLobby(CallbackContext callbackContext)
    {
        TurnOnLobby(false);
    }
}
