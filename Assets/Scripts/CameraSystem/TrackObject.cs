using UnityEngine;

public class TrackObject : MonoBehaviour
{
    private Camera _camera;
    public void SetCamera(Camera newCamera) =>
        _camera = newCamera;
    private void Awake()
    {
        CameraController.RegisterTrackObject(this);
    }
}