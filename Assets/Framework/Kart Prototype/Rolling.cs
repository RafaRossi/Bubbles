using System;
using UnityEngine;

namespace Framework.Kart_Prototype
{
    public class Rolling : MonoBehaviour, IForce
    {
        [SerializeField] private Rigidbody targetRigidbody;
        [SerializeField] private float maxSpeed;

        [SerializeField] private AnimationCurve powerCurve;

        private float accelInput;
        
        private void FixedUpdate()
        {
            ApplyForce();
        }

        private void Update()
        {
            accelInput = Input.GetAxis("Vertical");
        }

        public void ApplyForce()
        {
            if (Mathf.Abs(accelInput) > 0)
            {
                var speed = Vector3.Dot(transform.forward, targetRigidbody.velocity);

                var normalizedSpeed = Mathf.Clamp01(Mathf.Abs(speed) / maxSpeed);

                var availableTorque = powerCurve.Evaluate(normalizedSpeed) * accelInput;
                
                targetRigidbody.AddForceAtPosition(transform.forward * availableTorque, transform.position);
            }
        }
    }
}
