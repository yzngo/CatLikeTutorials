using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovingSphere : MonoBehaviour
{
    // public MyControls inputActions;

    [SerializeField]
    private Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

    [SerializeField, Range(0f, 100f)]
    private float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    private float maxAcceleration = 10f;

    [SerializeField, Range(0f, 1f)]
    private float bounciness = 0.5f;

    private Vector3 velocity;
    private void Awake() {
        // inputActions = new MyControls();
        // inputActions.Enable();
        // inputActions.Player.Move.performed += ctx =>
        // {
        //     Move(ctx.ReadValue<Vector2>());
        // };
    }

    private void OnDestroy() {
        // inputActions.Disable();
    }

    private void Move(Vector2 input) {
        Vector3 desireVelocity = new Vector3(input.x, 0f, input.y) * maxSpeed;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        // if (velocity.x < desireVelocity.x) {
        //     velocity.x = Mathf.Min(velocity.x + maxSpeedChange, desireVelocity.x);
        // } else {
        //     velocity.x = Mathf.Max(velocity.x - maxSpeedChange, desireVelocity.x);
        // }

        velocity.x = Mathf.MoveTowards(velocity.x, desireVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desireVelocity.z, maxSpeedChange);
        
        // Vector3 acceleration = new Vector3(input.x, 0f, input.y) * maxSpeed;
        // velocity += acceleration * Time.deltaTime;
        Vector3 displacement = velocity * Time.deltaTime;
        Vector3 newPosition = transform.localPosition + displacement;
        // if (!allowedArea.Contains(new Vector2(newPosition.x, newPosition.z))) {
        //     newPosition.x = Mathf.Clamp(newPosition.x, allowedArea.xMin, allowedArea.xMax);
        //     newPosition.z = Mathf.Clamp(newPosition.z, allowedArea.yMin, allowedArea.yMax);
        //     newPosition = transform.localPosition;
        // }
        if (newPosition.x < allowedArea.xMin) {
            newPosition.x = allowedArea.xMin;
            velocity.x = -velocity.x * bounciness;
        }
        else if (newPosition.x > allowedArea.xMax) {
            newPosition.x = allowedArea.xMax;
            velocity.x = -velocity.x * bounciness;
        }
        if (newPosition.z < allowedArea.yMin) {
            newPosition.z = allowedArea.yMin;
            velocity.z = -velocity.z * bounciness;
        }
        else if(newPosition.z > allowedArea.xMax) {
            newPosition.z = allowedArea.xMax;
            velocity.z = -velocity.z * bounciness;
        }

        transform.localPosition = newPosition;
        // transform.Translate(new Vector3(move.x, 0f, move.y));
        // transform.localPosition = new Vector3(move.x, 0f, move.y);
    }
    private void Update() {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        // playerInput.Normalize();
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        Move(playerInput);
    }
}
