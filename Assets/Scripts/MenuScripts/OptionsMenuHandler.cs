using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public class OptionsMenuHandler : MonoBehaviour
{
    [Header("Input Action")]
    [SerializeField] private PlayerController _playerController;

    [Header("Audio")]
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] TMP_Text _audioText;
    [SerializeField] Slider _audioSlider;
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

    [Header("Quality")]
    [SerializeField] TMP_Text _qualityText;
    private List<string> _qualitysStrings = new List<string>() { "Low", "Medium", "High", "Ultra" };
    private int _currentQualityIndex;
    private int _currentTextQualityIndex;

    private void OnEnable()
    {
        _playerController = new PlayerController();
        _playerController.UI.Enable();
        _playerController.UI.Confirm.performed += ApplyChanges;
        _playerController.UI.Back.performed += CanvasManager.Instance.ReturnToMainMenu;
        _playerController.UI.Navigate.performed += CheckInput;
        GetScreenResolution();
        SetWindowScreenStatusText();
        SetQualityText();
        SetVolumeStatusText(_audioSlider.value);
        CanvasManager.Instance.EventSystem.SetSelectedGameObject(_audioSlider.gameObject);
    }

    private void OnDisable()
    {
        //_playerController.UI.Confirm.performed -= ApplyChanges;
        //_playerController.UI.Back.performed -= CanvasManager.Instance.ReturnToMainMenu;
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
            _windowModeText.text = _windowModeTexts[0];
            _isFullScreen = true;
        }

        else
        {
            _windowModeText.text = _windowModeTexts[1];
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
    }

    public void SetVolumeStatusText(float volume)
    {
        _audioText.text = volume.ToString();
    }
    #endregion




    //public void NextOption()
    //{
    //    _cars[_selectedCharacterIndex].gameObject.SetActive(false);
    //    _selectedCharacterIndex = (_selectedCharacterIndex + 1) % _cars.Length; //For making a loop
    //    _cars[_selectedCharacterIndex].gameObject.SetActive(true);
    //    CheckCarAvailability();
    //}

    //public void PreviousOption()
    //{
    //    _cars[_selectedCharacterIndex].gameObject.SetActive(false);
    //    _selectedCharacterIndex--;
    //    if (_selectedCharacterIndex < 0)
    //        _selectedCharacterIndex += _cars.Length;
    //    _cars[_selectedCharacterIndex].gameObject.SetActive(true);
    //    CheckCarAvailability();
    //}

    public void ApplyChanges(CallbackContext callbackContext)
    {
        SetResolution();
        SetVolume();
        SetQuality();
    }

    private void CheckInput(CallbackContext callbackContext)
    {
        Vector2 input = _playerController.UI.Navigate.ReadValue<Vector2>();
        if ((input.x > -0.016 && input.x < 0.016) && (input.y > -0.016 && input.y < 0.016))
            MoveToLeftOption();

        else if (((input.x > -0.016 && input.x < 0.016) && (input.y == 1)))
            MoveToRightOption();
    }

    private void MoveToLeftOption()
    {
        Debug.Log("Move Left");
    }

    private void MoveToRightOption()
    {
        Debug.Log("Move Right");
    }

}
