using UnityEngine;

/// <summary>
/// Car sound controller, for play car sound effects
/// </summary>

[RequireComponent (typeof (CarController))]
public class CarSoundController :MonoBehaviour
{

    [Header("Engine sounds")]
    [SerializeField]
    private AudioClip EngineIdleClip;
    [SerializeField] private AudioClip EngineBackFireClip;
    [SerializeField] private float PitchOffset = 0.5f;
    [SerializeField] private AudioSource EngineSource;

    [Header("Slip sounds")]
    [SerializeField]
    private AudioSource SlipSource;
    [SerializeField] private float MinSlipSound = 0.15f;
    [SerializeField] private float MaxSlipForSound = 1f;

    private CarController CarController;

    private float MaxRPM { get { return CarController.GetMaxRPM; } }
    private float EngineRPM { get { return CarController.EngineRPM; } }

    private void Awake ()
    {
        CarController = GetComponent<CarController> ();
        CarController.BackFireAction += PlayBackfire;
    }

    private void Update ()
    {

        //Engine PRM sound
        EngineSource.pitch = (EngineRPM / MaxRPM) + PitchOffset;

        //Slip sound logic
        if (CarController.CurrentMaxSlip > MinSlipSound)
        {
            if (!SlipSource.isPlaying)
            {
                SlipSource.Play();
            }
            var slipVolumeProcent = CarController.CurrentMaxSlip / MaxSlipForSound;
            SlipSource.volume = slipVolumeProcent * 0.5f;
            SlipSource.pitch = Mathf.Clamp (slipVolumeProcent, 0.75f, 1);
        }
        else
        {
            SlipSource.Stop();
        }
    }

    private void PlayBackfire ()
    {
        EngineSource.PlayOneShot(EngineBackFireClip);
    }
}