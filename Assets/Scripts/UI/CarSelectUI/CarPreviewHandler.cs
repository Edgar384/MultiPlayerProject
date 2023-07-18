using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPreviewHandler : MonoBehaviour
{
    [SerializeField] CarShowoff[] _carShowoff = new CarShowoff[4];


    //Ugly but fast. Can fix and be smarter later
    public void ChangeCarPreview(int carIndex)
    {
        for (int i = 0; i < _carShowoff.Length; i++)
        {
            if(i!=carIndex)
            _carShowoff[i].SetActive(false);

            else
                _carShowoff[i].SetActive(true);
        }
    }
}
