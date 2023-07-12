using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasManager: MonoBehaviour
{

    public event Action<bool> OnPlayerPressedPlay;

    [HideInInspector] public static CanvasManager Instance;

    [Header("EventSystem")]
    [SerializeField] private EventSystem _eventSystem;

    [Header("Canvases")]
    [SerializeField] private MainMenuManager _mainMenuCanvas;
    [SerializeField] private OnlineMenuManager _onlineCanvas;
    [SerializeField] private GameObject _settingsCanvas;

    public EventSystem EventSystem => _eventSystem;

    private void Awake()
    {
        Instance = this;
        ResetScene();
        _onlineCanvas.OnReturnToMainMenu += ReturnToMainMenu;
    }

    private void ResetScene()
    {
        _mainMenuCanvas.gameObject.SetActive(true);
        _settingsCanvas.gameObject.SetActive(false);
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

    public void ExitGame()
    {
        Application.Quit();
    }

}
