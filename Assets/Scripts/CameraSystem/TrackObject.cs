using UnityEngine;

public class TrackObject : MonoBehaviour
{
    private void Start()
    {
        CameraController.RegisterTrackObject(this);
    }
}