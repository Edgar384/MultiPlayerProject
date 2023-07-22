using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTextLayers : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _underInfoText;
    [SerializeField] private TextMeshProUGUI _frontInfoText;

    public void ChangeText(string text)
    {
        _underInfoText.text = text;
        _frontInfoText.text = text;
    }

    public void ChangeColor(Color color)
    {
        _frontInfoText.color = color;
    }
}
