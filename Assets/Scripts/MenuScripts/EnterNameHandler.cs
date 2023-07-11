using System;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnterNameHandler : MonoBehaviour
{
    public event Action<string> OnNicknameEntered;

    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private Button _confirmNameButton;

    private bool _playerConnected;
    private Navigation _noneNavigation = new Navigation();
    private Navigation _autoNavigation = new Navigation();

    private void Awake()
    {
        _noneNavigation.mode = Navigation.Mode.None;
        _confirmNameButton.navigation = _noneNavigation;
        
    }

    private void Update()
    {
        if (_playerConnected)
            return;

        CanvasManager.Instance.EventSystem.SetSelectedGameObject(_nicknameInputField.gameObject);
        if (Input.GetKeyDown(KeyCode.KeypadEnter) ||CanvasManager.Instance.EventSystem.currentInputModule.input.GetButtonDown("Submit"))
            ConfirmName();
    }

    public void ConfirmName()
    {
        if (_nicknameInputField.text != string.Empty)
        {
            OnNicknameEntered?.Invoke(_nicknameInputField.text);
            _playerConnected = true;
        }
    }
}
