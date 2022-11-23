using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    [SerializeField] private MovableObject _movableObject;
    private bool _isOn = false;

    public void ChangeState()
    {
        if (!_isOn)
        {
            _movableObject.Activate();
            _isOn = true;
        }
        else
        {
            _movableObject.Deactivate();
            _isOn = false;
        }
    }
    
}
