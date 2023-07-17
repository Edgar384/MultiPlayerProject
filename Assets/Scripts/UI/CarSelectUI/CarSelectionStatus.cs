using UnityEngine;
using UnityEngine.UI;

public class CarSelectionStatus : MonoBehaviour
{
    [SerializeField] private Image _notvaiAvailableImage;
    private bool _isAvailable;

    private void OnEnable()
    {
        //_notvaiAvailableImage.gameObject.SetActive(!_isAvailable);
    }

    public bool CheckIfCarIsFree() { return _isAvailable; }

    public void ChangeCarAvailability(bool isAvailable)
    {
        _isAvailable = isAvailable;

        //_notvaiAvailableImage.gameObject.SetActive(!_isAvailable);
    }
}