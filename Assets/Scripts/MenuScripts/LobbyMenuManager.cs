using DefaultNamespace.MenuScripts;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyMenuManager : MonoBehaviour
{
    public event Action<string> OnJoinedRoom;
    public event Action<string> OnRoomCreated;
    public event Action OnLeftLobby;

    [Header("Logos")]
    [SerializeField] private Image _lobbyLogo;

    [Header("Name")]
    [SerializeField] private Image _currentPlayerNameImage;
    [SerializeField] private EnterNameHandler _enterNameHandler;
    [SerializeField] private Sprite[] _playerNameImages = new Sprite[2]; //0=color, 1=grey

    [Header("Rooom Join")]
    [SerializeField] private Image _joinRoomImage;
    [SerializeField] private GameObject _joinRoomObject;

    [Header("Rooom Creation")]
    [SerializeField] private LobbyRoomUIListHandler _lobbyRoomUIListHandler;
    [SerializeField] private Image _createRoomImage;
    [SerializeField] private GameObject _createRoomObject;
    [SerializeField] private TMP_InputField _roomNameInput;
    
    private readonly Color grayColor = new Color(0.5f, 0.5f, 0.5f, 1);
    private readonly Color normalColor = new Color(255, 255, 255, 1);

    private void Start()
    {
        _lobbyRoomUIListHandler.OnRoomListVisualUpdated += ChangeToPlayerConnectedVisuals;
        CanvasManager.Instance.InputSystemUIInputModule.cancel.ToInputAction().performed += CanvasManager.Instance.OnlineMenuManager.ReturnToMainMenu;
    }

    public void CreateRoom()
    {
        OnRoomCreated?.Invoke(_roomNameInput.text);
    }

    public void JoinRoom()
    {
        OnJoinedRoom?.Invoke(_roomNameInput.text);
    }

    public void ChangeLobbyVisual(bool isNewPlayer)
    {
        if (isNewPlayer)
            ChangeToNewPlayerVisuals();

        else
            ChangeToPlayerConnectedVisuals();
    }

    private void ChangeToNewPlayerVisuals()
    {
        CanvasManager.Instance.EventSystem.firstSelectedGameObject = _enterNameHandler.GameObject();
        _currentPlayerNameImage.sprite = _playerNameImages[0]; //change sprite to color
        _currentPlayerNameImage.color = normalColor;
        _lobbyLogo.color = grayColor;
        _joinRoomImage.color = grayColor;
        _createRoomImage.color = grayColor;
    }

    private void ChangeToPlayerConnectedVisuals()
    {
        _currentPlayerNameImage.sprite = _playerNameImages[1]; //change sprite to gray
        _joinRoomImage.color = normalColor;
        _createRoomImage.color = normalColor;
        _lobbyLogo.color = normalColor;
        if (_lobbyRoomUIListHandler.GetRoomCount > 0)
            CanvasManager.Instance.EventSystem.SetSelectedGameObject(_lobbyRoomUIListHandler.GetRoom(0).gameObject);

        else
            CanvasManager.Instance.EventSystem.SetSelectedGameObject(_roomNameInput.gameObject);

    }
}
