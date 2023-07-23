using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayAudioHandler : MonoBehaviour
{
    public static GameplayAudioHandler Instace;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip[] _leadingPlayer = new AudioClip[4]; //0-BennyMcNana, 1-MadammePhillipe, 2-GreasyDoris, 3-RockyHeels
    [SerializeField] AudioClip _5_SecondsLeft;
    [SerializeField] AudioClip _30_SecondsLeft;
    [SerializeField] AudioClip _splash;
    [SerializeField] AudioClip _clash_sound;

    private void Awake()
    {
        Instace = this;
    }

    public void PlayLeadingSound(int leadingID)
    {
        switch(leadingID) 
        {
            case -1: break;
            case 0: _audioSource.PlayOneShot(_leadingPlayer[0]); break;
            case 1: _audioSource.PlayOneShot(_leadingPlayer[1]); break;
            case 2: _audioSource.PlayOneShot(_leadingPlayer[2]); break;
            case 3: _audioSource.PlayOneShot(_leadingPlayer[3]); break;
            case 4: _audioSource.PlayOneShot(_leadingPlayer[4]); break;
        }
    }
        
    public void Play30SecondsLeft() { _audioSource.PlayOneShot(_30_SecondsLeft); }

    public void Play5SecondsLeft() { _audioSource.PlayOneShot(_5_SecondsLeft); }

    public void PlaySplashSound() { _audioSource.PlayOneShot(_splash); }
}
