using GarlicStudios.Online.Managers;
using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class OnlineMenuManager : MonoBehaviour
{
    public event Action OnOnlineCanvasDisabled;

    [SerializeField] LobbyMenuManager _lobby;
    [SerializeField] OnlineRoomUIHandler _carSelection;

    public LobbyMenuManager Lobby => _lobby;
    public OnlineRoomUIHandler Room => _carSelection;

    private void Start()
    {
        _lobby.gameObject.SetActive(false);
        _carSelection.gameObject.SetActive(false);
        CanvasManager.Instance.OnPlayerPressedPlay += TurnOnLobby;
    }

    private void OnDestroy()
    {
        CanvasManager.Instance.OnPlayerPressedPlay -= TurnOnLobby;
    }

    private void TurnOnLobby(bool isNewPlayer)
    {
        _lobby.gameObject.SetActive(true);
        _carSelection.gameObject.SetActive(false);
        _lobby.ChangeLobbyVisual(isNewPlayer);
    }

    private void TurnOnRoom()
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
        _lobby.gameObject.SetActive(false);
        _carSelection.gameObject.SetActive(false);
        CanvasManager.Instance.InputSystemUIInputModule.enabled = true;
        OnOnlineCanvasDisabled?.Invoke();
    }
}
