using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    [SerializeField] private MovableObject _movableObject;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && CharacterController_Light._isOnHeavy)
        {
            _movableObject.Activate();
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        
        if (other.gameObject.CompareTag("Player") && CharacterController_Light._isOnHeavy)
        {
            _movableObject.Activate();
        }
    }

}
