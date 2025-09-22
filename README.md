# Wind Manager for Unity

The Wind Manager is a script for Unity that controls the wind direction and force and applies them to the rigidbodies in the scene. It also includes turbulence, gusts, and dynamic wind cycles that can be adjusted.

## Getting Started

To use the Wind Manager in your Unity project, follow these steps:

1. Download the `WindManager.cs` file.
2. Import the script into your Unity project by placing it in any folder in the `Assets` directory.
3. Attach the Wind Manager script to a game object in your scene.
4. Adjust the public variables in the Inspector to customize the wind behavior.

### Public Variables

The Wind Manager script includes the following public variables that can be adjusted in the Inspector:

#### Wind Settings

- `baseDirection`: a `Vector3` that sets the default wind direction.
- `baseForce`: a `float` that sets the default wind force before variation.
- `minForce`: minimum wind force allowed.
- `maxForce`: maximum wind force allowed.
- `forceMode`: the `ForceMode` used when applying wind to rigidbodies.

#### Day Cycle (if `dynamicWind` is enabled)

- `dynamicWind`: enables animated wind direction and strength based on time of day.
- `dayLengthInSeconds`: sets how long a full wind cycle lasts.
- `forceOverDay`: an `AnimationCurve` controlling how wind force varies over the day.
- `yawOverDay`: an `AnimationCurve` controlling how wind yaw (rotation) changes over the day.

#### Turbulence

- `applyTurbulence`: enables 3D noise-based turbulence.
- `turbulenceIntensity`: multiplier for turbulence strength.
- `turbulenceScale`: frequency scale of the turbulence.
- `turbulenceOctaves`: number of noise layers used for turbulence detail.

#### Gusts

- `gustsEnabled`: enables short bursts of stronger wind.
- `gustIntensity`: how strong gusts are relative to base force.
- `gustFrequency`: how often gusts may occur.
- `gustDuration`: duration of each gust event.
- `gustEnvelope`: `AnimationCurve` defining the gust strength shape over time.

#### Rigidbody Targeting

- `dynamicRigidbodies`: whether to re-scan the scene for rigidbodies at runtime.
- `rescanInterval`: how often to refresh the list of targets (if dynamicRigidbodies is true).
- `affectedLayers`: a layer mask defining which rigidbodies are affected.
- `areaLimited`: if true, wind only applies inside a specified region.
- `areaBounds`: a `BoxCollider` used to define the wind-affected area.

#### Misc

- `noiseSeed`: optional seed for deterministic turbulence and gust behavior.

### Example Usage

To apply the wind force to a specific rigidbody:

1. Add a `Rigidbody` component to the GameObject.
2. Ensure the Rigidbody is **non-kinematic**.
3. Place the GameObject in the scene.
4. Run the game. The Wind Manager will apply wind forces automatically.

### Additional Notes

- The Wind Manager only affects rigidbodies that are not marked as kinematic.
- If `dynamicRigidbodies` is set to true, the script will re-scan all rigidbodies in the scene every few frames. This may impact performance in large scenes.
- The wind direction, turbulence, and force are recalculated and applied in `FixedUpdate`, making the behavior frame-rate independent.
- The `GetCurrentWindDirection()` and `GetCurrentWindForce()` methods can be called at runtime if other scripts need to react to the wind state.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
