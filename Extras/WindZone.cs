using UnityEngine;
using System.Collections.Generic;

public class WindZone : MonoBehaviour
{
    public float strength = 1f;
    public bool useFalloff = true;
    public AnimationCurve edgeFalloff = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    public LayerMask affectedLayers = ~0;
    public string requiredTag = "";
    public bool requireNonKinematic = true;
    public bool trackTriggers = true;
    public bool drawGizmos = true;

    private Collider[] colliders;
    private readonly HashSet<Rigidbody> inside = new HashSet<Rigidbody>();

    public event System.Action<Rigidbody> Entered;
    public event System.Action<Rigidbody> Exited;

    void Awake()
    {
        colliders = GetComponents<Collider>();
    }

    void OnEnable()
    {
        if (trackTriggers) inside.Clear();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!trackTriggers) return;
        var rb = other.attachedRigidbody;
        if (!Valid(rb)) return;
        if (IsRigidbodyInside(rb))
        {
            if (inside.Add(rb)) Entered?.Invoke(rb);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!trackTriggers) return;
        var rb = other.attachedRigidbody;
        if (!rb) return;
        if (inside.Remove(rb)) Exited?.Invoke(rb);
    }

    bool Valid(Rigidbody rb)
    {
        if (!rb) return false;
        if (requireNonKinematic && rb.isKinematic) return false;
        if (((1 << rb.gameObject.layer) & affectedLayers) == 0) return false;
        if (!string.IsNullOrEmpty(requiredTag) && !rb.CompareTag(requiredTag)) return false;
        return true;
    }

    public bool IsRigidbodyInside(Rigidbody rb)
    {
        if (!rb) return false;
        Vector3 p = rb.worldCenterOfMass;
        for (int i = 0; i < colliders.Length; i++)
        {
            var c = colliders[i];
            if (!c) continue;
            Vector3 cp = c.ClosestPoint(p);
            if ((cp - p).sqrMagnitude < 1e-8f) return true;
        }
        return false;
    }

    public float EvaluateMultiplier(Vector3 position)
    {
        if (!useFalloff) return strength;
        float m = 0f;
        for (int i = 0; i < colliders.Length; i++)
        {
            var c = colliders[i];
            if (!c) continue;
            Bounds b = c.bounds;
            float maxExtent = b.extents.magnitude + 1e-6f;
            float d = Vector3.Distance(position, c.ClosestPoint(position));
            float t = Mathf.Clamp01(d / maxExtent);
            float v = edgeFalloff.Evaluate(1f - t) * strength;
            if (v > m) m = v;
        }
        return Mathf.Max(m, 0f);
    }

    public IEnumerable<Rigidbody> GetInside()
    {
        if (trackTriggers) return inside;
        inside.Clear();
        var rbs = FindObjectsOfType<Rigidbody>();
        for (int i = 0; i < rbs.Length; i++)
        {
            var rb = rbs[i];
            if (!Valid(rb)) continue;
            if (IsRigidbodyInside(rb)) inside.Add(rb);
        }
        return inside;
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;
        var cols = GetComponents<Collider>();
        for (int i = 0; i < cols.Length; i++)
        {
            var c = cols[i];
            if (!c) continue;
            var b = c.bounds;
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.15f);
            Gizmos.DrawCube(b.center, b.size);
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.85f);
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
}
