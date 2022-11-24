using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffector : MonoBehaviour
{
    [SerializeField] private Transform _rayPoint;
    [SerializeField] private float _rayDistance;
    [SerializeField] private LayerMask _layerIndex;
    private AreaEffector2D _wind;
    private BoxCollider2D _collider;
    private Vector3 _area;

    private void Start()
    {
        _wind = GetComponent<AreaEffector2D>();
        _collider = GetComponent<BoxCollider2D>();
    }


    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(_rayPoint.position, Vector2.up, _collider.bounds.size.y, _layerIndex);
        
        if (hit.collider != null)
        {
            //_wind.enabled = false;
            _collider.size = new Vector2(_collider.size.x, hit.point.y);
            Debug.Log($"{_collider.size}");
            //this is working, but the offset needs to change too
        }
        else
        {
            _wind.enabled = true;
        }
    }
}
