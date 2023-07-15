using System.Collections;
using System.Collections.Generic;
using GamePlayLogic;
using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    // Update is called once per frame
    void Update()
    {
        _text.text = $"{TimeManager.TimeGame:0.00}s";
    }
}
