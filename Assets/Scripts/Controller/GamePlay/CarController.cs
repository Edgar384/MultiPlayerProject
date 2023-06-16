using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Main car controller
/// </summary>

[RequireComponent (typeof (Rigidbody))]
public class CarController :MonoBehaviour
{

    [SerializeField] private Wheel frontLeftWheel;
    [SerializeField] private Wheel frontRightWheel;
    [SerializeField] private Wheel rearLeftWheel;
    [SerializeField] private Wheel rearRightWheel;
    [SerializeField] private Transform com;
    [SerializeField] private List<ParticleSystem> backFireParticles = new List<ParticleSystem>();

    [FormerlySerializedAs("CarConfig")] [SerializeField] private CarConfig carConfig;

    #region Properties of car parameters

    private float _maxMotorTorque;
    private float MaxSteerAngle => carConfig.maxSteerAngle;
    private DriveType DriveType => carConfig.driveType;
    private bool AutomaticGearBox => carConfig.automaticGearBox;
    private AnimationCurve MotorTorqueFromRpmCurve => carConfig.motorTorqueFromRpmCurve;
    private float MaxRpm => carConfig.maxRpm;
    private float MinRpm => carConfig.minRpm;
    private float CutOffRpm => carConfig.cutOffRpm;
    private float CutOffOffsetRpm => carConfig.cutOffOffsetRpm;
    private float RpmToNextGear => carConfig.rpmToNextGear;
    private float RpmToPrevGear => carConfig.rpmToPrevGear;
    private float MaxForwardSlipToBlockChangeGear => carConfig.maxForwardSlipToBlockChangeGear;
    private float RpmEngineToRpmWheelsLerpSpeed => carConfig.rpmEngineToRpmWheelsLerpSpeed;
    private float[] GearsRatio => carConfig.gearsRatio;
    private float MainRatio => carConfig.mainRatio;
    private float ReversGearRatio => carConfig.reversGearRatio;
    private float MaxBrakeTorque => carConfig.maxBrakeTorque;

    #endregion //Properties of car parameters

    #region Properties of drif Settings

    private bool EnableSteerAngleMultiplier => carConfig.enableSteerAngleMultiplier;
    private float MinSteerAngleMultiplier => carConfig.minSteerAngleMultiplier;
    private float MaxSteerAngleMultiplier => carConfig.maxSteerAngleMultiplier;
    private float MaxSpeedForMinAngleMultiplier => carConfig.maxSpeedForMinAngleMultiplier;
    private float SteerAngleChangeSpeed => carConfig.steerAngleChangeSpeed;
    private float MinSpeedForSteerHelp => carConfig.minSpeedForSteerHelp;
    private float HelpSteerPower => carConfig.helpSteerPower;
    private float OppositeAngularVelocityHelpPower => carConfig.oppositeAngularVelocityHelpPower;
    private float PositiveAngularVelocityHelpPower => carConfig.positiveAngularVelocityHelpPower;
    private float MaxAngularVelocityHelpAngle => carConfig.maxAngularVelocityHelpAngle;
    private float AngularVelocityInMaxAngle => carConfig.angularVelucityInMaxAngle;
    private float AngularVelocityInMinAngle => carConfig.angularVelucityInMinAngle;

    #endregion //Properties of drif Settings

    private CarConfig GetCarConfig => carConfig;
    private Wheel[] Wheels { get; set; }										//All wheels, public link.			
    public System.Action BackFireAction;                                            //Backfire invoked when cut off (You can add a invoke when changing gears).

    private float[] _allGearsRatio;															 //All gears (Reverce, neutral and all forward).

    private Rigidbody _rb;
    public Rigidbody Rb
    {
        get
        {
            if (!_rb)
            {
                _rb = GetComponent<Rigidbody> ();
            }
            return _rb;
        }
    }

    public float CurrentMaxSlip { get; private set; }						//Max slip of all wheels.
    public int CurrentMaxSlipWheelIndex { get; private set; }				//Max slip wheel index.
    public float CurrentSpeed { get; private set; }							//Speed, magnitude of velocity.
    public float SpeedInHour => CurrentSpeed * C.KPHMult;
    public int CarDirection => CurrentSpeed < 1 ? 0 : (VelocityAngle < 90 && VelocityAngle > -90 ? 1 : -1);

