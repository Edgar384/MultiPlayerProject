using UnityEngine;

public class CarShowoff : MonoBehaviour
{

    //Should be scriptable object. Now just for the check
    [SerializeField] private bool _rotateCar;
    [SerializeField] private float _rotationSpeed;

    private void Update()
    {
        if (_rotateCar) 
        {
           transform.Rotate(0, _rotationSpeed*Time.deltaTime ,0);
        }
    }
}
