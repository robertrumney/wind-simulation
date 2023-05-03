using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    public float windForce = 10.0f;
    public Vector3 windDirection = Vector3.right;

    private void FixedUpdate()
    {
        Rigidbody[] allRigidbodies = GameObject.FindObjectsOfType<Rigidbody>();

        foreach (Rigidbody rb in allRigidbodies)
        {
            if (!rb.isKinematic)
            {
                rb.AddForce(windDirection.normalized * windForce);
            }
        }
    }
}
