using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    [SerializeField] private Button _confirmSelectionButton;
    [SerializeField] private GameObject _unselected;
    [SerializeField] private GameObject _onHover;
    [SerializeField] private GameObject _selected;
    [SerializeField] private GameTextLayers _playerName;

    Navigation _noNavigation = new Navigation();
    Navigation _defaultNavigation = new Navigation();
    private bool _isAvailable;

    private void OnEnable()
    {
        _noNavigation.mode = Navigation.Mode.None;
        _defaultNavigation.mode = Navigation.Mode.Automatic;
        _unselected.gameObject.SetActive(false);
        _onHover.gameObject.SetActive(false);
        _selected.gameObject.SetActive(true);
    }

    private void Update()
    {
        //Plaster until we have animations
        if(CanvasManager.Instance.EventSystem.currentSelectedGameObject != this.gameObject && _isAvailable)
        {
            _unselected.SetActive(true);
            _onHover.gameObject.SetActive(false);
        }

        else
        {
            _unselected.SetActive(false);
            _onHover.gameObject.SetActive(true);
        }

    }

    public bool CheckIfCharacterIsFree() { return _isAvailable; }

    public void ChangeCharacterAvailability(bool isAvailable, string playerName)
    {
        _isAvailable = isAvailable;
        _selected.gameObject.SetActive(!_isAvailable);
        _onHover.gameObject.SetActive(_isAvailable);
        _playerName.ChangeText(playerName);
        if (!isAvailable)
            _confirmSelectionButton.navigation = _defaultNavigation;

        else
            _confirmSelectionButton.navigation = _noNavigation;
    }
}
