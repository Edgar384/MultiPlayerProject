using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;


    private void Awake()
    {
        ResetSceneSettings();
    }

    private void Start()
    {
        CanvasManager.Instance.OnReturnedToMainMenu += ResetSceneSettings;
    }

    private void OnDestroy()
    {
        CanvasManager.Instance.OnReturnedToMainMenu -= ResetSceneSettings;
    }

    private void ResetSceneSettings()
    {
        CanvasManager.Instance.EventSystem.SetSelectedGameObject(_playButton.gameObject);
        CanvasManager.Instance.EventSystem.enabled = true;
    }
}
