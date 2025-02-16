using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartSuspension : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float strength = 6f;
    [SerializeField] private float damping = 6f;

    [SerializeField] private float suspensionLength = 0.4f;
    [SerializeField] private List<Transform> suspensionTargets = new List<Transform>();
    
    private void FixedUpdate()
    {
        foreach (var suspensionTarget in suspensionTargets)
        {
            ApplyForce(suspensionTarget);
        }
    }

    private void ApplyForce(Transform suspension)
    {
        var rayDidHit = Physics.Raycast(suspension.position, -suspension.up, out var hit, suspensionLength);
            
        if (rayDidHit)
        {
            var springDir = suspension.up;

            var worldSpaceVelocity = rb.GetPointVelocity(suspension.position);

            var offset = suspensionLength - hit.distance;

            var vel = Vector3.Dot(springDir, worldSpaceVelocity);

            var force = (offset * strength) - (vel * damping);
                
            rb.AddForceAtPosition(springDir * force, suspension.position);
        }
    }
}
