using System;

namespace OpenBve.Worlds.Vector
{
    /// <summary>Represents a 3D vector of System.Double coordinates.</summary>
    public struct Vector3D
    {
        internal double X;
        internal double Y;
        internal double Z;
        public Vector3D(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        /// <summary>Returns a normalized vector based on a 2D vector in the XZ plane and an additional Y-coordinate.</summary>
        /// <param name="Vector">The vector in the XZ-plane. The X and Y components in Vector represent the X- and Z-coordinates, respectively.</param>
        /// <param name="Y">The Y-coordinate.</param>
        public Vector3D(Vector.Vector2D Vector, double Y)
        {
            double t = 1.0 / Math.Sqrt((Vector.X * Vector.X) + (Vector.Y * Vector.Y) + (Y * Y));
            this.X = t * Vector.X;
            this.Y = t * Y;
            this.Z = t * Vector.Y;
        }
        /// <summary>Returns the sum of two vectors.</summary>
        public static Vector3D Add(Vector3D A, Vector3D B)
        {
            return new Vector3D(A.X + B.X, A.Y + B.Y, A.Z + B.Z);
        }
        /// <summary>Returns the difference of two vectors.</summary>
        public static Vector3D Subtract(Vector3D A, Vector3D B)
        {
            return new Vector3D(A.X - B.X, A.Y - B.Y, A.Z - B.Z);
        }

        public static Vector3D Cross(Vector3D A, Vector3D B)
        {
            Vector3D C;
            C.X = (A.Y * B.Z) - (A.Z * B.Y);
            C.Y = (A.Z * B.X) - (A.X * B.Z);
            C.Z = (A.X * B.Y) - (A.Y * B.X);
            return C;
        }

        public static void RotatePlane(ref Vector3D Vector, double cosa, double sina)
        {
            double u = (Vector.X * cosa) - (Vector.Z * sina);
            double v = (Vector.X * sina) + (Vector.Z * cosa);
            Vector.X = u;
            Vector.Z = v;
        }

        public static void RotateUpDown(ref Vector3D Vector, Vector.Vector2D Direction, double cosa, double sina)
        {
            double dx = Direction.X, dy = Direction.Y;
            double x = Vector.X, y = Vector.Y, z = Vector.Z;
            double u = (dy * x) - (dx * z);
            double v = (dx * x) + (dy * z);
            Vector.X = (dy * u) + (dx * v * cosa) - (dx * y * sina);
            Vector.Y = (y * cosa) + (v * sina);
            Vector.Z = (-dx * u) + (dy * v * cosa) - (dy * y * sina);
        }
        public static void RotateUpDown(ref Vector3D Vector, double dx, double dy, double cosa, double sina)
        {
            double x = Vector.X, y = Vector.Y, z = Vector.Z;
            double u = (dy * x) - (dx * z);
            double v = (dx * x) + (dy * z);
            Vector.X = (dy * u) + (dx * v * cosa) - (dx * y * sina);
            Vector.Y = (y * cosa) + (v * sina);
            Vector.Z = (-dx * u) + (dy * v * cosa) - (dy * y * sina);
        }
    }
}