    private float _currentSteerAngle;
    private float _currentAcceleration;
    private float _currentBrake;
    private bool _inHandBrake;

    private int _firstDriveWheel;
    private int _lastDriveWheel;

    private void Awake ()
    {
        Rb.centerOfMass = com.localPosition;

        //Copy wheels in public property
        Wheels = new Wheel[4] {
            frontLeftWheel,
            frontRightWheel,
            rearLeftWheel,
            rearRightWheel
        };

        //Set drive wheel.
        switch (DriveType)
        {
            case DriveType.Awd:
                _firstDriveWheel = 0;
                _lastDriveWheel = 3;
                break;
            case DriveType.Fwd:
                _firstDriveWheel = 0;
                _lastDriveWheel = 1;
                break;
            case DriveType.Rwd:
                _firstDriveWheel = 2;
                _lastDriveWheel = 3;
                break;
        }

        //Divide the motor torque by the count of driving wheels
        _maxMotorTorque = carConfig.maxMotorTorque / (_lastDriveWheel - _firstDriveWheel + 1);


        //Calculated gears ratio with main ratio
        _allGearsRatio = new float[GearsRatio.Length + 2];
        _allGearsRatio[0] = ReversGearRatio * MainRatio;
        _allGearsRatio[1] = 0;
        for (int i = 0; i < GearsRatio.Length; i++)
        {
            _allGearsRatio[i + 2] = GearsRatio[i] * MainRatio;
        }

        foreach (var particles in backFireParticles)
        {
            BackFireAction += () => particles.Emit (2);
        }
    }

    /// <summary>
    /// Update controls of car, from user control (TODO AI control).
    /// </summary>
    /// <param name="horizontal">Turn direction</param>
    /// <param name="vertical">Acceleration</param>
    /// <param name="handBrake"></param>
    public void UpdateControls (float horizontal, float vertical, bool handBrake)
    {
        float targetSteerAngle = horizontal * MaxSteerAngle;

        if (EnableSteerAngleMultiplier)
        {
            targetSteerAngle *= Mathf.Clamp (1 - SpeedInHour / MaxSpeedForMinAngleMultiplier, MinSteerAngleMultiplier, MaxSteerAngleMultiplier);
        }

        _currentSteerAngle = Mathf.MoveTowards (_currentSteerAngle, targetSteerAngle, Time.deltaTime * SteerAngleChangeSpeed);

        _currentAcceleration = vertical;
        _inHandBrake = handBrake;
    }

    private void Update ()
    {
        for (int i = 0; i < Wheels.Length; i++)
        {
            Wheels[i].UpdateVisual ();
        }
    }

    private void FixedUpdate ()
    {

        CurrentSpeed = Rb.velocity.magnitude;

        UpdateSteerAngleLogic ();
        UpdateRpmAndTorqueLogic ();

        //Find max slip and update braking ground logic.
        CurrentMaxSlip = Wheels[0].CurrentMaxSlip;
        CurrentMaxSlipWheelIndex = 0;

        if (_inHandBrake)
        {
            rearLeftWheel.WheelCollider.brakeTorque = MaxBrakeTorque;
            rearRightWheel.WheelCollider.brakeTorque = MaxBrakeTorque;
            frontLeftWheel.WheelCollider.brakeTorque = 0;
            frontRightWheel.WheelCollider.brakeTorque = 0;
        }

        for (int i = 0; i < Wheels.Length; i++)
        {
            if (!_inHandBrake)
            {
                Wheels[i].WheelCollider.brakeTorque = _currentBrake;
            }

            Wheels[i].FixedUpdate ();

            if (CurrentMaxSlip < Wheels[i].CurrentMaxSlip)
            {
                CurrentMaxSlip = Wheels[i].CurrentMaxSlip;
                CurrentMaxSlipWheelIndex = i;
            }
        }

    }

    #region Steer help logic

    //Angle between forward point and velocity point.
    public float VelocityAngle { get; private set; }

