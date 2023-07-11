using System;
using DefaultNamespace.MenuScripts;
using GarlicStudios.Online.Managers;
using UnityEngine;
using UnityEngine.Serialization;

public class OnlineMenuManager : MonoBehaviour
{
    public event Action OnReturnToMainMenu;

    [SerializeField] LobbyMenuManager _lobby;
    [SerializeField] OnlineRoomUIHandler _carSelection;

    private void Awake()
    {
        _lobby.gameObject.SetActive(true);
        _carSelection.gameObject.SetActive(false);
        _lobby.ChangeLobbyVisual(true);
    }
}
