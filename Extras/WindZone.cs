using UnityEngine;

public class WindZone : MonoBehaviour
{
    private Collider windZoneCollider;

    private void Start()
    {
        windZoneCollider = GetComponent<Collider>();
        if (windZoneCollider == null || !windZoneCollider.isTrigger)
        {
            Debug.LogError("WindZone must have a trigger collider attached.");
        }
    }

    public bool IsRigidbodyInside(Rigidbody rb)
    {
        return windZoneCollider.bounds.Contains(rb.position);
    }
}
