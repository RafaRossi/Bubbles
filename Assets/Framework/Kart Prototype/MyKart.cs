using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyKart : MonoBehaviour
{
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float friction = 0.98f; 
    
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private GroundChecker groundChecker;  

    [SerializeField] private Rigidbody rb;
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private AnimationCurve turnCurve;

    
    private float inputAcceleration;
    private float inputTurn;

    private const float CoyoteTime = 0.15f;
    private float coyoteTimeCounter;

    private bool IsGrounded => groundChecker.IsGrounded;
    private int _jumpCount = 0;


    private void Update()
    {
        inputAcceleration = Input.GetAxis("Vertical");
        inputTurn = Input.GetAxis("Horizontal");

        CheckGrounded();

        if (Input.GetButtonDown("Jump") && coyoteTimeCounter > 0)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        MoveKart();
        TurnKart();
    }
    
    private void MoveKart()
    {
        if (inputAcceleration != 0)
        {
            // Ensure speedFactor is never zero to prevent getting stuck
            float speedFactor = Mathf.Clamp(rb.velocity.magnitude / maxSpeed, 0.05f, 1f); 
            float curveMultiplier = accelerationCurve.Evaluate(speedFactor); // Get acceleration from curve
        
            // Prevent multiplier from reaching 0 to keep the kart moving
            curveMultiplier = Mathf.Max(curveMultiplier, 0.1f); 

            rb.AddForce(transform.forward * (inputAcceleration * acceleration * curveMultiplier), ForceMode.Acceleration);
        }

        // Limit max speed
        Vector3 velocity = rb.velocity;
        //velocity.y = 0; // Ignore Y velocity
        if (velocity.magnitude > maxSpeed)
        {
            rb.velocity = velocity.normalized * maxSpeed;
        }

        // Apply friction when no input
        rb.velocity *= friction;
    }

    private void TurnKart()
    {
        if (rb.velocity.magnitude > 1f)
        {
            float speedFactor = rb.velocity.magnitude / maxSpeed;
            float curveFactor = turnCurve.Evaluate(speedFactor);
            float turn = inputTurn * turnSpeed * curveFactor * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
    
    private void Jump()
    {
        if (!IsGrounded || _jumpCount > 0) return;
        
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        _jumpCount++;
    }
    

    private void CheckGrounded()
    {
        if (IsGrounded)
        {
            coyoteTimeCounter = CoyoteTime;
            _jumpCount = 0;
        }
        else
        {
            //rb.AddForce(-transform.up * gravityMultiplier, ForceMode.Acceleration);
            
            coyoteTimeCounter -= Time.deltaTime;
        }
    }
}
