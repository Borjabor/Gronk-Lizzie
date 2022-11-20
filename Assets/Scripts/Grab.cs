using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    [SerializeField] private Transform _grabPoint;
    [SerializeField] private Transform _rayPoint;
    [SerializeField] private float _rayDistance;

    private GameObject _grabbedObject;
    private int _layerIndex;

    private void Start()
    {
        _layerIndex = LayerMask.NameToLayer("Objects");
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(_rayPoint.position, transform.right, _rayDistance);
        Debug.DrawRay(_rayPoint.position, transform.right * _rayDistance, Color.red);

        if (hit.collider != null && hit.collider.gameObject.layer == _layerIndex)
        {
            if (Input.GetKey(KeyCode.Space) && _grabbedObject == null)
            {
                Debug.Log($"grab");
                    _grabbedObject = hit.collider.gameObject;
                _grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                _grabbedObject.transform.position = _grabPoint.position;
                _grabbedObject.transform.SetParent(transform);

            }else if (Input.GetKeyUp(KeyCode.Space))
            {
                _grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                _grabbedObject.transform.SetParent(null);
                _grabbedObject = null;
            }
        }
    }
}
