﻿using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Main car controller
/// </summary>

[RequireComponent (typeof (Rigidbody))]
public class CarController :MonoBehaviour
{

	[SerializeField] private Wheel FrontLeftWheel;
	[SerializeField] private Wheel FrontRightWheel;
	[SerializeField] private Wheel RearLeftWheel;
	[SerializeField] private Wheel RearRightWheel;
	[SerializeField] private Transform COM;
	[SerializeField] private List<ParticleSystem> BackFireParticles = new List<ParticleSystem>();

	[SerializeField] private CarConfig CarConfig;

	#region Properties of car parameters

	private float MaxMotorTorque;
	private float MaxSteerAngle { get { return CarConfig.MaxSteerAngle; } }
	private DriveType DriveType { get { return CarConfig.DriveType; } }
	private bool AutomaticGearBox { get { return CarConfig.AutomaticGearBox; } }
	private AnimationCurve MotorTorqueFromRpmCurve { get { return CarConfig.MotorTorqueFromRpmCurve; } }
	private float MaxRPM { get { return CarConfig.MaxRPM; } }
	private float MinRPM { get { return CarConfig.MinRPM; } }
	private float CutOffRPM { get { return CarConfig.CutOffRPM; } }
	private float CutOffOffsetRPM { get { return CarConfig.CutOffOffsetRPM; } }
	private float RpmToNextGear { get { return CarConfig.RpmToNextGear; } }
	private float RpmToPrevGear { get { return CarConfig.RpmToPrevGear; } }
	private float MaxForwardSlipToBlockChangeGear { get { return CarConfig.MaxForwardSlipToBlockChangeGear; } }
	private float RpmEngineToRpmWheelsLerpSpeed { get { return CarConfig.RpmEngineToRpmWheelsLerpSpeed; } }
	private float[] GearsRatio { get { return CarConfig.GearsRatio; } }
	private float MainRatio { get { return CarConfig.MainRatio; } }
	private float ReversGearRatio { get { return CarConfig.ReversGearRatio; } }
	private float MaxBrakeTorque { get { return CarConfig.MaxBrakeTorque; } }


	#endregion //Properties of car parameters

	#region Properties of drif Settings

	private bool EnableSteerAngleMultiplier { get { return CarConfig.EnableSteerAngleMultiplier; } }
	private float MinSteerAngleMultiplier { get { return CarConfig.MinSteerAngleMultiplier; } }
	private float MaxSteerAngleMultiplier { get { return CarConfig.MaxSteerAngleMultiplier; } }
	private float MaxSpeedForMinAngleMultiplier { get { return CarConfig.MaxSpeedForMinAngleMultiplier; } }
	private float SteerAngleChangeSpeed { get { return CarConfig.SteerAngleChangeSpeed; } }
	private float MinSpeedForSteerHelp { get { return CarConfig.MinSpeedForSteerHelp; } }
	private float HelpSteerPower { get { return CarConfig.HelpSteerPower; } }
	private float OppositeAngularVelocityHelpPower { get { return CarConfig.OppositeAngularVelocityHelpPower; } }
	private float PositiveAngularVelocityHelpPower { get { return CarConfig.PositiveAngularVelocityHelpPower; } }
	private float MaxAngularVelocityHelpAngle { get { return CarConfig.MaxAngularVelocityHelpAngle; } }
	private float AngularVelucityInMaxAngle { get { return CarConfig.AngularVelucityInMaxAngle; } }
	private float AngularVelucityInMinAngle { get { return CarConfig.AngularVelucityInMinAngle; } }

	#endregion //Properties of drif Settings

	public CarConfig GetCarConfig { get { return CarConfig; } }
	public Wheel[] Wheels { get; private set; }										//All wheels, public link.			
	public System.Action BackFireAction;                                            //Backfire invoked when cut off (You can add a invoke when changing gears).

	private float[] AllGearsRatio;															 //All gears (Reverce, neutral and all forward).

	private Rigidbody _RB;
	public Rigidbody RB
	{
		get
		{
			if (!_RB)
			{
				_RB = GetComponent<Rigidbody> ();
			}
			return _RB;
		}
	}

