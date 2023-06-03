using Managers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInRoomUI : MonoBehaviour
{
    [SerializeField] TMP_Text _playerText;
    [SerializeField] Image _playerImage;

    public void Init(OnlinePlayer onlinePlayer)
    {
        _playerText.text = onlinePlayer.NickName;
        _playerImage.color = Color.red;
    }
    
    public void SetReadyStatus(bool isReady)=>
        _playerImage.color = isReady ? Color.green : Color.red;
}