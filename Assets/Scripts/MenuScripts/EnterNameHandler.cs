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
    [SerializeField] private Button _confirmNameButton;
    
    private Navigation _noneNavigation = new Navigation();
    private Navigation _autoNavigation = new Navigation();

    private void Awake()
    {
        _noneNavigation.mode = Navigation.Mode.None;
        _confirmNameButton.navigation = _noneNavigation;
    }

    private void OnEnable()
    {
        CanvasManager.Instance.EventSystem.SetSelectedGameObject(_nicknameInputField.gameObject);
        CanvasManager.Instance.InputSystemUIInputModule.submit.ToInputAction().performed += ConfirmName;
    }

    private void OnDisable()
    {
        CanvasManager.Instance.InputSystemUIInputModule.submit.ToInputAction().performed -= ConfirmName;
    }

    public void ConfirmName(CallbackContext callbackContext)
    {
        if (_nicknameInputField.text != string.Empty)
        {
            OnNicknameEntered?.Invoke(_nicknameInputField.text);
            _confirmNameButton.navigation = _autoNavigation;
        }
    }
}
