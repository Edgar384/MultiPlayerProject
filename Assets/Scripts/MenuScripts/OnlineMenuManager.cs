using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineMenuManager : MonoBehaviour
{

    public event Action OnReturnToMainMenu;

    [HideInInspector] public static OnlineMenuManager Instance;

    [Header("Canvases")]
    [SerializeField] EnterNameHandler _enterNameMenu;
    [SerializeField] LobbyMenuHandler _lobbyMenu;
    [SerializeField] RoomMenuHandler _roomMenuHandler;
    [SerializeField] CharacterSelectionMenuHandler _characterSelectionMenu;

    public CharacterSelectionMenuHandler CharacterSelectionMenu => _characterSelectionMenu;

    private void OnEnable()
    {
        ChangeEnterNameCanvasStatus(true);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RegisterEvents();
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        MainMenuManager.Instance.OnPlayerWantToPlay += ChangeEnterNameCanvasStatus;
        _enterNameMenu.OnNicknameEntered += MoveToLobbyMenu;
        _lobbyMenu.OnJoinedLobby += MoveToRoomMenu;
        _lobbyMenu.OnRoomCreated += MoveToRoomMenu;

    }

    private void UnregisterEvents()
    {
        MainMenuManager.Instance.OnPlayerWantToPlay -= ChangeEnterNameCanvasStatus;
        _enterNameMenu.OnNicknameEntered -= MoveToLobbyMenu;
        _lobbyMenu.OnJoinedLobby -= MoveToRoomMenu;
        _lobbyMenu.OnRoomCreated -= MoveToRoomMenu;
    }

    public void MoveToMainMenu()
    {
        ChangeEnterNameCanvasStatus(false);
        ChangeLobbyCanvasStatus(false);
        ChangeRoomCanvasStatus(false);
        ChangeCharacterSelectioStatus(false);
        OnReturnToMainMenu?.Invoke();
        MainMenuManager.Instance.ReturnToMainMenu(false);
    }

    private void MoveToLobbyMenu(string nickname)
    {
        ChangeEnterNameCanvasStatus(false);
        ChangeLobbyCanvasStatus(true);
    }

    private void MoveToRoomMenu()
    {
        ChangeEnterNameCanvasStatus(false);
        ChangeLobbyCanvasStatus(true);
    }

    private void ChangeEnterNameCanvasStatus(bool _toActivate)
    {
        _enterNameMenu.gameObject.SetActive(_toActivate);
    }

    private void ChangeLobbyCanvasStatus(bool _toActivate)
    {
        _lobbyMenu.gameObject.SetActive(_toActivate);
    }

    private void ChangeRoomCanvasStatus(bool _toActivate)
    {
        _roomMenuHandler.gameObject.SetActive(_toActivate);
    }

    private void ChangeCharacterSelectioStatus(bool _toActivate)
    {
        _characterSelectionMenu.gameObject.SetActive(_toActivate);
    }
}
