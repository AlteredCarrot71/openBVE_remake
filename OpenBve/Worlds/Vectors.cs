using OpenBveApi.Math;
using System;

namespace OpenBve.Worlds
{
    public class Vectors
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
            public Vector3D(Vector2 Vector, double Y)
            {
                double t = 1.0 / Math.Sqrt((Vector.X * Vector.X) + (Vector.Y * Vector.Y) + (Y * Y));
                this.X = t * Vector.X;
                this.Y = t * Y;
                this.Z = t * Vector.Y;
            }
            /// <summary>Returns the sum of two vectors.</summary>
            public static Vector3D Add(Vectors.Vector3D A, Vectors.Vector3D B)
            {
                return new Vector3D(A.X + B.X, A.Y + B.Y, A.Z + B.Z);
            }
            /// <summary>Returns the difference of two vectors.</summary>
            public static Vector3D Subtract(Vectors.Vector3D A, Vectors.Vector3D B)
            {
                return new Vector3D(A.X - B.X, A.Y - B.Y, A.Z - B.Z);
            }

            public void RotatePlane(double cosa, double sina)
            {
                this.X = this.X * cosa - this.Z * sina;
                this.Z = this.X * sina + this.Z * cosa;
            }

            public void Rotate(Vector3D Direction, Vector3D Up, Vector3D Side)
            {
                this.X = (Side.X * this.X) + (Up.X * this.Y) + (Direction.X * this.Z);
                this.Y = (Side.Y * this.X) + (Up.Y * this.Y) + (Direction.Y * this.Z);
                this.Z = (Side.Z * this.X) + (Up.Z * this.Y) + (Direction.Z * this.Z);
            }

        }
        /// <summary>Represents a 3D vector of System.Single coordinates.</summary>
        public struct Vector3Df
        {
            internal float X;
            internal float Y;
            internal float Z;
            public Vector3Df(float X, float Y, float Z)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
            }
            public bool IsZero()
            {
                if (this.X != 0.0f) return false;
                if (this.Y != 0.0f) return false;
                if (this.Z != 0.0f) return false;
                return true;
            }
        }

        public static void Cross(double ax, double ay, double az, double bx, double by, double bz, out double cx, out double cy, out double cz)
        {
            cx = (ay * bz) - (az * by);
            cy = (az * bx) - (ax * bz);
            cz = (ax * by) - (ay * bx);
        }
        public static Vector3D Cross(Vector3D A, Vector3D B)
        {
            Vector3D C;
            C.X = (A.Y * B.Z) - (A.Z * B.Y);
            C.Y = (A.Z * B.X) - (A.X * B.Z);
            C.Z = (A.X * B.Y) - (A.Y * B.X);
            return C;
        }

        internal static void Rotate(ref double px, ref double py, ref double pz, double dx, double dy, double dz, double cosa, double sina)
        {
            double t = 1.0 / Math.Sqrt(dx * dx + dy * dy + dz * dz);
            dx *= t; dy *= t; dz *= t;
            double oc = 1.0 - cosa;
            double x = (cosa + oc * dx * dx) * px + (oc * dx * dy - sina * dz) * py + (oc * dx * dz + sina * dy) * pz;
            double y = (cosa + oc * dy * dy) * py + (oc * dx * dy + sina * dz) * px + (oc * dy * dz - sina * dx) * pz;
            double z = (cosa + oc * dz * dz) * pz + (oc * dx * dz - sina * dy) * px + (oc * dy * dz + sina * dx) * py;
            px = x; py = y; pz = z;
        }
        internal static void Rotate(ref double px, ref double py, ref double pz, double dx, double dy, double dz, double ux, double uy, double uz, double sx, double sy, double sz)
        {
            double x, y, z;
            x = sx * px + ux * py + dx * pz;
            y = sy * px + uy * py + dy * pz;
            z = sz * px + uz * py + dz * pz;
            px = x; py = y; pz = z;
        }
    }
}
