using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsObject : MonoBehaviour
{
    [SerializeField] Image _background;
    [SerializeField] Image _playerImage;
    [SerializeField] GameTextLayers _nickname;
    [SerializeField] GameTextLayers _points;
    public void RefreshVisuals(Sprite backgorund, Sprite playerImage, string nickname, string points)
    {
        _background.sprite = backgorund;
        _playerImage.sprite = playerImage;
        _nickname.ChangeText(nickname);
        _points.ChangeText(points);
    }
}
