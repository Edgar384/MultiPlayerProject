using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

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
    [SerializeField] private GameObject _settingsCanvas;

    public OnlineMenuManager OnlineMenuManager => _onlineCanvas;
    public EventSystem EventSystem => _eventSystem;
    public InputSystemUIInputModule InputSystemUIInputModule => _uiInputModule;

    public PlayerController _playerController;
    private bool _keyBoardMode = true;


    private void Awake()
    {
        Instance = this;
        ResetScene();
        _onlineCanvas.OnOnlineCanvasDisabled += ReturnToMainMenu;
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
    public void Play()
    {
        _mainMenuCanvas.gameObject.SetActive(false);
        _settingsCanvas.gameObject.SetActive(false);
        OnPlayerPressedPlay?.Invoke(true);
    }

    public void OpenSettings()
    {
        _mainMenuCanvas.gameObject.SetActive(false);
        _settingsCanvas.gameObject.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        ResetScene();
    }

    public void ReturnToMainMenu(CallbackContext callbackContext)
    {
        ResetScene();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void GetControllerButtons()
    {
        string[] controllerNames = Input.GetJoystickNames();

        if (controllerNames.Length > 0)
        {
            _keyBoardMode = false;

            foreach (var controllerName in controllerNames)
            {
                if (controllerName.Length == 19)
                {
                    Debug.Log("<color=#00ff00>PS4 CONTROLLER IS CONNECTED</color>");
                }

                if (controllerName.Length == 33)
                {
                    Debug.Log("<color=#00ff00>XBOX ONE CONTROLLER IS CONNECTED</color>");
                }
            }
        }
        else
        {
            Debug.Log("<color=#ff0000>Non CONTROLLER IS CONNECTED</color>");
        }
    }

    private void CheckControllerInput()
    {
        _playerController = new PlayerController();
        _playerController.UI.Enable();
        GetControllerButtons();
        //CanvasManager.Instance.InputSystemUIInputModule.actionsAsset.a
    }

}
