using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenusAudioHandler : MonoBehaviour
{
    [SerializeField] AudioSource _audiSource;
    [SerializeField] AudioClip _carChosen;
    [SerializeField] AudioClip _buttonClick;
    [SerializeField] AudioClip _buttonSwitch;

    public void PlayCarChosen()
    {
        _audiSource.PlayOneShot(_carChosen);
    }

    public void PlayButtonClick()
    {
        _audiSource.PlayOneShot(_buttonClick);
    }

    public void PlayButtonSwitch()
    {
        _audiSource.PlayOneShot(_buttonSwitch);
    }
}
