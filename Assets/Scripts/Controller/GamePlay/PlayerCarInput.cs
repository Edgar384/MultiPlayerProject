using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// For user multiplatform control.
/// </summary>
[RequireComponent (typeof (CarController))]
public class PlayerCarInput : MonoBehaviourPun
{
    CarController ControlledCar;

    [SerializeField] private InputAction _carController;
    [SerializeField] private InputAction _brakeController;
    [SerializeField] private InputAction _abilityController;
    
    public bool Brake { get; private set; }


    private void Awake()
    {
        ControlledCar = GetComponent<CarController>();
        _carController.Enable();
    }

    void Update ()
    {
        if (!photonView.IsMine)
            return;
        
        //Standart input control (Keyboard or gamepad).
        var moveDir = _carController.ReadValue<Vector2>();
        Brake = _brakeController.ReadValue<bool>();
        Debug.Log(Brake);
        //Apply control for controlled car.
        ControlledCar.UpdateControls(moveDir.x, moveDir.y, Brake);
    }
}