using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KartController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Wheel[] wheels;
    
    [SerializeField] private LayerMask drivable;

    [Header("Suspension Settings")] 
    [SerializeField] private float springStiffness = 30000f;
    [SerializeField] private float damperStiffness = 3000f;
    [SerializeField] private float restLength = 0.2f;
    [SerializeField] private float springTravel = 0.1f;
    [SerializeField] private float wheelRadius = 0.05f;
    
    [Header("Acceleration/Braking")]
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float maxSpeed = 50f;

    [SerializeField] private float dragForce = 5f;
    [SerializeField] private float lowSpeedDrag = 10f;

    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private AnimationCurve decelerationCurve;

    [Header("Steering Settings")] 
    [SerializeField] private float steeringStrength = 10f;
    [SerializeField] private float dragCoefficient = 100f;

    [SerializeField] private AnimationCurve steeringCurve;

    private float moveInput;
    private float steerInput;

    private const float InputThreshold = 0.1f;

    private float steeringMultiplier = 1;
    private float dragMultiplier = 1;

    private bool IsGrounded => wheels.Any(wheel => wheel.isGrounded);
    
    public float CurrentSpeed => rb.velocity.magnitude;

    private void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");

        steeringMultiplier = Input.GetButton("Jump") ? 2 : 1;
        dragMultiplier = Input.GetButton("Jump") ? 0.5f : 1;
    }

    private void FixedUpdate()
    {
        Suspension();
        
        Movement();

        SidewaysDrag();
        
        Debug.DrawRay(transform.position, rb.velocity * acceleration, Color.blue);
    }
    
    private void Movement()
    {
        if(!IsGrounded) return;

        if (Mathf.Abs(moveInput) > InputThreshold)
        {
            Acceleration();
        }
        else
        { 
            Deceleration();
        }
        
        Turn();
    }
    
    private void Acceleration()
    {
        var speedForward = Vector3.Dot(rb.velocity, transform.forward);
        var absSpeed = Mathf.Abs(speedForward);

        var speedRatio = absSpeed / maxSpeed;
        
        const float threshold = 0.1f;

        if (absSpeed < maxSpeed)
        {
            var curveValue = accelerationCurve.Evaluate(speedRatio);
            rb.AddForceAtPosition(transform.forward * (acceleration * moveInput * curveValue), transform.position, ForceMode.Acceleration);
        }
        else if (speedForward * moveInput > threshold) 
        {
            rb.AddForceAtPosition(-transform.forward * ((absSpeed - maxSpeed) * 0.5f), transform.position, ForceMode.Acceleration);
        }
    }
    
    private void Deceleration()
    {
        var speedForward = Vector3.Dot(rb.velocity, transform.forward);
        var absSpeed = Mathf.Abs(speedForward);
        var speedRatio = absSpeed / maxSpeed;

        var curveValue = decelerationCurve.Evaluate(speedRatio);

        var resistance = dragForce * curveValue;

        if (absSpeed < 1f)
        {
            resistance = lowSpeedDrag;
        }
        
        rb.AddForce(-rb.velocity.normalized * resistance, ForceMode.Acceleration);
    }

    private void Turn()
    {
        if (!(rb.velocity.magnitude > 0.1f)) return;
        
        var speedForward = Vector3.Dot(rb.velocity, transform.forward);
        var absSpeed = Mathf.Abs(speedForward);
        var speedRatio = absSpeed / maxSpeed;

        var steeringEffect = steeringCurve.Evaluate(speedRatio);
            
        rb.AddTorque(steeringStrength * steeringMultiplier * steerInput * steeringEffect * transform.up, ForceMode.Acceleration);
    }

    private void SidewaysDrag()
    {
        var steeringDirection = transform.right;

        var worldSpaceVelocity = rb.GetPointVelocity(transform.position);

        var steeringVelocity = Vector3.Dot(steeringDirection, worldSpaceVelocity);
        var desiredVelocityChange = -steeringVelocity * dragCoefficient * dragMultiplier;

        var desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;
                
        rb.AddForceAtPosition(steeringDirection * (desiredAcceleration), transform.position);
    }
    
    private void Suspension()
    {
        foreach (var wheel in wheels)
        {
            var rayPoint = wheel.transform;
            
            var maxLength = restLength + springTravel;

            if (Physics.Raycast(rayPoint.position, -rayPoint.up, out var hit, maxLength + wheelRadius, drivable))
            {
                var currentSpringLenght = hit.distance - wheelRadius;
                var springCompression = (restLength - currentSpringLenght) / springTravel;

                var springVelocity = Vector3.Dot(rb.GetPointVelocity(rayPoint.position), rayPoint.up);
                var dampForce = damperStiffness * springVelocity;

                var springForce = springStiffness * springCompression;

                var netForce = springForce - dampForce;
                
                rb.AddForceAtPosition(netForce * rayPoint.up, rayPoint.position);
                
                Debug.DrawLine(rayPoint.position, hit.point, Color.red);

                wheel.isGrounded = true;
            }
            else
            {
                wheel.isGrounded = false;
                Debug.DrawLine(rayPoint.position, rayPoint.position + (wheelRadius + maxLength) * -rayPoint.up, Color.green);
            }
        }
    }
}

[Serializable]
public class Wheel
{
    public Transform transform;
    public bool isGrounded;
}
