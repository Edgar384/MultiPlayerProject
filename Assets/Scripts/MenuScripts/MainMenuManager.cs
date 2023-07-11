using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager: MonoBehaviour
{

    public event Action<bool> OnPlayerWantToPlay;

    [HideInInspector] public static MainMenuManager Instance;

    [Header("Canvases")]
    [SerializeField] private GameObject _mainMenuCanvas;
    [SerializeField] private GameObject _onlineCanvas;
    [SerializeField] private GameObject _settingsCanvas;

    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;


    [SerializeField] private EventSystem _eventSystem;

    private void Awake()
    {
        Instance = this;
        ResetScene();
    }

    private void ResetScene()
    {
        _mainMenuCanvas.SetActive(true);
        _onlineCanvas.SetActive(false);
        _settingsCanvas.SetActive(false);

        _eventSystem.firstSelectedGameObject = _playButton.gameObject;
    }
    public void Play()
    {
        _mainMenuCanvas.SetActive(false);
        _onlineCanvas.SetActive(true);
        OnPlayerWantToPlay?.Invoke(true);
    }

    public void OpenSettings()
    {
        _mainMenuCanvas.SetActive(false);
        _settingsCanvas.SetActive(true);
    }

    public void ReturnToMainMenu(bool isFromSettings)
    {
        if(isFromSettings) { _settingsCanvas.SetActive(false); }
        else{ _onlineCanvas.SetActive(false); }

        _mainMenuCanvas.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
