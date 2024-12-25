using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounceable : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Collider2D _col;
    [SerializeField] private float _force = 10f;
    [SerializeField] private LayerMask _layerMask;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
    }

    public void Bounce()
    {
        RaycastHit2D hit = Physics2D.Raycast(_col.bounds.center, Vector2.down, _col.bounds.extents.y + 0.1f, _layerMask);
        bool grounded = hit.collider != null;
        if(grounded)_rb.velocity = Vector2.up * _force;
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(_col.bounds.center, Vector2.down, _col.bounds.extents.y + 0.1f, _layerMask);
        Color rayColor;
        if (hit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(_col.bounds.center, Vector2.down * (_col.bounds.extents.y + 0.1f));
    }
}
