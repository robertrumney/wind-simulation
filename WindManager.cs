using UnityEngine;
using System.Collections.Generic;

public class WindManager : MonoBehaviour
{
    [Header("Base Wind")]
    public Vector3 baseDirection = Vector3.right;
    public float baseForce = 10f;
    public float minForce = 0f;
    public float maxForce = 40f;
    public ForceMode forceMode = ForceMode.Acceleration;

    [Header("Day Cycle")]
    public bool dynamicWind = true;
    public float dayLengthInSeconds = 120f;
    public AnimationCurve forceOverDay = AnimationCurve.EaseInOut(0, 1, 1, 1);
    public AnimationCurve yawOverDay = AnimationCurve.Linear(0, 0, 1, 360);

    [Header("Turbulence")]
    public bool applyTurbulence = true;
    public float turbulenceIntensity = 1f;
    public float turbulenceScale = 0.1f;
    public int turbulenceOctaves = 2;

    [Header("Gusts")]
    public bool gustsEnabled = true;
    public float gustIntensity = 1.5f;
    public float gustFrequency = 0.2f;
    public float gustDuration = 1.5f;
    public AnimationCurve gustEnvelope = AnimationCurve.EaseInOut(0, 0, 1, 0);

    [Header("Targets")]
    public bool dynamicRigidbodies = true;
    public float rescanInterval = 0.5f;
    public LayerMask affectedLayers = ~0;
    public bool areaLimited = false;
    public BoxCollider areaBounds;

    [Header("Determinism")]
    public int noiseSeed = 0;

    private readonly List<Rigidbody> targets = new List<Rigidbody>();
    private float rescanTimer;
    private float dayClock;
    private Vector3 noiseOffsetA;
    private Vector3 noiseOffsetB;
    private float gustTimer;
    private bool gustActive;
    private float gustPhase;
    private float fixedT;

    void OnEnable()
    {
        if (!dynamicRigidbodies) Rescan();
        var rnd = noiseSeed == 0 ? new System.Random() : new System.Random(noiseSeed);
        noiseOffsetA = new Vector3((float)rnd.NextDouble() * 1000f, (float)rnd.NextDouble() * 1000f, (float)rnd.NextDouble() * 1000f);
        noiseOffsetB = new Vector3((float)rnd.NextDouble() * 1000f, (float)rnd.NextDouble() * 1000f, (float)rnd.NextDouble() * 1000f);
    }

    void FixedUpdate()
    {
        fixedT += Time.fixedDeltaTime;
        if (dynamicRigidbodies)
        {
            rescanTimer -= Time.fixedDeltaTime;
            if (rescanTimer <= 0f)
            {
                Rescan();
                rescanTimer = rescanInterval;
            }
        }

        if (dynamicWind && dayLengthInSeconds > 0f)
        {
            dayClock += Time.fixedDeltaTime / dayLengthInSeconds;
            if (dayClock >= 1f) dayClock -= 1f;
        }

        if (gustsEnabled)
        {
            if (!gustActive)
            {
                gustTimer -= Time.fixedDeltaTime;
                if (gustTimer <= 0f)
                {
                    if (Random.value < gustFrequency * Time.fixedDeltaTime)
                    {
                        gustActive = true;
                        gustPhase = 0f;
                    }
                    gustTimer = 0.25f;
                }
            }
            else
            {
                gustPhase += Time.fixedDeltaTime / Mathf.Max(0.001f, gustDuration);
                if (gustPhase >= 1f)
                {
                    gustActive = false;
                    gustPhase = 0f;
                }
            }
        }

        Vector3 dir = baseDirection.normalized;
        if (dynamicWind)
        {
            float yaw = yawOverDay.Evaluate(dayClock);
            dir = Quaternion.Euler(0f, yaw, 0f) * Vector3.right;
        }

        if (applyTurbulence)
        {
            Vector3 turb = Turbulence(fixedT, turbulenceScale, turbulenceOctaves) * turbulenceIntensity;
            dir = (dir + turb).normalized;
        }

        float forceMul = dynamicWind ? Mathf.Max(0f, forceOverDay.Evaluate(dayClock)) : 1f;
        if (applyTurbulence) forceMul *= Mathf.Clamp01(1f + Turbulence1D(fixedT, turbulenceScale * 0.75f, turbulenceOctaves) * 0.25f);
        if (gustsEnabled && gustActive) forceMul *= Mathf.Max(0.001f, 1f + gustEnvelope.Evaluate(Mathf.Clamp01(gustPhase)) * gustIntensity);

        float force = Mathf.Clamp(baseForce * forceMul, minForce, maxForce);

        for (int i = 0; i < targets.Count; i++)
        {
            var rb = targets[i];
            if (!rb) continue;
            if (rb.isKinematic) continue;
            if (((1 << rb.gameObject.layer) & affectedLayers) == 0) continue;
            if (areaLimited && areaBounds) { if (!areaBounds.bounds.Contains(rb.worldCenterOfMass)) continue; }
            rb.AddForce(dir * force, forceMode);
        }
    }

