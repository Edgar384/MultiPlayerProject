using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [HideInInspector] public static MainMenuManager Instance;

    [Header("Canvases")]
    [SerializeField] private GameObject _mainMenuCanvas;
    [SerializeField] private GameObject _photonCanvas;
    [SerializeField] private GameObject _settingsCanvas;

    private void Awake()
    {
        Instance = this;
    }
    public void Play()
    {
        _mainMenuCanvas.SetActive(false);
        _photonCanvas.SetActive(true);
    }

    public void OpenSettings()
    {
        _mainMenuCanvas.SetActive(false);
        _settingsCanvas.SetActive(true);
    }

    public void ReturnToMainMenu(bool isFromSettings)
    {
        if(isFromSettings) { _settingsCanvas.SetActive(false); }
        else{ _photonCanvas.SetActive(false); }

        _mainMenuCanvas.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
