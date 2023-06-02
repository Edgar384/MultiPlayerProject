using System;
using UnityEngine;

public class MainMenuManager: MonoBehaviour
{

    public event Action<bool> OnPlayerWantToPlay;

    [HideInInspector] public static MainMenuManager Instance;

    [Header("Canvases")]
    [SerializeField] private GameObject _mainMenuCanvas;
    [SerializeField] private GameObject _onlineCanvas;
    [SerializeField] private GameObject _settingsCanvas;

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
