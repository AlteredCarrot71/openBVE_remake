using System;

namespace OpenBve.Worlds
{
    public class Vectors
    {
        /// <summary>Represents a 2D vector of System.Double coordinates.</summary>
        public struct Vector2D
        {
            public double X { get; set; }
            public double Y { get; set; }
            
            public Vector2D(double X, double Y)
            {
                this.X = X;
                this.Y = Y;
            }

            public void Rotate(double cosa, double sina)
            {
                this.X = (this.X * cosa) - (this.Y * sina);
                this.Y = (this.X * sina) + (this.Y * cosa);
            }

            public void Normalize()
            {
                double t = (this.X * this.X) + (this.Y * this.Y);
                if (t != 0.0)
                {
                    t = 1.0 / Math.Sqrt(t);
                    this.X *= t;
                    this.Y *= t;
                }
            }
        }
        /// <summary>Represents a 2D vector of System.Single coordinates.</summary>
        public struct Vector2Df
        {
            internal float X;
            internal float Y;
            public Vector2Df(float X, float Y)
            {
                this.X = X;
                this.Y = Y;
            }
        }
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
            public Vector3D(Vectors.Vector2D Vector, double Y)
            {
                double t = 1.0 / Math.Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y + Y * Y);
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
            cx = ay * bz - az * by;
            cy = az * bx - ax * bz;
            cz = ax * by - ay * bx;
        }
        public static Vector3D Cross(Vector3D A, Vector3D B)
        {
            Vector3D C; Cross(A.X, A.Y, A.Z, B.X, B.Y, B.Z, out C.X, out C.Y, out C.Z);
            return C;
        }
    }
}
