using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To tilt the car body (Visually only).
/// </summary>

[RequireComponent (typeof (CarController))]
public class BodyTilt :MonoBehaviour
{

	[SerializeField] private Transform Body;                                //Link to car body.
	[SerializeField] private float MaxAngle = 10;                           //Max tilt angle of car body.
	[SerializeField] private float AngleVelocityMultiplayer = 0.2f;         //Rotation angle multiplier when moving forward.
	[SerializeField] private float RearAngleVelocityMultiplayer = 0.4f;     //Rotation angle multiplier when moving backwards.
	[SerializeField] private float MaxTiltOnSpeed = 60;                     //The speed at which the maximum tilt is reached.

	private CarController Car;
	private float Angle;

	private void Awake ()
	{
		Car = GetComponent<CarController> ();
	}

	private void Update ()
	{

		if (Car.CarDirection == 1)
			Angle = -Car.VelocityAngle * AngleVelocityMultiplayer;
		else if (Car.CarDirection == -1)
		{
			Angle = MathExtentions.LoopClamp (Car.VelocityAngle + 180, -180, 180) * RearAngleVelocityMultiplayer;
		}
		else
		{
			Angle = 0;
		}

		Angle *= Mathf.Clamp01 (Car.SpeedInHour / MaxTiltOnSpeed);
		Angle = Mathf.Clamp (Angle, -MaxAngle, MaxAngle);
		Body.localRotation = Quaternion.AngleAxis (Angle, Vector3.forward);
	}
}
