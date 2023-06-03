using System;
using Managers;
using UnityEngine;

public class OnlineMenuManager : MonoBehaviour
{
    public event Action OnReturnToMainMenu;

    [HideInInspector] public static OnlineMenuManager Instance;

    [Header("Canvases")]
    [SerializeField] EnterNameHandler _enterNameMenu;
    [SerializeField] LobbyMenuHandler _lobbyMenu;
    [SerializeField] CharacterSelectionMenuHandler _characterSelectionMenu;

    [SerializeField] private OnlineRoomManager _onlineRoomManager;
    [SerializeField] private OnlineGameManager _onlineGameManager;
    
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
        //_lobbyMenu.OnRoomCreated  += _onlineRoomManager.CreateRoom;
        OnlineRoomManager.OnJoinRoomEvent += MoveToCarSelectionMenu;
        _enterNameMenu.OnNicknameEntered += _onlineGameManager.ConnectedToMaster;
        MainMenuManager.Instance.OnPlayerWantToPlay += ChangeEnterNameCanvasStatus;
        PhotonEventer.OnConnectedToMasterEvent += MoveToLobbyMenu;
    }

    private void UnregisterEvents()
    {
        //_lobbyMenu.OnRoomCreated  -= _onlineRoomManager.CreateRoom;
        OnlineRoomManager.OnJoinRoomEvent -= MoveToCarSelectionMenu;
        _enterNameMenu.OnNicknameEntered -= _onlineGameManager.ConnectedToMaster;
        MainMenuManager.Instance.OnPlayerWantToPlay -= ChangeEnterNameCanvasStatus;
        PhotonEventer.OnConnectedToMasterEvent -= MoveToLobbyMenu;
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

    private void MoveToLobbyMenu()
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
