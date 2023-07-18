using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTextLayers : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roomUnderInfoText;
    [SerializeField] private TextMeshProUGUI _roomFrontInfoText;

    public void ChangeText(string text)
    {
        _roomUnderInfoText.text = text;
        _roomFrontInfoText.text = text;
    }

    public void ChangeColor(Color color)
    {
        _roomFrontInfoText.color = color;
    }
}
