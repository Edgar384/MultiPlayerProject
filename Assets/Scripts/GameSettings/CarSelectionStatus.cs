using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSelectionStatus : MonoBehaviour
{
    [SerializeField] Image _notvaiAvailableImage;
    private bool _isTaken;
    private Image _takenImage;

    private void OnEnable()
    {
        if(_isTaken)
            _notvaiAvailableImage.gameObject.SetActive(true);

        else
            _notvaiAvailableImage.gameObject.SetActive(false);
    }

    public bool CheckIfCarIsFree() { return _isTaken; }

    public void ChangeCarAvailability(bool isAvailable)
    {
        _isTaken = isAvailable;
        if(_isTaken)
            _takenImage.gameObject.SetActive(true);

        else
            _takenImage.gameObject.SetActive(false);
    }
}
