using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        CanvasManager.Instance.PlayerController.UI.Navigate.performed += CheckInput;
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
    }

    private void OnDestroy()
    {
        CanvasManager.Instance.OnReturnedToMainMenu -= ResetSceneSettings;
        CanvasManager.Instance.PlayerController.UI.Navigate.performed -= CheckInput;
    }

    private void ResetSceneSettings()
    {
        CanvasManager.Instance.EventSystem.SetSelectedGameObject(_playButton.gameObject);
        CanvasManager.Instance.EventSystem.enabled = true;
    }

    private void CheckInput(CallbackContext callbackContext)
    {
        Vector2 input = CanvasManager.Instance.PlayerController.UI.Navigate.ReadValue<Vector2>();
        if (input.y == -1 || input.y==1) 
            CanvasManager.Instance.MenusAudioHandler.PlayButtonSwitch();
    }
}
