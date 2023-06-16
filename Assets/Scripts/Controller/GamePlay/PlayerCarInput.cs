using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// For user multiplatform control.
/// </summary>
[RequireComponent (typeof (CarController))]
public class PlayerCarInput : MonoBehaviourPun
{
    private CarController _controlledCar;

    [SerializeField] private InputAction _carController;
    [SerializeField] private InputAction _brakeController;
    [SerializeField] private InputAction _abilityController;
    


    private void Awake()
    {
        _controlledCar = GetComponent<CarController>();
        _carController.Enable();
    }

    void Update ()
    {
        if (!photonView.IsMine)
            return;
        
        //Standart input control (Keyboard or gamepad).
        var moveDir = _carController.ReadValue<Vector2>();
        bool brake = _brakeController.ReadValue<bool>();
        Debug.Log(brake);
        //Apply control for controlled car.
        _controlledCar.UpdateControls(moveDir.x, moveDir.y, brake);
    }
}