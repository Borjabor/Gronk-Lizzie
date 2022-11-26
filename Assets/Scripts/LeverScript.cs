using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    [SerializeField] private MovableObject _movableObject;
    private bool _isOn = false;

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
            _audioSource.PlayOneShot(_onAudio);
            _isOn = true;
        }
        else
        {
            _movableObject.Deactivate();
            _audioSource.PlayOneShot(_offAudio);
            _isOn = false;
        }
    }
    
}
