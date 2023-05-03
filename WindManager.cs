using UnityEngine;
using System.Collections.Generic;

public class WindManager : MonoBehaviour
{
    public Vector3 windDirection = Vector3.right;
    private Vector3 initialWindDirection;
 
    public float windForce = 10.0f;
    public float dayLengthInSeconds = 60.0f;
    public float turbulenceIntensity = 1.0f;
    public float turbulenceScale = 0.1f;
    
    public bool applyTurbulence = false;
    public bool dynamicRigidbodies = true;
    public bool dynamicWind = false;

    private List<Rigidbody> cachedRigidbodies;
    private float elapsedTime = 0.0f;
    private float initialWindForce;

    private void Start()
    {
        if (!dynamicRigidbodies)
        {
            CacheRigidbodies();
        }

        initialWindDirection = windDirection;
        initialWindForce = windForce;
    }

    private void FixedUpdate()
    {
        List<Rigidbody> allRigidbodies = dynamicRigidbodies ? new List<Rigidbody>(GameObject.FindObjectsOfType<Rigidbody>()) : cachedRigidbodies;

        if (dynamicWind)
        {
            UpdateWind();
        }
        
        if (applyTurbulence)
        {
            ApplyTurbulence();
        }
        
        foreach (Rigidbody rb in allRigidbodies)
        {
            if (rb != null && !rb.isKinematic)
            {
                rb.AddForce(windDirection.normalized * windForce);
            }
        }
    }

    private void ApplyTurbulence()
    {
        float xTurbulence = Mathf.PerlinNoise(Time.time * turbulenceScale, 0) * 2 - 1;
        float yTurbulence = Mathf.PerlinNoise(0, Time.time * turbulenceScale) * 2 - 1;
        float zTurbulence = Mathf.PerlinNoise(Time.time * turbulenceScale, Time.time * turbulenceScale) * 2 - 1;

        Vector3 turbulence = new Vector3(xTurbulence, yTurbulence, zTurbulence) * turbulenceIntensity;
        windDirection = initialWindDirection + turbulence;

        float forceTurbulence = (Mathf.PerlinNoise(Time.time * turbulenceScale, 1000) * 2 - 1) * turbulenceIntensity;
        windForce = initialWindForce + forceTurbulence;
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
