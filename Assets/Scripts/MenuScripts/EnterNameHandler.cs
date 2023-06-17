using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnterNameHandler : MonoBehaviour
{
    public event Action<string> OnNicknameEntered;

    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private Button _confirmNameButton;

    private void Awake()
    {
        _confirmNameButton.interactable = false;
    }

    private void Update()
    {
        if(_nicknameInputField.text != string.Empty) { _confirmNameButton.interactable = true; }
    }

    public void ConfirmName()
    {
        if (_nicknameInputField.text != string.Empty)
        {
            OnNicknameEntered?.Invoke(_nicknameInputField.text);
        }
    }
}
