using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PG_Physics.Wheel
{

	[RequireComponent (typeof (WheelCollider))]
	public class PgWheelCollider :MonoBehaviour
	{
		[FormerlySerializedAs("WheelConfig")] [SerializeField, FullField] private PgWheelColliderConfig wheelConfig;

		[FormerlySerializedAs("m_WheelCollider")] [SerializeField, HideInInspector] private WheelCollider mWheelCollider;
		[FormerlySerializedAs("m_RB")] [SerializeField, HideInInspector] private Rigidbody mRb;

		private WheelCollider WheelCollider
		{
			get
			{
				if (mWheelCollider == null)
				{
					mWheelCollider = GetComponent<WheelCollider> ();
				}
				return mWheelCollider;
			}
		}

		public Rigidbody Rb
		{
			get
			{
				if (mRb == null)
				{
					mRb = WheelCollider.attachedRigidbody;
				}
				return mRb;
			}
		}

		public void UpdateStiffness (float forward, float sideways)
		{
			var forwardFriction = WheelCollider.forwardFriction;
			var sidewaysFriction = WheelCollider.sidewaysFriction;

			forwardFriction.stiffness = forward;
			sidewaysFriction.stiffness = sideways;

			WheelCollider.forwardFriction = forwardFriction;
			WheelCollider.sidewaysFriction = sidewaysFriction;
		}

		public void UpdateConfig ()
		{
			UpdateConfig(wheelConfig);
		}

		public void UpdateConfig (PgWheelColliderConfig newConfig)
		{
			if (Rb == null)
			{
				Debug.LogError ("WheelCollider without attached RigidBody");
				return;		
			}
			wheelConfig.forwardFriction = newConfig.forwardFriction;
			wheelConfig.sidewaysFriction = newConfig.sidewaysFriction;

			if (newConfig.isFullConfig)
			{
				float springValue = Mathf.Lerp(MinSpring, MaxSpring, newConfig.spring);
				float damperValue = Mathf.Lerp(MinDamper, MaxDamper, newConfig.damper);

				JointSpring spring = new JointSpring();
				spring.spring = springValue;
				spring.damper = damperValue;
				spring.targetPosition = newConfig.targetPoint;

				WheelCollider.mass = newConfig.mass;
				WheelCollider.radius = newConfig.radius;
				WheelCollider.wheelDampingRate = newConfig.wheelDampingRate;
				WheelCollider.suspensionDistance = newConfig.suspensionDistance;
				WheelCollider.forceAppPointDistance = newConfig.forceAppPointDistance;
				WheelCollider.center = newConfig.center;
				WheelCollider.suspensionSpring = spring;
			}

			WheelFrictionCurve forwardFriction = new WheelFrictionCurve();
			forwardFriction.extremumSlip = Mathf.Lerp (MinExtremumSlip, MaxExtremumSlip, newConfig.forwardFriction);
			forwardFriction.extremumValue = Mathf.Lerp (MinExtremumValue, MaxExtremumValue, newConfig.forwardFriction);
			forwardFriction.asymptoteSlip = Mathf.Lerp (MinAsymptoteSlip, MaxAsymptoteSlip, newConfig.forwardFriction);
			forwardFriction.asymptoteValue = Mathf.Lerp (MinAsymptoteValue, MaxAsymptoteValue, newConfig.forwardFriction);
			forwardFriction.stiffness = 1;

			WheelFrictionCurve sidewaysFriction = new WheelFrictionCurve();
			sidewaysFriction.extremumSlip = Mathf.Lerp (MinExtremumSlip, MaxExtremumSlip, newConfig.sidewaysFriction);
			sidewaysFriction.extremumValue = Mathf.Lerp (MinExtremumValue, MaxExtremumValue, newConfig.sidewaysFriction);
			sidewaysFriction.asymptoteSlip = Mathf.Lerp (MinAsymptoteSlip, MaxAsymptoteSlip, newConfig.sidewaysFriction);
			sidewaysFriction.asymptoteValue = Mathf.Lerp (MinAsymptoteValue, MaxAsymptoteValue, newConfig.sidewaysFriction);
			sidewaysFriction.stiffness = 1;

			WheelCollider.forwardFriction = forwardFriction;
			WheelCollider.sidewaysFriction = sidewaysFriction;
		}


		public bool CheckFirstEnable ()
		{
			if (mWheelCollider != null) return false;

			var suspensionSpring = WheelCollider.suspensionSpring;
			var sprigValue = (suspensionSpring.spring - MinSpring) / (MaxSpring - MinSpring);
			var damper = (suspensionSpring.damper - MinDamper) / (MaxDamper - MinDamper);
			var forwardFriction = (WheelCollider.forwardFriction.extremumValue - MinExtremumValue) / (MaxExtremumValue - MinExtremumValue);
			var sidewaysFriction = (WheelCollider.sidewaysFriction.extremumValue - MinExtremumValue) / (MaxExtremumValue - MinExtremumValue);

			wheelConfig = new PgWheelColliderConfig ();
			wheelConfig.mass = WheelCollider.mass;
			wheelConfig.radius = WheelCollider.radius;
			wheelConfig.wheelDampingRate = WheelCollider.wheelDampingRate;
			wheelConfig.suspensionDistance = WheelCollider.suspensionDistance;
			wheelConfig.forceAppPointDistance = WheelCollider.forceAppPointDistance;
			wheelConfig.center = WheelCollider.center;
			wheelConfig.targetPoint = WheelCollider.suspensionSpring.targetPosition;
			wheelConfig.spring = sprigValue;
			wheelConfig.damper = damper;
			wheelConfig.forwardFriction = forwardFriction;
			wheelConfig.sidewaysFriction = sidewaysFriction;

			return true;
		}

		//Spring constants
		private const float MinSpring = 0;
		private const float MaxSpring = 60000;
		private const float MinDamper = 0;
		private const float MaxDamper = 10000;

		//Minimum friction constants
		private const float MinExtremumSlip = 0.4f;
		private const float MinExtremumValue = 0.7f;
		private const float MinAsymptoteSlip = 0.6f;
		private const float MinAsymptoteValue = 0.65f;

		//Maximum friction constants
		private const float MaxExtremumSlip = 0.4f;
		private const float MaxExtremumValue = 4.5f;
		private const float MaxAsymptoteSlip = 0.6f;
		private const float MaxAsymptoteValue = 4f;
	}

	[System.Serializable]
	public struct PgWheelColliderConfig
	{
		[FormerlySerializedAs("IsFoldout")] [SerializeField] private bool isFoldout;
		[FormerlySerializedAs("IsFullConfig")] public bool isFullConfig;

		[FormerlySerializedAs("Mass")] public float mass;
		[FormerlySerializedAs("Radius")] public float radius;
		[FormerlySerializedAs("WheelDampingRate")] public float wheelDampingRate;
		[FormerlySerializedAs("SuspensionDistance")] public float suspensionDistance;
		[FormerlySerializedAs("ForceAppPointDistance")] public float forceAppPointDistance;
		[FormerlySerializedAs("Center")] public Vector3 center;
		
		//Suspension spring
		[FormerlySerializedAs("Spring")] public float spring;
		[FormerlySerializedAs("Damper")] public float damper;
		[FormerlySerializedAs("TargetPoint")] public float targetPoint;

		//Frictions;
		[FormerlySerializedAs("ForwardFriction")] public float forwardFriction;
		[FormerlySerializedAs("SidewaysFriction")] public float sidewaysFriction;
	}

	/// <summary>
	/// Custom FullField Attribute 
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Field)]
	public class FullField :PropertyAttribute { }

}