	public float CurrentMaxSlip { get; private set; }						//Max slip of all wheels.
	public int CurrentMaxSlipWheelIndex { get; private set; }				//Max slip wheel index.
	public float CurrentSpeed { get; private set; }							//Speed, magnitude of velocity.
	public float SpeedInHour { get { return CurrentSpeed * C.KPHMult; } }
	public int CarDirection { get { return CurrentSpeed < 1 ? 0 : (VelocityAngle < 90 && VelocityAngle > -90 ? 1 : -1); } }

	private float CurrentSteerAngle;
	private float CurrentAcceleration;
	private float CurrentBrake;
	private bool InHandBrake;

	private int FirstDriveWheel;
	private int LastDriveWheel;

	private void Awake ()
	{
		RB.centerOfMass = COM.localPosition;

		//Copy wheels in public property
		Wheels = new Wheel[4] {
			FrontLeftWheel,
			FrontRightWheel,
			RearLeftWheel,
			RearRightWheel
		};

		//Set drive wheel.
		switch (DriveType)
		{
			case DriveType.AWD:
			FirstDriveWheel = 0;
			LastDriveWheel = 3;
			break;
			case DriveType.FWD:
			FirstDriveWheel = 0;
			LastDriveWheel = 1;
			break;
			case DriveType.RWD:
			FirstDriveWheel = 2;
			LastDriveWheel = 3;
			break;
		}

		//Divide the motor torque by the count of driving wheels
		MaxMotorTorque = CarConfig.MaxMotorTorque / (LastDriveWheel - FirstDriveWheel + 1);


		//Calculated gears ratio with main ratio
		AllGearsRatio = new float[GearsRatio.Length + 2];
		AllGearsRatio[0] = ReversGearRatio * MainRatio;
		AllGearsRatio[1] = 0;
		for (int i = 0; i < GearsRatio.Length; i++)
		{
			AllGearsRatio[i + 2] = GearsRatio[i] * MainRatio;
		}

		foreach (var particles in BackFireParticles)
		{
			BackFireAction += () => particles.Emit (2);
		}
	}

