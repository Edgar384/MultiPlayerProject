using System;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class EnterNameHandler : MonoBehaviour
{
    public static event Action<string> OnNicknameEntered;

    [SerializeField] private TMP_InputField _nicknameInputField;

    private Navigation _noneNavigation = new Navigation();
    private Navigation _autoNavigation = new Navigation();

    private void Awake()
    {
        _noneNavigation.mode = Navigation.Mode.None;
    }

    private void OnEnable()
    {
        CanvasManager.Instance.EventSystem.SetSelectedGameObject(_nicknameInputField.gameObject);
        CanvasManager.Instance.PlayerController.UI.Confirm.performed += ConfirmName;
    }

    private void OnDisable()
    {
        CanvasManager.Instance.PlayerController.UI.Confirm.performed -= ConfirmName;
    }

    public void ConfirmName(CallbackContext callbackContext)
    {
        if (_nicknameInputField.text != string.Empty)
        {
            CanvasManager.Instance.MenusAudioHandler.PlayButtonClick();
            OnNicknameEntered?.Invoke(_nicknameInputField.text);
        }
        gameObject.SetActive(false);
    }
}
