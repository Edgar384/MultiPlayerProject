using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;

    private void OnEnable()
    {
        CanvasManager.Instance.EventSystem.firstSelectedGameObject = _playButton.gameObject;
    }

    private void SubscribeEvents()
    {
    }

    private void UnsubscribeEvnents()
    {

    }
}
