using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MovableObject : MonoBehaviour
{
    [SerializeField] private GameObject _targetPos;
    private Vector2 _startPos;
    [SerializeField] private float _openSpeed = 4f;
    private bool _isOpening = false;
    private AudioSource _audioSource;

    private void Awake()
    {
        //GoalReachable.enabled = false;
        _startPos = transform.position;
        _audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        //Debug controls
         // if (Input.GetKeyDown(KeyCode.Alpha2))
         // {
         //     _isOpening = true;
         // }
         // if (Input.GetKeyDown(KeyCode.Alpha1))
         // {
         //     _isOpening = false;
         // }

        if (_isOpening)
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetPos.transform.position, _openSpeed * Time.deltaTime);
            //if(!_audioSource.isPlaying && transform.position.y != _targetPos.transform.position.y) _audioSource.Play();
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, _startPos, _openSpeed * Time.deltaTime);
            //if(!_audioSource.isPlaying && transform.position.y != _startPos.y) _audioSource.Play();
        }
        
        //StartCoroutine(FadeCoroutine());
    }

    public void Activate()
    {
        _isOpening = true;
        //transform.position = Vector2.MoveTowards(transform.position, _targetPos.transform.position, _openSpeed * Time.deltaTime);
    }
    
    public void Deactivate()
    {
        _isOpening = false;
        //transform.position = Vector2.MoveTowards(transform.position, _startPos, _openSpeed * Time.deltaTime);
    }

}