    /// <summary>
    /// Update all helpers logic.
    /// </summary>
    private void UpdateSteerAngleLogic ()
    {
        var needHelp = SpeedInHour > MinSpeedForSteerHelp && CarDirection > 0;
        float targetAngle = 0;
        VelocityAngle = -Vector3.SignedAngle (Rb.velocity, transform.TransformDirection (Vector3.forward), Vector3.up);

        if (needHelp)
        {
            //Wheel turning helper.
            targetAngle = Mathf.Clamp (VelocityAngle * HelpSteerPower, -MaxSteerAngle, MaxSteerAngle);
        }

        //Wheel turn limitation.
        targetAngle = Mathf.Clamp (targetAngle + _currentSteerAngle, -(MaxSteerAngle + 10), MaxSteerAngle + 10);

        //Front wheel turn.
        Wheels[0].WheelCollider.steerAngle = targetAngle;
        Wheels[1].WheelCollider.steerAngle = targetAngle;

        if (needHelp)
        {
            //Angular velocity helper.
            var absAngle = Mathf.Abs (VelocityAngle);

            //Get current procent help angle.
            float currentAngularProcent = absAngle / MaxAngularVelocityHelpAngle;

            var currAngle = Rb.angularVelocity;

            if (VelocityAngle * _currentSteerAngle > 0)
            {
                //Turn to the side opposite to the angle. To change the angular velocity.
                var angularVelocityMagnitudeHelp = OppositeAngularVelocityHelpPower * _currentSteerAngle * Time.fixedDeltaTime;
                currAngle.y += angularVelocityMagnitudeHelp * currentAngularProcent;
            }
            else if (!Mathf.Approximately (_currentSteerAngle, 0))
            {
                //Turn to the side positive to the angle. To change the angular velocity.
                var angularVelocityMagnitudeHelp = PositiveAngularVelocityHelpPower * _currentSteerAngle * Time.fixedDeltaTime;
                currAngle.y += angularVelocityMagnitudeHelp * (1 - currentAngularProcent);
            }

            //Clamp and apply of angular velocity.
            var maxMagnitude = ((AngularVelocityInMaxAngle - AngularVelocityInMinAngle) * currentAngularProcent) + AngularVelocityInMinAngle;
            currAngle.y = Mathf.Clamp (currAngle.y, -maxMagnitude, maxMagnitude);
            Rb.angularVelocity = currAngle;
        }
    }

    #endregion //Steer help logic

    #region Rpm and torque logic

    public int CurrentGear { get; private set; }
    public int CurrentGearIndex => CurrentGear + 1;
    public float EngineRpm { get; private set; }
    public float GetMaxRpm => MaxRpm;
    public float GetMinRpm => MinRpm;
    public float GetInCutOffRpm => CutOffRpm - CutOffOffsetRpm;

    private float _cutOffTimer;
    private bool _inCutOff;

