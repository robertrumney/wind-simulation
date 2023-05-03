using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    public float windForce = 10.0f;
    public Vector3 windDirection = Vector3.right;
    public bool dynamicRigidbodies = true;

    private List<Rigidbody> cachedRigidbodies;

    private void Start()
    {
        if (!dynamicRigidbodies)
        {
            cacheRigidbodies();
        }
    }

    private void FixedUpdate()
    {
        List<Rigidbody> allRigidbodies = dynamicRigidbodies ? new List<Rigidbody>(GameObject.FindObjectsOfType<Rigidbody>()) : cachedRigidbodies;

        foreach (Rigidbody rb in allRigidbodies)
        {
            if (rb != null && !rb.isKinematic)
            {
                rb.AddForce(windDirection.normalized * windForce);
            }
        }
    }

    private void CacheRigidbodies()
    {
        cachedRigidbodies = new List<Rigidbody>(GameObject.FindObjectsOfType<Rigidbody>());
    }
}
