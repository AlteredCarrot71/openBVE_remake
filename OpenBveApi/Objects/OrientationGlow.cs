using OpenBveApi.Math;

namespace OpenBveApi.Objects
{
    /// <summary>Represents an abstract orientational glow. This is the base class from which all orientational glows must inherit.</summary>
    /// <remarks>This type of glow computes the intensity as a function of the camera and object's position and orientation.</remarks>
    public abstract class OrientationalGlow : AbstractGlow
    {
        /// <summary>Gets the intensity of the glow.</summary>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraOrientation">The orientation of the camera.</param>
        /// <param name="objectPosition">The position of the object.</param>
        /// <param name="objectOrientation">The orientation of the object.</param>
        /// <returns>The intensity of the glow expressed as a value between 0 and 1.</returns>
        public abstract double GetIntensity(Vector3 cameraPosition, Orientation3 cameraOrientation, Vector3 objectPosition, Vector3 objectOrientation);
    }
}
