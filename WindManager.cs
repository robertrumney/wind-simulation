using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    public float windForce = 10.0f;
    public Vector3 windDirection = Vector3.right;
    public bool dynamicRigidbodies = true;
    public bool dynamicWind = false;
    public float dayLengthInSeconds = 60.0f;

    private List<Rigidbody> cachedRigidbodies;
    private float elapsedTime = 0.0f;

    private void Start()
    {
        if (!dynamicRigidbodies)
        {
            CacheRigidbodies();
        }
    }

    private void FixedUpdate()
    {
        List<Rigidbody> allRigidbodies = dynamicRigidbodies ? new List<Rigidbody>(GameObject.FindObjectsOfType<Rigidbody>()) : cachedRigidbodies;

        if (dynamicWind)
        {
            UpdateWind();
        }

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

    private void UpdateWind()
    {
        elapsedTime += Time.fixedDeltaTime;

        float dayProgress = elapsedTime / dayLengthInSeconds;

        float angle = 360.0f * dayProgress;
        float forceVariation = Mathf.Sin(2 * Mathf.PI * dayProgress);

        windDirection = Quaternion.Euler(0, angle, 0) * Vector3.right;
        windForce += forceVariation;

        if (elapsedTime >= dayLengthInSeconds)
        {
            elapsedTime = 0.0f;
        }
    }
}
