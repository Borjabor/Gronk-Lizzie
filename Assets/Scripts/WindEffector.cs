using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffector : MonoBehaviour
{
    [SerializeField] private AreaEffector2D _wind;
    [SerializeField] private Transform _rayPoint;
    [SerializeField] private float _rayDistance;
    [SerializeField] private LayerMask _layerIndex;


    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(_rayPoint.position, Vector2.up, _rayDistance, _layerIndex);
        
        if (hit.collider != null)
        {
            _wind.enabled = false;
        }
        else
        {
            _wind.enabled = true;
        }
    }
}
