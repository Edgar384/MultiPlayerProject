using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

[DefaultExecutionOrder(-999)]
public class CanvasManager: MonoBehaviour
{
    public event Action<bool> OnPlayerPressedPlay;
    public event Action OnReturnedToMainMenu;

    [HideInInspector] public static CanvasManager Instance;

    [Header("EventSystem")]
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private InputSystemUIInputModule _uiInputModule;

    [Header("Canvases")]
    [SerializeField] private MainMenuManager _mainMenuCanvas;
    [SerializeField] private OnlineMenuManager _onlineCanvas;
    [SerializeField] private OptionsMenuHandler _settingsCanvas;

    [Header("Audio Manager")]
    [SerializeField] private MenusAudioHandler _manusAudioHandler;

    public OnlineMenuManager OnlineMenuManager => _onlineCanvas;
    public EventSystem EventSystem => _eventSystem;
    public InputSystemUIInputModule InputSystemUIInputModule => _uiInputModule;
    public MenusAudioHandler MenusAudioHandler => _manusAudioHandler;

    private bool _keyBoardMode = true;
    public PlayerController PlayerController { get; private set; }


    private void Awake()
    {
        Instance = this;
        _onlineCanvas.OnOnlineCanvasDisabled += ReturnToMainMenu;
        PlayerController = new PlayerController();
        PlayerController.UI.Enable();
    }

    private void Start()
    {
        CheckIfConnected();
    }

    private void OnDestroy()
    {
        _onlineCanvas.OnOnlineCanvasDisabled -= ReturnToMainMenu;
    }

    private void ResetScene()
    {
        _mainMenuCanvas.gameObject.SetActive(true);
        _settingsCanvas.gameObject.SetActive(false);
        OnReturnedToMainMenu?.Invoke();
    }

    private IEnumerator PlayForConnected()
    {
        yield return new WaitForSeconds(0.5f);
        Play(true);
    }

    public void Play(bool isConnected)
    {
        _mainMenuCanvas.gameObject.SetActive(false);
        _settingsCanvas.gameObject.SetActive(false);
        if(!isConnected)
        _manusAudioHandler.PlayButtonClick();
        OnPlayerPressedPlay?.Invoke(isConnected);
    }

    private void CheckIfConnected()
    {
        if (PhotonNetwork.IsConnected)
        {
            //PhotonNetwork.LeaveRoom();
            StartCoroutine(PlayForConnected());
        }

        else
            ResetScene();
    }

    public void OpenSettings()
    {
        _manusAudioHandler.PlayButtonClick();
        _mainMenuCanvas.gameObject.SetActive(false);
        _settingsCanvas.gameObject.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        
        ResetScene();
    }

    public void ReturnToMainMenu(CallbackContext callbackContext)
    {
        _manusAudioHandler.PlayButtonClick();
        ResetScene();
    }

    public void ExitGame()
    {
        _manusAudioHandler.PlayButtonClick();
        Application.Quit();
    }
}
