using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] GameObject[] _characters;
    [SerializeField] int _selectedCharacterIndex;

    public void NextCharacter()
    {
        _characters[_selectedCharacterIndex].SetActive(false);
        _selectedCharacterIndex= (_selectedCharacterIndex+1) %_characters.Length; //For making a loop
        _characters[_selectedCharacterIndex].SetActive(true);
    }

    public void PreviousCharacter()
    {
        _characters[_selectedCharacterIndex].SetActive(false);
        _selectedCharacterIndex--;
        if (_selectedCharacterIndex < 0)
            _selectedCharacterIndex += _characters.Length;
        _characters[_selectedCharacterIndex].SetActive(true);
    }

    public void ConfirmSelection()
    {

    }
}