	/// <summary>
	/// Update controls of car, from user control (TODO AI control).
	/// </summary>
	/// <param name="horizontal">Turn direction</param>
	/// <param name="vertical">Acceleration</param>
	/// <param name="brake">Brake</param>
	public void UpdateControls (float horizontal, float vertical, bool handBrake)
	{
		float targetSteerAngle = horizontal * MaxSteerAngle;

		if (EnableSteerAngleMultiplier)
		{
			targetSteerAngle *= Mathf.Clamp (1 - SpeedInHour / MaxSpeedForMinAngleMultiplier, MinSteerAngleMultiplier, MaxSteerAngleMultiplier);
		}

		CurrentSteerAngle = Mathf.MoveTowards (CurrentSteerAngle, targetSteerAngle, Time.deltaTime * SteerAngleChangeSpeed);

		CurrentAcceleration = vertical;
        InHandBrake = handBrake;
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

		CurrentSpeed = RB.velocity.magnitude;

		UpdateSteerAngleLogic ();
		UpdateRpmAndTorqueLogic ();

		//Find max slip and update braking ground logic.
		CurrentMaxSlip = Wheels[0].CurrentMaxSlip;
		CurrentMaxSlipWheelIndex = 0;

        if (InHandBrake)
        {
            RearLeftWheel.WheelCollider.brakeTorque = MaxBrakeTorque;
            RearRightWheel.WheelCollider.brakeTorque = MaxBrakeTorque;
            FrontLeftWheel.WheelCollider.brakeTorque = 0;
            FrontRightWheel.WheelCollider.brakeTorque = 0;
        }

        for (int i = 0; i < Wheels.Length; i++)
		{
            if (!InHandBrake)
            {
                Wheels[i].WheelCollider.brakeTorque = CurrentBrake;
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
		VelocityAngle = -Vector3.SignedAngle (RB.velocity, transform.TransformDirection (Vector3.forward), Vector3.up);

		if (needHelp)
		{
			//Wheel turning helper.
			targetAngle = Mathf.Clamp (VelocityAngle * HelpSteerPower, -MaxSteerAngle, MaxSteerAngle);
		}

		//Wheel turn limitation.
		targetAngle = Mathf.Clamp (targetAngle + CurrentSteerAngle, -(MaxSteerAngle + 10), MaxSteerAngle + 10);

		//Front wheel turn.
		Wheels[0].WheelCollider.steerAngle = targetAngle;
		Wheels[1].WheelCollider.steerAngle = targetAngle;

		if (needHelp)
		{
			//Angular velocity helper.
			var absAngle = Mathf.Abs (VelocityAngle);

			//Get current procent help angle.
			float currentAngularProcent = absAngle / MaxAngularVelocityHelpAngle;

			var currAngle = RB.angularVelocity;

			if (VelocityAngle * CurrentSteerAngle > 0)
			{
				//Turn to the side opposite to the angle. To change the angular velocity.
				var angularVelocityMagnitudeHelp = OppositeAngularVelocityHelpPower * CurrentSteerAngle * Time.fixedDeltaTime;
				currAngle.y += angularVelocityMagnitudeHelp * currentAngularProcent;
			}
			else if (!Mathf.Approximately (CurrentSteerAngle, 0))
			{
				//Turn to the side positive to the angle. To change the angular velocity.
				var angularVelocityMagnitudeHelp = PositiveAngularVelocityHelpPower * CurrentSteerAngle * Time.fixedDeltaTime;
				currAngle.y += angularVelocityMagnitudeHelp * (1 - currentAngularProcent);
			}

			//Clamp and apply of angular velocity.
			var maxMagnitude = ((AngularVelucityInMaxAngle - AngularVelucityInMinAngle) * currentAngularProcent) + AngularVelucityInMinAngle;
			currAngle.y = Mathf.Clamp (currAngle.y, -maxMagnitude, maxMagnitude);
			RB.angularVelocity = currAngle;
		}
	}

	#endregion //Steer help logic

	#region Rpm and torque logic

	public int CurrentGear { get; private set; }
	public int CurrentGearIndex { get { return CurrentGear + 1; } }
	public float EngineRPM { get; private set; }
	public float GetMaxRPM { get { return MaxRPM; } }
	public float GetMinRPM { get { return MinRPM; } }
	public float GetInCutOffRPM { get { return CutOffRPM - CutOffOffsetRPM; } }

	private float CutOffTimer;
	private bool InCutOff;

	private void UpdateRpmAndTorqueLogic ()
	{

		if (InCutOff)
		{
			if (CutOffTimer > 0)
			{
				CutOffTimer -= Time.fixedDeltaTime;
				EngineRPM = Mathf.Lerp (EngineRPM, GetInCutOffRPM, RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime);
			}
			else
			{
				InCutOff = false;
			}
		}

		// if (!GameController.RaceIsStarted)
		// {
		// 	if (InCutOff)
		// 		return;
		//
		// 	float rpm = CurrentAcceleration > 0 ? MaxRPM : MinRPM;
		// 	float speed = CurrentAcceleration > 0 ? RpmEngineToRpmWheelsLerpSpeed : RpmEngineToRpmWheelsLerpSpeed * 0.2f;
		// 	EngineRPM = Mathf.Lerp (EngineRPM, rpm, speed * Time.fixedDeltaTime);
		// 	if (EngineRPM >= CutOffRPM)
		// 	{
		// 		PlayBackfireWithProbability ();
		// 		InCutOff = true;
		// 		CutOffTimer = CarConfig.CutOffTime;
		// 	}
		// 	return;
		// }

		//Get drive wheel with MinRPM.
		float minRPM = 0;
		for (int i = FirstDriveWheel + 1; i <= LastDriveWheel; i++)
		{
			minRPM += Wheels[i].WheelCollider.rpm;
		}

		minRPM /= LastDriveWheel - FirstDriveWheel + 1;

		if (!InCutOff)
		{
			//Calculate the rpm based on rpm of the wheel and current gear ratio.
			float targetRPM = ((minRPM + 20) * AllGearsRatio[CurrentGearIndex]).Abs ();              //+20 for normal work CutOffRPM
			targetRPM = Mathf.Clamp (targetRPM, MinRPM, MaxRPM);
			EngineRPM = Mathf.Lerp (EngineRPM, targetRPM, RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime);
		}

		if (EngineRPM >= CutOffRPM)
		{
			PlayBackfireWithProbability ();
			InCutOff = true;
			CutOffTimer = CarConfig.CutOffTime;
			return;
		}

		if (!Mathf.Approximately (CurrentAcceleration, 0))
		{
			//If the direction of the car is the same as Current Acceleration.
			if (CarDirection * CurrentAcceleration >= 0)
			{
				CurrentBrake = 0;

				float motorTorqueFromRpm = MotorTorqueFromRpmCurve.Evaluate (EngineRPM * 0.001f);
				var motorTorque = CurrentAcceleration * (motorTorqueFromRpm * (MaxMotorTorque * AllGearsRatio[CurrentGearIndex]));
				if (Mathf.Abs (minRPM) * AllGearsRatio[CurrentGearIndex] > MaxRPM)
				{
					motorTorque = 0;
				}

				//If the rpm of the wheel is less than the max rpm engine * current ratio, then apply the current torque for wheel, else not torque for wheel.
				float maxWheelRPM = AllGearsRatio[CurrentGearIndex] * EngineRPM;
				for (int i = FirstDriveWheel; i <= LastDriveWheel; i++)
				{
					if (Wheels[i].WheelCollider.rpm <= maxWheelRPM)
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
				CurrentBrake = MaxBrakeTorque;
			}
		}
		else
		{
            CurrentBrake = 0;

            for (int i = FirstDriveWheel; i <= LastDriveWheel; i++)
			{
				Wheels[i].WheelCollider.motorTorque = 0;
			}
		}

		//Automatic gearbox logic. 
		if (AutomaticGearBox)
		{

			bool forwardIsSlip = false;
			for (int i = FirstDriveWheel; i <= LastDriveWheel; i++)
			{
				if (Wheels[i].CurrentForwardSleep > MaxForwardSlipToBlockChangeGear)
				{
					forwardIsSlip = true;
					break;
				}
			}

			float prevRatio = 0;
			float newRatio = 0;

			if (!forwardIsSlip && EngineRPM > RpmToNextGear && CurrentGear >= 0 && CurrentGear < (AllGearsRatio.Length - 2))
			{
				prevRatio = AllGearsRatio[CurrentGearIndex];
				CurrentGear++;
				newRatio = AllGearsRatio[CurrentGearIndex];
			}
			else if (EngineRPM < RpmToPrevGear && CurrentGear > 0 && (EngineRPM <= MinRPM || CurrentGear != 1))
			{
				prevRatio = AllGearsRatio[CurrentGearIndex];
				CurrentGear--;
				newRatio = AllGearsRatio[CurrentGearIndex];
			}

			if (!Mathf.Approximately (prevRatio, 0) && !Mathf.Approximately (newRatio, 0))
			{
				EngineRPM = Mathf.Lerp (EngineRPM, EngineRPM * (newRatio / prevRatio), RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime); //EngineRPM * (prevRatio / newRatio);// 
			}

			if (CarDirection <= 0 && CurrentAcceleration < 0)
			{
				CurrentGear = -1;
			}
			else if (CurrentGear <= 0 && CarDirection >= 0 && CurrentAcceleration > 0)
			{
				CurrentGear = 1;
			}
			else if (CarDirection == 0 && CurrentAcceleration == 0)
			{
				CurrentGear = 0;
			}
		}

		//TODO manual gearbox logic.
	}

	private void PlayBackfireWithProbability ()
	{
		PlayBackfireWithProbability (GetCarConfig.ProbabilityBackfire);
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
		var velocity = transform.position + (Vector3.ClampMagnitude (RB.velocity, 4));
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
/// For easy initialization and change of parameters in the future. TODO Add tuning.
/// </summary>
[System.Serializable]
public class CarConfig
{
	[TabGroup("Steer Settings")] public float MaxSteerAngle = 25;

	[TabGroup("Engine and power settings")] public DriveType DriveType = DriveType.RWD;				//Drive type AWD, FWD, RWD. With the current parameters of the car only RWD works well. TODO Add rally and offroad regime.
	[TabGroup("Engine and power settings")] public bool AutomaticGearBox = true;
	[TabGroup("Engine and power settings")] public float MaxMotorTorque = 150;						//Max motor torque engine (Without GearBox multiplier).
	[TabGroup("Engine and power settings")] public AnimationCurve MotorTorqueFromRpmCurve;			//Curve motor torque (Y(0-1) motor torque, X(0-7) motor RPM).
	[TabGroup("Engine and power settings")] public float MaxRPM = 7000;
	[TabGroup("Engine and power settings")] public float MinRPM = 700;
	[TabGroup("Engine and power settings")] public float CutOffRPM = 6800;							//The RPM at which the cutoff is triggered.
	[TabGroup("Engine and power settings")] public float CutOffOffsetRPM = 500;
	[TabGroup("Engine and power settings")] public float CutOffTime = 0.1f;
	[TabGroup("Engine and power settings"),Range(0, 1)]public float ProbabilityBackfire = 0.2f;   //Probability backfire: 0 - off backfire, 1 always on backfire.
	[TabGroup("Engine and power settings")] public float RpmToNextGear = 6500;						//The speed at which there is an increase in gearbox.
	[TabGroup("Engine and power settings")] public float RpmToPrevGear = 4500;						//The speed at which there is an decrease in gearbox.
	[TabGroup("Engine and power settings")] public float MaxForwardSlipToBlockChangeGear = 0.5f;	//Maximum rear wheel slip for shifting gearbox.
	[TabGroup("Engine and power settings")] public float RpmEngineToRpmWheelsLerpSpeed = 15;		//Lerp Speed change of RPM.
	[TabGroup("Engine and power settings")] public float[] GearsRatio;								//Forward gears ratio.
	[TabGroup("Engine and power settings")] public float MainRatio;
	[TabGroup("Engine and power settings")] public float ReversGearRatio;							//Reverse gear ratio.

	[TabGroup("Braking settings")] public float MaxBrakeTorque = 1000;
	
	[TabGroup("Helper settings")] public bool EnableSteerAngleMultiplier = true;
	[TabGroup("Helper settings")] public float MinSteerAngleMultiplier = 0.05f;			//Min steer angle multiplayer to limit understeer at high speeds.
	[TabGroup("Helper settings")] public float MaxSteerAngleMultiplier = 1f;			//Max steer angle multiplayer to limit understeer at high speeds.
	[TabGroup("Helper settings")] public float MaxSpeedForMinAngleMultiplier = 250;		//The maximum speed at which there will be a minimum steering angle multiplier.
	[TabGroup("Helper settings")] 
	[Space(10)]
	[TabGroup("Helper settings")] public float SteerAngleChangeSpeed;                     //Wheel turn speed.
	[TabGroup("Helper settings")] public float MinSpeedForSteerHelp;                      //Min speed at which helpers are enabled.
	[TabGroup("Helper settings"),Range(0f, 1f)] public float HelpSteerPower;            //The power of turning the wheels in the direction of the drift.
	[TabGroup("Helper settings")] public float OppositeAngularVelocityHelpPower = 0.1f;	//The power of the helper to turn the rigidbody in the direction of the control turn.
	[TabGroup("Helper settings")] public float PositiveAngularVelocityHelpPower = 0.1f;	//The power of the helper to positive turn the rigidbody in the direction of the control turn.
	[TabGroup("Helper settings")] public float MaxAngularVelocityHelpAngle;               //The angle at which the assistant works 100%.
	[TabGroup("Helper settings")] public float AngularVelucityInMaxAngle;                 //Min angular velucity, reached at max drift angles.
	[TabGroup("Helper settings")] public float AngularVelucityInMinAngle;                 //Max angular velucity, reached at min drift angles.
}

public enum DriveType
{
	AWD,                    //All wheels drive
	FWD,                    //Forward wheels drive
	RWD                     //Rear wheels drive
}
