using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovingSphere : MonoBehaviour
{
    // public MyControls inputActions;

    // [SerializeField]
    // private Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

    [SerializeField, Range(0f, 100f)] private float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 10f;
    [SerializeField, Range(0f, 100f)] private float maxAirAcceleration = 1f;
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 2f;
    [SerializeField, Range(0f, 5f)] private int maxAirJumps = 0;
    [SerializeField, Range(0f, 90f)] private float maxGroundAngle = 25f;

    // [SerializeField, Range(0f, 1f)]
    // private float bounciness = 0.5f;
    private Rigidbody body;
    private Vector3 velocity;
    private Vector3 desiredVelocity;
    private bool desiredJump;
    private int groundContactCount;

    //readonly property
    private bool OnGround => groundContactCount > 0;
    
    private int jumpPhase;
    private float minGroundDotProduct;
    private Vector3 contactNormal;
    private int stepsSinceLastGrounded;

    private void OnValidate() {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }
    private void Awake() {
        body = GetComponent<Rigidbody>();
        OnValidate();
    }

    private void OnDestroy() {
    }

    private void Move(Vector2 input) {
        velocity = body.velocity;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        // if (velocity.x < desireVelocity.x) {
        //     velocity.x = Mathf.Min(velocity.x + maxSpeedChange, desireVelocity.x);
        // } else {
        //     velocity.x = Mathf.Max(velocity.x - maxSpeedChange, desireVelocity.x);
        // }

        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        
        // Vector3 acceleration = new Vector3(input.x, 0f, input.y) * maxSpeed;
        // velocity += acceleration * Time.deltaTime;
        Vector3 displacement = velocity * Time.deltaTime;
        Vector3 newPosition = transform.localPosition + displacement;

        // 判定是否在区域内
        // if (!allowedArea.Contains(new Vector2(newPosition.x, newPosition.z))) {
        //     newPosition.x = Mathf.Clamp(newPosition.x, allowedArea.xMin, allowedArea.xMax);
        //     newPosition.z = Mathf.Clamp(newPosition.z, allowedArea.yMin, allowedArea.yMax);
        //     newPosition = transform.localPosition;
        // }

        // 回弹
        // if (newPosition.x < allowedArea.xMin) {
        //     newPosition.x = allowedArea.xMin;
        //     velocity.x = -velocity.x * bounciness;
        // }
        // else if (newPosition.x > allowedArea.xMax) {
        //     newPosition.x = allowedArea.xMax;
        //     velocity.x = -velocity.x * bounciness;
        // }
        // if (newPosition.z < allowedArea.yMin) {
        //     newPosition.z = allowedArea.yMin;
        //     velocity.z = -velocity.z * bounciness;
        // }
        // else if(newPosition.z > allowedArea.xMax) {
        //     newPosition.z = allowedArea.xMax;
        //     velocity.z = -velocity.z * bounciness;
        // }

        // transform.localPosition = newPosition;
        body.velocity = velocity;
    }

    private void FixedUpdate() {
        UpdateState();
        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        if (desiredJump) {
            desiredJump = false;
            Jump();
        }
        body.velocity = velocity;
        // onGround = false;
        ClearState();
    }

    private void ClearState()
    {
        // onGround = false;
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    } 

    private void UpdateState()
    {
        stepsSinceLastGrounded += 1;
        velocity = body.velocity;
        if (OnGround || SnapToGround()) {
            stepsSinceLastGrounded = 0;
            jumpPhase = 0;
            if (groundContactCount > 1) {
                contactNormal.Normalize();
            }
        }
        else {
            contactNormal = Vector3.up;
        }
    }

    private bool SnapToGround() 
    {
        if (stepsSinceLastGrounded > 1) {
            return false;
        }
        return true;
    }
    
    private void Jump() {
        if (OnGround || jumpPhase < maxAirJumps) {
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            float alignedSpeed = Vector3.Dot(velocity, contactNormal);
            if (alignedSpeed > 0f) {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            velocity += contactNormal * jumpSpeed;
        }
    }

    private void Update() {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        desiredJump |= Input.GetButtonDown("Jump");
        GetComponent<Renderer>().material.SetColor(
            "_Color", OnGround ? Color.black : Color.white
        );
    }

    private void OnCollisionEnter(Collision other) {
        EvaluateCollision(other);
    }
    private void OnCollisionStay(Collision other) {
        EvaluateCollision(other);
    }

    private void EvaluateCollision(Collision collision) {
        for (int i = 0; i < collision.contactCount; i++) {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct) {
                groundContactCount += 1;
                // onGround = true;
                contactNormal += normal;
            }
        }
    }
    

    //Time.fixedDeltaTime is a value to set, not read
    //Time.deltaTime is the amount of time 
    //between frames the inverse of which would be your frames per second. It is a value you READ
    //You should use Time.deltaTime whether you're in FixedUpdate or Update.
    //It will return the actual amount of time that has elapsed 
    //between calls of the method you use it in 
    //(it automatically detects whether its in FixedUpdate or Update and 
    //returns the correct value).
}
