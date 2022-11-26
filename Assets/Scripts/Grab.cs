using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    [Header("Audio")]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _grabAudio;

    
    
    
    [SerializeField] private Transform _grabPoint;
    [SerializeField] private Transform _rayPoint;
    [SerializeField] private float _rayDistance;

    private Collider2D _col;
    private GameObject _grabbedObject;
    [SerializeField] private LayerMask _layerIndex;

    private void Start()
    {
        //_layerIndex = LayerMask.NameToLayer("Objects");
        _col = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(_rayPoint.position, direction, _rayDistance, _layerIndex);
        Debug.DrawRay(_rayPoint.position, direction * _rayDistance, Color.red);

        if (hit.collider != null /*&& hit.collider.gameObject.layer == _layerIndex*/)
        {
            //Debug.Log($"hit");
            if (Input.GetKey(KeyCode.Space) && _grabbedObject == null)
            {
                //Debug.Log($"grab");
                _audioSource.PlayOneShot(_grabAudio);
                _grabbedObject = hit.collider.gameObject;
                //_grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                var grabbedRb = _grabbedObject.GetComponent<Rigidbody2D>();
                var grabbedCol = _grabbedObject.GetComponent<Collider2D>();
                _grabbedObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                _grabbedObject.transform.position = _grabPoint.position;
                _grabbedObject.transform.SetParent(transform);
                Physics2D.IgnoreCollision(_col, grabbedCol, true);
                FixedJoint2D fj = transform.gameObject.AddComponent(typeof(FixedJoint2D)) as FixedJoint2D;
                fj.connectedBody = grabbedRb;

            }else if (Input.GetKeyUp(KeyCode.Space))
            {
                //_grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                _grabbedObject.GetComponent<Rigidbody2D>().gravityScale = 1;
                var grabbed = _grabbedObject.GetComponent<Collider2D>();
                _grabbedObject.transform.SetParent(null);
                _grabbedObject = null;
                Physics2D.IgnoreCollision(_col, grabbed, false);
                Destroy(GetComponent<FixedJoint2D>());
            }
        }
    }
}
