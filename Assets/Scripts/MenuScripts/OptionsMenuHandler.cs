using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
using System;

public class OptionsMenuHandler : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] TMP_Text _audioText;
    [SerializeField] Slider _audioSlider;
    [SerializeField] Image _handleImage;
    private float _currentVolume;
    private float _changedVolume;

    [Header("Resolution")]
    [SerializeField] TMP_Text _resolutionText;
    private Resolution[] _possibleResolutions; //The resolutions that the screen get be
    private List<string> _resolutionsStrings = new List<string>(); //the string for all the resolutions
    private int _currentScreenResolutionIndex;
    private int _currentTextResolutionIndex;

    [Header("Window Mode")]
    [SerializeField] TMP_Text _windowModeText;
    private bool _isFullScreen;
    private List<string> _windowModeTexts = new List<string>() {"Full Screen","Windows Mode"};
    private int _currentTextWindowModeIndex;

    [Header("Quality")]
    [SerializeField] TMP_Text _qualityText;
    private List<string> _qualitysStrings = new List<string>() { "Low", "Medium", "High", "Ultra" };
    private int _currentQualityIndex;
    private int _currentTextQualityIndex;

    private void OnEnable()
    {
        CanvasManager.Instance.PlayerController.UI.Confirm.performed += ApplyChanges;
        CanvasManager.Instance.PlayerController.UI.Back.performed += CanvasManager.Instance.ReturnToMainMenu;
        CanvasManager.Instance.PlayerController.UI.Navigate.performed += CheckInput;
        GetScreenResolution();
        SetWindowScreenStatusText();
        SetQualityText();
        SetVolumeStatusText(_audioSlider.value);
        CanvasManager.Instance.EventSystem.SetSelectedGameObject(_audioSlider.gameObject);
        _handleImage.SetNativeSize();
    }

    private void OnDisable()
    {
        CanvasManager.Instance.PlayerController.UI.Confirm.performed -= ApplyChanges;
        CanvasManager.Instance.PlayerController.UI.Back.performed -= CanvasManager.Instance.ReturnToMainMenu;
        CanvasManager.Instance.PlayerController.UI.Navigate.performed -= CheckInput;
    }


    #region Resolution
    public void GetScreenResolution()
    {
        _possibleResolutions = Screen.resolutions;

        for (int i = 0; i < _possibleResolutions.Length; i++)
        {
            string resolutionText = _possibleResolutions[i].width + " x " + _possibleResolutions[i].height;
            _resolutionsStrings.Add(resolutionText);

            if (_possibleResolutions[i].width == Screen.currentResolution.width && _possibleResolutions[i].height == Screen.currentResolution.height)
            {
                _currentScreenResolutionIndex = i;
                _currentTextResolutionIndex = i;
                _resolutionText.text = _resolutionsStrings[i];
            }
        }
    }

    public void SetResolution()
    {
        Resolution resolution = _possibleResolutions[_currentTextResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, _isFullScreen);
    }
    #endregion

    #region Window Mode
    public void SetWindowScreenStatusText()
    {
        if (Screen.fullScreen)
        {
            _currentTextWindowModeIndex = 0;
            _windowModeText.text = _windowModeTexts[_currentTextWindowModeIndex];
            _isFullScreen = true;
        }

        else
        {
            _currentTextWindowModeIndex = 1;
            _windowModeText.text = _windowModeTexts[_currentTextWindowModeIndex];
            _isFullScreen = false;
        }
    }

    public void SetWindowScreenStatus(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    #endregion

    #region Quality
    public void SetQuality()
    {
        QualitySettings.SetQualityLevel(_currentTextQualityIndex);
        _currentQualityIndex = _currentTextQualityIndex;
    }

    public void SetQualityText()
    {
        int qualityLevel = QualitySettings.GetQualityLevel();
        _currentQualityIndex = qualityLevel;
        _qualityText.text = _qualitysStrings[_currentQualityIndex];
    }
    #endregion

    #region Volume
    public void SetVolume()
    {
        //_audioMixer.SetFloat("Volume", _changedVolume);
        _currentVolume = _changedVolume;
        AudioListener.volume = _currentVolume;
        _handleImage.SetNativeSize();
    }

    public void SetVolumeStatusText(float volume)
    {
        _audioText.text = volume.ToString();
    }
    #endregion

    public void ApplyChanges(CallbackContext callbackContext)
    {
        CanvasManager.Instance.MenusAudioHandler.PlayButtonClick();
        SetResolution();
        SetVolume();
        SetQuality();
    }

    private void CheckInput(CallbackContext callbackContext)
    {
        Vector2 input = CanvasManager.Instance.PlayerController.UI.Navigate.ReadValue<Vector2>();
        if (input.x == 0)
            CanvasManager.Instance.MenusAudioHandler.PlayButtonSwitch();

        else if (input.x == -1)
        {
            MoveToLeftOption();
            CanvasManager.Instance.MenusAudioHandler.PlayButtonSwitch();
        }

        else if (input.x == 1)
        {
            MoveToRightOption();
            CanvasManager.Instance.MenusAudioHandler.PlayButtonSwitch();
        }

    }

    private void MoveToLeftOption()
    {
        GameObject gameObject = CanvasManager.Instance.EventSystem.currentSelectedGameObject;
        if (gameObject == _audioSlider.gameObject)
            return;
        else if (gameObject == _resolutionText.gameObject)
            PreviousOption(0);

        else if (gameObject == _windowModeText.gameObject)
            PreviousOption(1);

        else if (gameObject == _qualityText.gameObject)
            PreviousOption(2);
    }

    private void MoveToRightOption()
    {
        GameObject gameObject = CanvasManager.Instance.EventSystem.currentSelectedGameObject;
        if (gameObject == _audioSlider.gameObject)
            return;
        else if (gameObject == _resolutionText.gameObject)
            NextOption(0);

        else if (gameObject == _windowModeText.gameObject)
            NextOption(1);

        else if (gameObject == _qualityText.gameObject)
            NextOption(2);
    }

    /// <summary>
    /// 0-Resolutions, 1-Window Mode, 2-Quality
    /// </summary>
    /// <param name="switchOption"></param>
    private void NextOption(int switchOption)
    {
        switch (switchOption)
        {
            case 0: //Resolutions
                {
                    _currentTextResolutionIndex++;
                    if (_currentTextResolutionIndex == _resolutionsStrings.Count)
                        _currentTextResolutionIndex = 0;
                    _resolutionText.text = _resolutionsStrings[_currentTextResolutionIndex];
                    break;
                }
            case 1: //Window Mode
                {
                    _currentTextWindowModeIndex++;
                    if (_currentTextWindowModeIndex == _windowModeTexts.Count)
                        _currentTextWindowModeIndex = 0;
                    _windowModeText.text = _windowModeTexts[_currentTextWindowModeIndex];
                    break;
                }
            case 2: //Quality
                {
                    _currentQualityIndex++;
                    if (_currentQualityIndex == _qualitysStrings.Count)
                        _currentQualityIndex = 0;
                    _qualityText.text = _qualitysStrings[_currentQualityIndex];
                    break;
                }
        }
    }

    /// <summary>
    /// 0-Resolutions, 1-Window Mode, 2-Quality
    /// </summary>
    /// <param name="switchOption"></param>
    private void PreviousOption(int switchOption)
    {
        switch (switchOption)
        {
            case 0: //Resolutions
                {
                    _currentTextResolutionIndex--;
                    if (_currentTextResolutionIndex == -1)
                        _currentTextResolutionIndex = _resolutionsStrings.Count-1;
                    _resolutionText.text = _resolutionsStrings[_currentTextResolutionIndex];
                    break;
                }
            case 1: //Window Mode
                {
                    _currentTextWindowModeIndex--;
                    if (_currentTextWindowModeIndex == -1)
                        _currentTextWindowModeIndex = _windowModeTexts.Count-1;
                    _windowModeText.text = _windowModeTexts[_currentTextWindowModeIndex];
                    break;
                }
            case 2: //Quality
                {
                    _currentQualityIndex--;
                    if (_currentQualityIndex == -1)
                        _currentQualityIndex = _qualitysStrings.Count-1;
                    _qualityText.text = _qualitysStrings[_currentQualityIndex];
                    break;
                }
        }
    }

}
