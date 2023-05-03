# Wind Manager for Unity

The Wind Manager is a script for Unity that controls the wind direction and force and applies them to the rigidbodies in the scene. It also includes a turbulence effect that can be adjusted.

## Getting Started

To use the Wind Manager in your Unity project, follow these steps:

1. Download the WindManager.cs file.
2. Import the script into your Unity project by placing it in any folder in the Assets directory.
3. Attach the Wind Manager script to a game object in your scene.
4. Adjust the public variables in the Inspector to customize the wind behavior.

### Public Variables

The Wind Manager script includes the following public variables that can be adjusted in the Inspector:

- `windForce`: a float value that sets the initial wind force that will be applied to the rigidbodies.
- `dayLengthInSeconds`: a float value that sets the duration of a day in seconds.
- `turbulenceIntensity`: a float value that sets the intensity of the turbulence effect on the wind direction.
- `turbulenceScale`: a float value that sets the scale of the turbulence effect on the wind direction.
- `windDirection`: a Vector3 value that sets the initial wind direction. Note that this value is overridden by the turbulence effect.
- `dynamicRigidbodies`: a bool that determines whether the script will cache the rigidbodies in the scene or find them dynamically every frame.
- `dynamicWind`: a bool that determines whether the wind direction and force will be updated dynamically based on the current time.

### Example Usage

To apply the wind force to a specific rigidbody, add a Rigidbody component to the game object and ensure that "Use Gravity" is disabled. Then, place the game object in the scene and run the game. The Wind Manager script will automatically apply the wind force to the rigidbody every frame.

### Additional Notes

- The Wind Manager script only applies the wind force to non-kinematic rigidbodies.
- If `dynamicRigidbodies` is set to true, the script will find all rigidbodies in the scene every frame, which can be resource-intensive for large scenes.
- If `dynamicWind` is set to true, the wind direction and force will change dynamically based on the current time, which can create a more realistic wind behavior.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
