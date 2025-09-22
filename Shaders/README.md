# WindInteractionAdvanced Shader for Unity

This shader simulates dynamic wind-driven deformation for vegetation, cloth, and other soft materials. It responds to wind direction, turbulence, gusts, and object height, producing realistic bending and swaying effects entirely on the GPU.

Designed to be used with external wind systems such as `WindManager.cs` and `WindZone.cs`.

---

## Getting Started

1. Save the shader as `WindInteractionAdvanced.shader` in your `Assets/Shaders/` folder.
2. Create a new **Material** using this shader (`Shader > Custom > WindInteractionAdvanced`).
3. Assign the material to any mesh object you want to be affected by wind (e.g. grass, trees, flags).
4. Adjust the material's properties in the Inspector to fine-tune visual behavior.

---

## Shader Properties

### Main Settings

- **_MainTex**: The base texture applied to the mesh.
- **_Color**: Tint color (multiplied with `_MainTex`).

### Wind Simulation

- **_WindDirection**: The main wind direction as a world-space vector (e.g. (1,0,0) for wind from the left).
- **_WindIntensity**: Multiplier for the base wind strength.
- **_BendAmount**: How much the mesh bends in response to wind.
- **_BendAxisMask**: Which axes are allowed to bend (e.g. (0,1,0) for Y-only bend).

### Turbulence

- **_TurbulenceScale**: Frequency of noise-based variation (higher = smaller/faster noise).
- **_TurbulenceStrength**: Intensity of turbulence applied on top of wind.

### Gusts

- **_GustStrength**: How strong gusts become at peak.
- **_GustSpeed**: How fast gusts move through the world.

### Other

- **_HeightInfluence**: How much an object's height affects wind bending (1 = full top bend, 0 = uniform).
- **_Cutoff**: Optional alpha cutoff for transparency (used to discard pixels if alpha < threshold).

---

## Notes

- This shader operates entirely in the vertex stage and does not require CPU-side bone animation or vertex offset logic.
- Works best with dense geometry (e.g., foliage cards or cloth meshes).
- Use consistent mesh pivot orientation and world alignment for predictable results.
- For optimal performance, limit use of per-pixel transparency when using alpha blending.

---

## Recommended Pairings

- **WindManager.cs**: To control global wind direction, strength, turbulence, and gusts.
- **WindZone.cs**: To define local wind zones with falloff and object filtering.

These scripts can set shader global values using:

```csharp
Shader.SetGlobalVector("_WindDirection", currentWindDirection);
Shader.SetGlobalFloat("_WindIntensity", currentWindForce);
```
