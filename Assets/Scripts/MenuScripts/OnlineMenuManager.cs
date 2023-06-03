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
    [SerializeField] CharacterSelectionMenuHandler _characterSelectionMenu;

    public CharacterSelectionMenuHandler CharacterSelectionMenu => _characterSelectionMenu;

    private void OnEnable()
    {
        CloseAllCanvases();
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
        _lobbyMenu.OnJoinedLobby += MoveToCarSelectionMenu;
        _lobbyMenu.OnRoomCreated += MoveToCarSelectionMenu;

    }

    private void UnregisterEvents()
    {
        MainMenuManager.Instance.OnPlayerWantToPlay -= ChangeEnterNameCanvasStatus;
        _enterNameMenu.OnNicknameEntered -= MoveToLobbyMenu;
        _lobbyMenu.OnJoinedLobby -= MoveToCarSelectionMenu;
        _lobbyMenu.OnRoomCreated -= MoveToCarSelectionMenu;
    }

    private void CloseAllCanvases()
    {
        ChangeEnterNameCanvasStatus(false);
        ChangeLobbyCanvasStatus(false);
        ChangeCharacterSelectioStatus(false);
    }

    public void MoveToMainMenu()
    {
        CloseAllCanvases();
        OnReturnToMainMenu?.Invoke();
        MainMenuManager.Instance.ReturnToMainMenu(false);
    }

    private void MoveToLobbyMenu(string nickname)
    {
        ChangeEnterNameCanvasStatus(false);
        ChangeLobbyCanvasStatus(true);
    }

    private void MoveToCarSelectionMenu()
    {
        ChangeLobbyCanvasStatus(false);
        ChangeCharacterSelectioStatus(true);
    }


    private void ChangeEnterNameCanvasStatus(bool _toActivate)
    {
        _enterNameMenu.gameObject.SetActive(_toActivate);
    }

    private void ChangeLobbyCanvasStatus(bool _toActivate)
    {
        _lobbyMenu.gameObject.SetActive(_toActivate);
    }

    private void ChangeCharacterSelectioStatus(bool _toActivate)
    {
        _characterSelectionMenu.gameObject.SetActive(_toActivate);
    }
}
