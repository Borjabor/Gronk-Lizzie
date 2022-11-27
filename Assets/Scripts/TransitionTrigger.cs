using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    [SerializeField] private MovableObject _movableObject;

    public Animator _animator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && CharacterController_Light._isOnHeavy)
        {
            _movableObject.Activate();
            playAnimation();
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        
        if (other.gameObject.CompareTag("Player") && CharacterController_Light._isOnHeavy)
        {
            _movableObject.Activate();
            playAnimation();
        }
    }

    private void playAnimation()
    {
        _animator.SetBool("Together", true);
        _animator.SetTrigger("PlayerOn");
    }

}
