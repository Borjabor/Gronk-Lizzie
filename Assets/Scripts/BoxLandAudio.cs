using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxLandAudio : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        foreach (ContactPoint2D hitPos in other.contacts)
        {
            if (hitPos.normal.y > 0 && other.gameObject.layer == 6)
            {
                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
        }
    }
}
