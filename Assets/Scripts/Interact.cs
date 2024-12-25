using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private Transform _rayPoint;
    [SerializeField] private float _rayDistance;
    [SerializeField] private LayerMask _layerIndex;
    [SerializeField] private GameObject _canInteract;
    private LeverScript _lever;

    private void Awake()
    {
        _canInteract.SetActive(false);
    }

    private void Update()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(_rayPoint.position, direction, _rayDistance, _layerIndex);
        Debug.DrawRay(_rayPoint.position, direction * _rayDistance, Color.red);

        if (hit.collider != null)
        {
            _canInteract.SetActive(true);
        }
        else
        {
            _canInteract.SetActive(false);
        }
        

        if (Input.GetKeyDown(KeyCode.RightControl) && hit.collider != null)
        {
            _lever = hit.collider.GetComponent<LeverScript>();
            _lever.ChangeState();

        }else if (Input.GetKeyUp(KeyCode.RightControl))
        {
            
        }
    }
}
