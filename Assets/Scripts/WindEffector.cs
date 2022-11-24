using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffector : MonoBehaviour
{
    [SerializeField] private Transform _rayPoint;
    [SerializeField] private float _rayDistance;
    [SerializeField] private LayerMask _layerIndex;
    private BoxCollider2D _collider;
    private Vector2 _originalArea;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _rayDistance = _collider.bounds.size.y;
        _originalArea = _collider.size;
    }


    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(_rayPoint.position, Vector2.up, _rayDistance, _layerIndex);
        
        if (hit.collider != null)
        {
            var halfRay = _rayDistance / 2;
            _collider.size = new Vector2(_collider.size.x, hit.point.y/_rayDistance);
            _collider.offset = new Vector2(_collider.offset.x, (1 - hit.point.y/_rayDistance) / 2 * -1);
        }
        else
        {
            _collider.size = _originalArea;
            _collider.offset = Vector2.zero;
        }
    }
}