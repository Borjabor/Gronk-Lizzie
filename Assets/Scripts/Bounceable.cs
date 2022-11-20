using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounceable : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float _force = 10f;
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Bounce()
    {
        _rb.velocity = Vector2.up * _force;
    }
}
