using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Nucleon : MonoBehaviour
{
    public float attractionForce;
    public int direction;

    private Rigidbody body;

    private void Awake() {
        body = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (direction == 0) {
            body.AddForce((transform.localPosition+Vector3.right) * -attractionForce);        
        } else {
            body.AddForce((transform.localPosition+Vector3.left) * -attractionForce);        
        }
    }
}
