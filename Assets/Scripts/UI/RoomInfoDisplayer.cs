using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomInfoDisplayer : MonoBehaviour
{
    public event Action<string> OnJoinRoom;

    [SerializeField] private Button _joinRoomButton;
    [SerializeField] private GameTextLayers _roomInfoText;
    [SerializeField] private GameTextLayers _playerCountInfoText;
    [SerializeField] public float Offset;
    public RoomInfo roomInfo { get; private set; }
    private string _roomName;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
        _roomName = roomInfo.Name;
        _roomInfoText.ChangeText($"RoomName:{roomInfo.Name}");
        _playerCountInfoText.ChangeText($"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers}");
    }

    public void JoinRoom()
    {
        if(_roomName!=string.Empty || _roomName!=null)
            OnJoinRoom?.Invoke(_roomName);
    }
}
