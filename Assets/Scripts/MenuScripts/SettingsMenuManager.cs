using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsMenuManager : MonoBehaviour
{
    [SerializeField] private Toggle _fullscreenToggle;
    [SerializeField] private TMP_Dropdown _resolutionsDropdown;
    [SerializeField] private TMP_Dropdown _graphicsDropdown;
    [SerializeField] private Slider _volumeSlider;

    [SerializeField] private AudioMixer _audioMixer;

    private Resolution[] _screenResolutions;
    private List<string> _resolutionsOptions = new List<string>();
    private int _currentResolutionIndex = 0;

    private void Start()
    {
        SetScreenResolutionInDropdown();
    }

    private void SetScreenResolutionInDropdown()
    {
        _screenResolutions = Screen.resolutions;
        _resolutionsDropdown.ClearOptions();
        for (int i = 0; i < _screenResolutions.Length; i++)
        {
            string option = $"{_screenResolutions[i].width} x {_screenResolutions[i].height}";
            _resolutionsOptions.Add(option);

            if (_screenResolutions[i].width == Screen.currentResolution.width && _screenResolutions[i].height == Screen.currentResolution.height)
                _currentResolutionIndex = i;
        }
        _resolutionsDropdown.AddOptions(_resolutionsOptions);
        _resolutionsDropdown.value = _currentResolutionIndex;
        _resolutionsDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution newResolution = _screenResolutions[resolutionIndex];
        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);
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
