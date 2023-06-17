using System;
using DefaultNamespace.MenuScripts;
using GarlicStudios.Online.Managers;
using UnityEngine;
using UnityEngine.Serialization;

public class OnlineMenuManager : MonoBehaviour
{
    public event Action OnReturnToMainMenu;

    [HideInInspector] public static OnlineMenuManager Instance;

    [Header("Canvases")]
    [SerializeField]
    private EnterNameHandler _enterNameMenu;
    [SerializeField] private LobbyMenuHandler _lobbyMenu;
    [SerializeField] private LobbyRoomUIListHandler _lobbyRoomUIListHandler;
    [FormerlySerializedAs("_characterSelectionMenu")] [SerializeField]
    private OnlineRoomUIHandler onlineRoomUI;

    [SerializeField] private OnlineGameManager _onlineGameManager;
    
    public OnlineRoomUIHandler OnlineRoomUI => onlineRoomUI;

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
        OnlineLobbyManager.OnRoomListUpdateEvent += _lobbyRoomUIListHandler.UpdateRoomUI;
        _lobbyMenu.OnRoomCreated += OnlineLobbyManager.CreateRoom;
        OnlineRoomManager.OnJoinRoomEvent += MoveToCarSelectionMenu;
        _enterNameMenu.OnNicknameEntered += _onlineGameManager.ConnectedToMaster;
        MainMenuManager.Instance.OnPlayerWantToPlay += ChangeEnterNameCanvasStatus;
        OnlineGameManager.OnConnectedToMasterEvent += MoveToLobbyMenu;
    }

    private void UnregisterEvents()
    {
        OnlineLobbyManager.OnRoomListUpdateEvent -= _lobbyRoomUIListHandler.UpdateRoomUI;
        _lobbyMenu.OnRoomCreated -= OnlineLobbyManager.CreateRoom;
        OnlineRoomManager.OnJoinRoomEvent -= MoveToCarSelectionMenu;
        _enterNameMenu.OnNicknameEntered -= _onlineGameManager.ConnectedToMaster;
        MainMenuManager.Instance.OnPlayerWantToPlay -= ChangeEnterNameCanvasStatus;
        OnlineGameManager.OnConnectedToMasterEvent -= MoveToLobbyMenu;
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
        onlineRoomUI.gameObject.SetActive(_toActivate);
    }
}
