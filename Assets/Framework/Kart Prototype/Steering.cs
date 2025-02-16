using System;
using UnityEngine;

namespace Framework.Kart_Prototype
{
    public class Steering : MonoBehaviour, IForce
    {
        [Range(0, 1)] [SerializeField] private float gripFactor;
        [SerializeField] private float mass;

        [SerializeField] private Rigidbody targetRigidbody;

        private void FixedUpdate()
        {
            ApplyForce();
        }

        public void ApplyForce()
        {
            var steeringDirection = transform.right;

            var worldSpaceVelocity = targetRigidbody.GetPointVelocity(transform.position);

            var steeringVelocity = Vector3.Dot(steeringDirection, worldSpaceVelocity);
            var desiredVelocityChange = -steeringVelocity * gripFactor;

            var desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;
                
            targetRigidbody.AddForceAtPosition(steeringDirection * (desiredAcceleration), transform.position);
        }

    }
}
