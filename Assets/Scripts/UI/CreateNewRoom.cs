using GarlicStudios.Online.Managers;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class CreateNewRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField _roomName;

    private void Awake()
    {
        CanvasManager.Instance.PlayerController.UI.CreateRoomResetSettings.performed += CreateRoom;
    }

    public void CreateRoom(CallbackContext callbackContext)
    {
        if (CanvasManager.Instance.EventSystem.currentSelectedGameObject == this.gameObject && _roomName.text != null)
            OnlineLobbyManager.CreateRoom(_roomName.text);
    }
}