    private void UpdateRpmAndTorqueLogic ()
    {

        if (_inCutOff)
        {
            if (_cutOffTimer > 0)
            {
                _cutOffTimer -= Time.fixedDeltaTime;
                EngineRpm = Mathf.Lerp (EngineRpm, GetInCutOffRpm, RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime);
            }
            else
            {
                _inCutOff = false;
            }
        }

        //Get drive wheel with MinRPM.
        float minRpm = 0;
        for (int i = _firstDriveWheel + 1; i <= _lastDriveWheel; i++)
        {
            minRpm += Wheels[i].WheelCollider.rpm;
        }

        minRpm /= _lastDriveWheel - _firstDriveWheel + 1;

        if (!_inCutOff)
        {
            //Calculate the rpm based on rpm of the wheel and current gear ratio.
            float targetRpm = ((minRpm + 20) * _allGearsRatio[CurrentGearIndex]).Abs ();              //+20 for normal work CutOffRPM
            targetRpm = Mathf.Clamp (targetRpm, MinRpm, MaxRpm);
            EngineRpm = Mathf.Lerp (EngineRpm, targetRpm, RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime);
        }

        if (EngineRpm >= CutOffRpm)
        {
            PlayBackfireWithProbability ();
            _inCutOff = true;
            _cutOffTimer = carConfig.cutOffTime;
            return;
        }

        if (!Mathf.Approximately (_currentAcceleration, 0))
        {
            //If the direction of the car is the same as Current Acceleration.
            if (CarDirection * _currentAcceleration >= 0)
            {
                _currentBrake = 0;

                float motorTorqueFromRpm = MotorTorqueFromRpmCurve.Evaluate (EngineRpm * 0.001f);
                var motorTorque = _currentAcceleration * (motorTorqueFromRpm * (_maxMotorTorque * _allGearsRatio[CurrentGearIndex]));
                if (Mathf.Abs (minRpm) * _allGearsRatio[CurrentGearIndex] > MaxRpm)
                {
                    motorTorque = 0;
                }

                //If the rpm of the wheel is less than the max rpm engine * current ratio, then apply the current torque for wheel, else not torque for wheel.
                float maxWheelRpm = _allGearsRatio[CurrentGearIndex] * EngineRpm;
                for (int i = _firstDriveWheel; i <= _lastDriveWheel; i++)
                {
                    if (Wheels[i].WheelCollider.rpm <= maxWheelRpm)
                    {
                        Wheels[i].WheelCollider.motorTorque = motorTorque;
                    }
                    else
                    {
                        Wheels[i].WheelCollider.motorTorque = 0;
                    }
                }
            }
            else
            {
                _currentBrake = MaxBrakeTorque;
            }
        }
        else
        {
            _currentBrake = 0;

            for (int i = _firstDriveWheel; i <= _lastDriveWheel; i++)
            {
                Wheels[i].WheelCollider.motorTorque = 0;
            }
        }

        //Automatic gearbox logic. 
        if (AutomaticGearBox)
        {

            bool forwardIsSlip = false;
            for (int i = _firstDriveWheel; i <= _lastDriveWheel; i++)
            {
                if (Wheels[i].CurrentForwardSleep > MaxForwardSlipToBlockChangeGear)
                {
                    forwardIsSlip = true;
                    break;
                }
            }

            float prevRatio = 0;
            float newRatio = 0;

            if (!forwardIsSlip && EngineRpm > RpmToNextGear && CurrentGear >= 0 && CurrentGear < (_allGearsRatio.Length - 2))
            {
                prevRatio = _allGearsRatio[CurrentGearIndex];
                CurrentGear++;
                newRatio = _allGearsRatio[CurrentGearIndex];
            }
            else if (EngineRpm < RpmToPrevGear && CurrentGear > 0 && (EngineRpm <= MinRpm || CurrentGear != 1))
            {
                prevRatio = _allGearsRatio[CurrentGearIndex];
                CurrentGear--;
                newRatio = _allGearsRatio[CurrentGearIndex];
            }

            if (!Mathf.Approximately (prevRatio, 0) && !Mathf.Approximately (newRatio, 0))
            {
                EngineRpm = Mathf.Lerp (EngineRpm, EngineRpm * (newRatio / prevRatio), RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime); //EngineRPM * (prevRatio / newRatio);// 
            }

            if (CarDirection <= 0 && _currentAcceleration < 0)
            {
                CurrentGear = -1;
            }
            else if (CurrentGear <= 0 && CarDirection >= 0 && _currentAcceleration > 0)
            {
                CurrentGear = 1;
            }
            else if (CarDirection == 0 && _currentAcceleration == 0)
            {
                CurrentGear = 0;
            }
        }

        //TODO manual gearbox logic.
    }

    private void PlayBackfireWithProbability ()
    {
        PlayBackfireWithProbability (GetCarConfig.probabilityBackfire);
    }

    private void PlayBackfireWithProbability (float probability)
    {
        if (Random.Range (0f, 1f) <= probability)
        {
            BackFireAction.SafeInvoke ();
        }
    }

    #endregion


    private void OnDrawGizmosSelected ()
    {
        var centerPos = transform.position;
        var velocity = transform.position + (Vector3.ClampMagnitude (Rb.velocity, 4));
        var forwardPos = transform.TransformPoint (Vector3.forward * 4);

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere (centerPos, 0.2f);
        Gizmos.DrawLine (centerPos, velocity);
        Gizmos.DrawLine (centerPos, forwardPos);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (forwardPos, 0.2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere (velocity, 0.2f);

        Gizmos.color = Color.white;
    }

}

/// <summary>
/// For easy initialization and change of parameters in the future.
/// </summary>
[System.Serializable]
public class CarConfig
{
    [FormerlySerializedAs("MaxSteerAngle")] [TabGroup("Steer Settings")] public float maxSteerAngle = 25;

