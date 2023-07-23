using GamePlayLogic;
using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private bool _is30SecondsReached;
    private bool _is5SecondsReached;

    private void Awake()
    {
        _is30SecondsReached = false;
        _is5SecondsReached = false;
    }

    // Update is called once per frame
    void Update()
    {
        _text.text = $"{TimeManager.TimeGame:0.00}s";

        if (TimeManager.TimeGame < 0.3 && !_is30SecondsReached)
        {
            _is30SecondsReached = true;
            GameplayAudioHandler.Instace.Play30SecondsLeft();
        }

        else if (TimeManager.TimeGame < 0.05 && !_is5SecondsReached)
        {
            _is5SecondsReached = true;
            GameplayAudioHandler.Instace.Play5SecondsLeft();
        }
    }
}
