using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    [SerializeField] private MovableObject _movableObject;
    private bool _isOn = false;

    public Animator _animator;
    public Animator _LeverAnimator;

    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _onAudio;
    [SerializeField]
    private AudioClip _offAudio;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void ChangeState()
    {
        if (!_isOn)
        {
            _movableObject.Activate();
            _animator.SetBool("DoorActive", true);
            _LeverAnimator.SetBool("LeverActive", true);
            _audioSource.PlayOneShot(_onAudio);
            _isOn = true;
        }
        else
        {
            _movableObject.Deactivate();
            _animator.SetBool("DoorActive", false);
            _LeverAnimator.SetBool("LeverActive", false);
            _audioSource.PlayOneShot(_offAudio);
            _isOn = false;
        }
    }
    
}
