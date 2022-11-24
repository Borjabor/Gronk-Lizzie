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
        _rayDistance = _collider.bounds.size.y;
    }


    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(_rayPoint.position, Vector2.up, _rayDistance, _layerIndex);
        Debug.DrawRay(_rayPoint.position, Vector2.up * _rayDistance);
        
        if (hit.collider != null)
        {
            //_wind.enabled = false;
            _collider.size = new Vector2(_collider.size.x, hit.point.y/_rayDistance);
            _collider.offset = new Vector2(_collider.offset.x, (1 - hit.point.y) / 2 * -1);
            //_collider.c
            Debug.Log($"{_collider.size}");
            //this is working, but the offset needs to change too
        }
        else
        {
            _wind.enabled = true;
        }
    }
}
