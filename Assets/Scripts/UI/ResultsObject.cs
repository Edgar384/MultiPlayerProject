using DefaultNamespace.SciptableObject.PlayerData;
using UnityEngine;
using UnityEngine.UI;

public class ResultsObject : MonoBehaviour
{
    [SerializeField] Image _background;
    [SerializeField] Image _playerImage;
    [SerializeField] GameTextLayers _nickname;
    [SerializeField] GameTextLayers _points;
    public void RefreshVisuals(PlayerData playerData, string nickname, string points)
    {
        _background.sprite = playerData.PlayerBackground;
        _playerImage.sprite = playerData.PlayerPic;
        _nickname.ChangeText(nickname);
        _points.ChangeText(points);
    }
}
