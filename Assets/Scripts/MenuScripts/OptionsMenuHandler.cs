using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenuHandler : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void SetVolume(float volume)
    {
        _audioMixer.SetFloat("Volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }


}
