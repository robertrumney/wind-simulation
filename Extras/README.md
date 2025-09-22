# WindZone for Unity

The `WindZone` script defines a custom wind region in your Unity scene. It provides falloff-based strength control, filtering options (layers, tags, kinematic state), and runtime queries to determine which rigidbodies are inside the zone.

This script is intended to work with external wind systems (such as `WindManager.cs`) by providing localized wind zones with intensity falloff and filtering.

## Getting Started

To use the WindZone in your Unity project:

1. Download the `WindZone.cs` file.
2. Place the script in your Unity project's `Assets` folder.
3. Add the `WindZone` component to any GameObject with one or more Collider components.
4. Ensure that at least one Collider is marked as `isTrigger = true`.
5. Adjust the public fields in the Inspector to configure behavior.

## Public Variables

- `strength`: Base multiplier for wind force inside the zone.
- `useFalloff`: Enables falloff curve from center to edge.
- `edgeFalloff`: An `AnimationCurve` that controls how wind strength drops off near the edge. Evaluated from 0 (center) to 1 (edge).
- `affectedLayers`: Only objects on these layers will be considered valid.
- `requiredTag`: (Optional) Only affect rigidbodies with this tag.
- `requireNonKinematic`: If true, kinematic rigidbodies are excluded.
- `trackTriggers`: Enables OnTriggerEnter/Exit detection and internal tracking of contained rigidbodies.
- `drawGizmos`: If true, draws a wireframe box and filled gizmo of the zone bounds in the scene view.

## Runtime Methods

### `bool IsRigidbodyInside(Rigidbody rb)`
Returns true if the specified Rigidbody is currently inside the zone.

### `float EvaluateMultiplier(Vector3 position)`
Returns the wind strength multiplier at a given world-space position, factoring in falloff and the zone’s shape.

### `IEnumerable<Rigidbody> GetInside()`
Returns all rigidbodies currently inside the zone. Uses internal cache if `trackTriggers` is enabled, otherwise checks all rigidbodies in the scene.

## Events

### `Action<Rigidbody> Entered`
Fired when a valid rigidbody enters the zone (only if `trackTriggers` is true).

### `Action<Rigidbody> Exited`
Fired when a valid rigidbody exits the zone (only if `trackTriggers` is true).

## Notes

- The `WindZone` does **not** apply wind forces directly. It is designed to work alongside external systems (such as a WindManager).
- You can assign multiple colliders to a single zone object. All will be evaluated for overlap and falloff calculations.
- This system works best with box or capsule colliders. Mesh colliders are supported but may be more performance-intensive.

## Example Usage

```csharp
if (windZone.IsRigidbodyInside(rb))
{
    float forceMultiplier = windZone.EvaluateMultiplier(rb.position);
    rb.AddForce(windManager.GetCurrentWindDirection() * windManager.GetCurrentWindForce() * forceMultiplier);
}
```
