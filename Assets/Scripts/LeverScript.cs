using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    [SerializeField] private MovableObject _movableObject;
    private bool _isOn = false;

    public Animator _animator;

    public void ChangeState()
    {
        if (!_isOn)
        {
            _movableObject.Activate();
            _animator.SetBool("DoorActive", true);
            _isOn = true;
        }
        else
        {
            _movableObject.Deactivate();
            _animator.SetBool("DoorActive", false);
            _isOn = false;
        }
    }
    
}
