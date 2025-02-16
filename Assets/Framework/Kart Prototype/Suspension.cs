using System;
using UnityEngine;

namespace Framework.Kart_Prototype
{
    public interface IForce
    {
        void ApplyForce();
    }

    public class Suspension : MonoBehaviour, IForce
    {
        [SerializeField] private float restDistance;

        [SerializeField] private float strength;
        [SerializeField] private float damping;

        [SerializeField] private Rigidbody targetRigidbody;

        private void FixedUpdate()
        {
            ApplyForce();
        }

        public void ApplyForce()
        {
            var rayDidHit = Physics.Raycast(transform.position, -transform.up, out var hit, restDistance);
            
            if (rayDidHit)
            {
                var springDir = transform.up;

                var worldSpaceVelocity = targetRigidbody.GetPointVelocity(transform.position);

                var offset = restDistance - hit.distance;

                var vel = Vector3.Dot(springDir, worldSpaceVelocity);

                var force = (offset * strength) - (vel * damping);
                
                targetRigidbody.AddForceAtPosition(springDir * force, transform.position);
            }
        }
    }
}