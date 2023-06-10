using GarlicStudios.Online.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInRoomUI : MonoBehaviour
{
    [SerializeField] TMP_Text _playerText;
    [SerializeField] Image _playerImage;
    
    public int ID { get; private set; }

    public void Init(OnlinePlayer onlinePlayer)
    {
        _playerText.text = onlinePlayer.NickName;
        _playerImage.color = Color.red;
        ID = onlinePlayer.ActorNumber;
        gameObject.SetActive(true);
    }
    
    public void SetReadyStatus(bool isReady)=>
        _playerImage.color = isReady ? Color.green : Color.red;
}