    [TabGroup("Engine and power settings")] public DriveType driveType = DriveType.Rwd;				//Drive type AWD, FWD, RWD. With the current parameters of the car only RWD works well. TODO Add rally and offroad regime.
    [TabGroup("Engine and power settings")] public bool automaticGearBox = true;
    [TabGroup("Engine and power settings")] public float maxMotorTorque = 150;						//Max motor torque engine (Without GearBox multiplier).
    [TabGroup("Engine and power settings")] public AnimationCurve motorTorqueFromRpmCurve;			//Curve motor torque (Y(0-1) motor torque, X(0-7) motor RPM).
    [TabGroup("Engine and power settings")] public float maxRpm = 7000;
    [TabGroup("Engine and power settings")] public float minRpm = 700;
    [TabGroup("Engine and power settings")] public float cutOffRpm = 6800;							//The RPM at which the cutoff is triggered.
    [TabGroup("Engine and power settings")] public float cutOffOffsetRpm = 500;
    [TabGroup("Engine and power settings")] public float cutOffTime = 0.1f;
    [TabGroup("Engine and power settings"),Range(0, 1)]public float probabilityBackfire = 0.2f;   //Probability backfire: 0 - off backfire, 1 always on backfire.
    [TabGroup("Engine and power settings")] public float rpmToNextGear = 6500;						//The speed at which there is an increase in gearbox.
    [TabGroup("Engine and power settings")] public float rpmToPrevGear = 4500;						//The speed at which there is an decrease in gearbox.
    [TabGroup("Engine and power settings")] public float maxForwardSlipToBlockChangeGear = 0.5f;	//Maximum rear wheel slip for shifting gearbox.
    [TabGroup("Engine and power settings")] public float rpmEngineToRpmWheelsLerpSpeed = 15;		//Lerp Speed change of RPM.
    [TabGroup("Engine and power settings")] public float[] gearsRatio;								//Forward gears ratio.
    [TabGroup("Engine and power settings")] public float mainRatio;
    [TabGroup("Engine and power settings")] public float reversGearRatio;							//Reverse gear ratio.

    [TabGroup("Braking settings")] public float maxBrakeTorque = 1000;
	
    [TabGroup("Helper settings")] public bool enableSteerAngleMultiplier = true;
    [TabGroup("Helper settings")] public float minSteerAngleMultiplier = 0.05f;			//Min steer angle multiplayer to limit understeer at high speeds.
    [TabGroup("Helper settings")] public float maxSteerAngleMultiplier = 1f;			//Max steer angle multiplayer to limit understeer at high speeds.
    [TabGroup("Helper settings")] public float maxSpeedForMinAngleMultiplier = 250;		//The maximum speed at which there will be a minimum steering angle multiplier.
    
    [TabGroup("Helper settings")] 
    [Space(10)]
    [TabGroup("Helper settings")] public float steerAngleChangeSpeed;                     //Wheel turn speed.
    [TabGroup("Helper settings")] public float minSpeedForSteerHelp;                      //Min speed at which helpers are enabled.
    [TabGroup("Helper settings"),Range(0f, 1f)] public float helpSteerPower;            //The power of turning the wheels in the direction of the drift.
    [TabGroup("Helper settings")] public float oppositeAngularVelocityHelpPower = 0.1f;	//The power of the helper to turn the rigidbody in the direction of the control turn.
    [TabGroup("Helper settings")] public float positiveAngularVelocityHelpPower = 0.1f;	//The power of the helper to positive turn the rigidbody in the direction of the control turn.
    [TabGroup("Helper settings")] public float maxAngularVelocityHelpAngle;               //The angle at which the assistant works 100%.
    [TabGroup("Helper settings")] public float angularVelucityInMaxAngle;                 //Min angular velucity, reached at max drift angles.
    [TabGroup("Helper settings")] public float angularVelucityInMinAngle;                 //Max angular velucity, reached at min drift angles.
}

public enum DriveType
{
    Awd,                    //All wheels drive
    Fwd,                    //Forward wheels drive
    Rwd                     //Rear wheels drive
}