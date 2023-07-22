using GamePlayLogic;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// For user multiplatform control.
/// </summary>
[RequireComponent(typeof(CarController))]
public class PlayerCarInput : MonoBehaviourPun
{
    private bool _keyBoardMode = true;

    private CarController _controlledCar;

    private float _horizontal;
    private float _vertical;
    private bool _brake;

    private PlayerController _playerController;


    private void Awake()
    {
        _controlledCar = GetComponent<CarController>();
        _playerController = new PlayerController();
        _playerController.CarControl.Enable();
    }

    private void Start()
    {
        string[] controllerNames = Input.GetJoystickNames();

        if (controllerNames.Length > 0)
        {
            _keyBoardMode = false;
            
            foreach (var controllerName in controllerNames)
            {
                if (controllerName.Length == 19)
                {
                    Debug.Log("<color=#00ff00>PS4 CONTROLLER IS CONNECTED</color>");
                }

                if (controllerName.Length == 33)
                {
                    Debug.Log("<color=#00ff00>XBOX ONE CONTROLLER IS CONNECTED</color>");
                }
            }
        }
        else
        {
            Debug.Log("<color=#ff0000>Non CONTROLLER IS CONNECTED</color>");
        }
    }

    private void Update()
    {
        if (!OnlineGameManager.IsGameRunning)
        {
            _controlledCar.UpdateControls(0, 0, true);
            return;
        }
        
        if (!photonView.IsMine)
            return;

        if (_keyBoardMode)
        {
            _horizontal = _playerController.CarControl.Steer.ReadValue<Vector2>().x;
            _vertical = _playerController.CarControl.Steer.ReadValue<Vector2>().y;
            _brake = _playerController.CarControl.Break.IsPressed();
        }
        else
        {
             float gass = _playerController.CarControl.Gass.ReadValue<float>();
             float revers = _playerController.CarControl.Revers.ReadValue<float>();
            if (gass > 0)
                _vertical = gass;
            else
                _vertical = -revers;
             _horizontal = _playerController.CarControl.Steer.ReadValue<Vector2>().x;
            _brake = _playerController.CarControl.Break.IsPressed();
        }
        
        _controlledCar.UpdateControls(_horizontal, _vertical, _brake);
    }
}