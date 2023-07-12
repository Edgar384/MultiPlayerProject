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
        OnReturnToMenu();
    }

    private void Start()
    {
        CanvasManager.Instance.OnReturnedToMainMenu += OnReturnToMenu;
        CanvasManager.Instance.InputSystemUIInputModule.submit.ToInputAction().performed += DebugCheck;
    }

    private void OnDestroy()
    {
        CanvasManager.Instance.OnReturnedToMainMenu -= OnReturnToMenu;
    }

    private void OnReturnToMenu()
    {
        CanvasManager.Instance.EventSystem.SetSelectedGameObject(_playButton.gameObject);
    }

    private void DebugCheck(CallbackContext callbackContext)
    {
        Debug.Log("submit");
    }

    private void SubscribeEvents()
    {
    }

    private void UnsubscribeEvnents()
    {

    }
}
