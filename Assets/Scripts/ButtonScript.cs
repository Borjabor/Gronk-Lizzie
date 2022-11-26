using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    private Vector3 _oringialPos;
    [Tooltip("Drag button here")]
    [SerializeField] private GameObject _targetPos;
    bool moveBack = false;
    [Tooltip("Drag door you want to open here")]
    //[SerializeField] private DoorOpen _doorOpen;
    [SerializeField] private MovableObject _movableObject;

    public Animator button;
    public Animator animator;

    private AudioSource _audioSource;
    private bool _hasPlayed = false;
    
    private void Start()
    {
        _oringialPos = transform.position;
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Heavy" || collision.transform.tag == "Objects")
        {
            //transform.Translate(0, -0.01f, 0);
            transform.position = Vector2.MoveTowards(transform.position, _targetPos.transform.position, Time.deltaTime);
            moveBack = false;
            if (!_hasPlayed && transform.position.y == _targetPos.transform.position.y)
            {
                _audioSource.Play();
                _hasPlayed = true;
            }
            _movableObject.Activate();
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Heavy" || collision.transform.tag == "Objects")
        {
            collision.transform.parent = transform;
        }
        //button.SetBool("Button_State", false);
        moveBack = true;
        collision.transform.parent = null;
        _hasPlayed = false;
        _movableObject.Deactivate();
    }

    private void Update()
    {
        if (moveBack)
        {
            if (transform.position.y < _oringialPos.y)
            {
                transform.Translate(0, 0.01f, 0);
            }
            else
            {
                moveBack = false;
            }
        }
    }
}