    void Rescan()
    {
        targets.Clear();
        var rbs = GameObject.FindObjectsOfType<Rigidbody>();
        for (int i = 0; i < rbs.Length; i++)
        {
            var rb = rbs[i];
            if (!rb) continue;
            if (((1 << rb.gameObject.layer) & affectedLayers) == 0) continue;
            if (areaLimited && areaBounds) { if (!areaBounds.bounds.Contains(rb.worldCenterOfMass)) continue; }
            targets.Add(rb);
        }
    }

    Vector3 Turbulence(float t, float scale, int octaves)
    {
        float f = Mathf.Max(0.0001f, scale);
        float amp = 1f;
        float sumX = 0f, sumY = 0f, sumZ = 0f;
        float freq = f;
        for (int o = 0; o < Mathf.Max(1, octaves); o++)
        {
            float nx = Perlin3D(noiseOffsetA + new Vector3(t * freq, 0f, 0f));
            float ny = Perlin3D(noiseOffsetA + new Vector3(0f, t * freq, 0f));
            float nz = Perlin3D(noiseOffsetA + new Vector3(0f, 0f, t * freq));
            sumX += (nx * 2f - 1f) * amp;
            sumY += (ny * 2f - 1f) * amp;
            sumZ += (nz * 2f - 1f) * amp;
            freq *= 2f;
            amp *= 0.5f;
        }
        return new Vector3(sumX, sumY, sumZ);
    }

    float Turbulence1D(float t, float scale, int octaves)
    {
        float f = Mathf.Max(0.0001f, scale);
        float amp = 1f;
        float sum = 0f;
        float freq = f;
        for (int o = 0; o < Mathf.Max(1, octaves); o++)
        {
            float n = Perlin3D(noiseOffsetB + new Vector3(t * freq, t * 0.37f * freq, t * 0.73f * freq));
            sum += (n * 2f - 1f) * amp;
            freq *= 2f;
            amp *= 0.5f;
        }
        return sum;
    }

    float Perlin3D(Vector3 p)
    {
        float xy = Mathf.PerlinNoise(p.x, p.y);
        float yz = Mathf.PerlinNoise(p.y, p.z);
        float zx = Mathf.PerlinNoise(p.z, p.x);
        return (xy + yz + zx) / 3f;
    }

    public Vector3 GetCurrentWindDirection()
    {
        float yaw = dynamicWind ? yawOverDay.Evaluate(dayClock) : 0f;
        Vector3 dir = Quaternion.Euler(0f, yaw, 0f) * baseDirection.normalized;
        if (applyTurbulence) dir = (dir + Turbulence(fixedT, turbulenceScale, turbulenceOctaves)).normalized;
        return dir;
    }

    public float GetCurrentWindForce()
    {
        float forceMul = dynamicWind ? Mathf.Max(0f, forceOverDay.Evaluate(dayClock)) : 1f;
        if (applyTurbulence) forceMul *= Mathf.Clamp01(1f + Turbulence1D(fixedT, turbulenceScale * 0.75f, turbulenceOctaves) * 0.25f);
        if (gustsEnabled && gustActive) forceMul *= Mathf.Max(0.001f, 1f + gustEnvelope.Evaluate(Mathf.Clamp01(gustPhase)) * gustIntensity);
        return Mathf.Clamp(baseForce * forceMul, minForce, maxForce);
    }
}
