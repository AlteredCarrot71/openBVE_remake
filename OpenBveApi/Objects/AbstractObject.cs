using OpenBveApi.Math;

namespace OpenBveApi.Objects
{
	/// <summary>Represents an abstract object. This is the base class from which all objects must inherit.</summary>
	public abstract class AbstractObject
	{
		/// <summary>Translates the object by the specified offset.</summary>
		/// <param name="offset">The offset by which to translate.</param>
		public abstract void Translate(Vector3 offset);
		/// <summary>Translates the object by the specified offset that is measured in the specified orientation.</summary>
		/// <param name="orientation">The orientation along which to translate.</param>
		/// <param name="offset">The offset measured in the specified orientation.</param>
		public abstract void Translate(Orientation3 orientation, Vector3 offset);
		/// <summary>Rotates the object around the specified axis.</summary>
		/// <param name="direction">The axis along which to rotate.</param>
		/// <param name="cosineOfAngle">The cosine of the angle by which to rotate.</param>
		/// <param name="sineOfAngle">The sine of the angle by which to rotate.</param>
		public abstract void Rotate(Vector3 direction, double cosineOfAngle, double sineOfAngle);
		/// <summary>Rotates the object from the default orientation into the specified orientation.</summary>
		/// <param name="orientation">The target orientation.</param>
		/// <remarks>The default orientation is X = {1, 0, 0), Y = {0, 1, 0} and Z = {0, 0, 1}.</remarks>
		public abstract void Rotate(Orientation3 orientation);
		/// <summary>Scales the object by the specified factor.</summary>
		/// <param name="factor">The factor by which to scale.</param>
		public abstract void Scale(Vector3 factor);
	}
}