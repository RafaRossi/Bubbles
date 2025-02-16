using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    
    [SerializeField] private float groundOffset = 0.3f;
    [SerializeField] private float springStrength = 0.3f;
    [SerializeField] private float springDamper = 0.3f;
    
    private void FixedUpdate()
    {
        var didHit = Physics.Raycast(transform.position, -transform.up, out var hit, groundOffset);

        if (didHit)
        {
            var velocity = rb.velocity;
            var rayDir = transform.TransformDirection(-Vector3.up);

            var otherVel = Vector3.zero;
            var hitBody = hit.rigidbody;

            if (hitBody)
            {
                otherVel = hitBody.velocity;
            }

            var rayDirVel = Vector3.Dot(rayDir, velocity);
            var otherDirVel = Vector3.Dot(rayDir, otherVel);

            var relVel = rayDirVel - otherDirVel;

            var x = hit.distance - groundOffset;

            var springForce = (x * springStrength) - (relVel * springDamper);
            
            rb.AddForce(rayDir * springForce);

            if (hitBody)
            {
                hitBody.AddForceAtPosition(rayDir * -springForce, hit.point);
            }
        }
    }
}
