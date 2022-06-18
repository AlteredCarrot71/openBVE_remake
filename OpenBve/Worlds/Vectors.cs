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
            public static Vector3D Add(Vector3D A, Vector3D B)
            {
                return new Vector3D(A.X + B.X, A.Y + B.Y, A.Z + B.Z);
            }
            /// <summary>Returns the difference of two vectors.</summary>
            public static Vector3D Subtract(Vector3D A, Vector3D B)
            {
                return new Vector3D(A.X - B.X, A.Y - B.Y, A.Z - B.Z);
            }

            public void RotatePlane(double cosa, double sina)
            {
                double x = (this.X * cosa) - (this.Z * sina);
                double z = (this.X * sina) + (this.Z * cosa);
                this.X = x;
                this.Z = z;
            }

            public void Rotate(Vector3D Direction, double cosa, double sina)
            {
                double t = 1.0 / Math.Sqrt((Direction.X * Direction.X) + (Direction.Y * Direction.Y) + (Direction.Z * Direction.Z));
                double dx = Direction.X * t;
                double dy = Direction.Y * t;
                double dz = Direction.Z * t;
                double oc = 1.0 - cosa;
                double x = ((cosa + (oc * dx * dx)) * this.X) + (((oc * dx * dy) - (sina * dz)) * this.Y) + (((oc * dx * dz) + (sina * dy)) * this.Z);
                double y = ((cosa + (oc * dy * dy)) * this.Y) + (((oc * dx * dy) + (sina * dz)) * this.X) + (((oc * dy * dz) - (sina * dx)) * this.Z);
                double z = ((cosa + (oc * dz * dz)) * this.Z) + (((oc * dx * dz) - (sina * dy)) * this.X) + (((oc * dy * dz) + (sina * dx)) * this.Y);
                this.X = x;
                this.Y = y;
                this.Z = z;
            }
            public void Rotate(Vector3D Direction, Vector3D Up, Vector3D Side)
            {
                double x = (Side.X * this.X) + (Up.X * this.Y) + (Direction.X * this.Z);
                double y = (Side.Y * this.X) + (Up.Y * this.Y) + (Direction.Y * this.Z);
                double z = (Side.Z * this.X) + (Up.Z * this.Y) + (Direction.Z * this.Z);
                this.X = x;
                this.Y = y;
                this.Z = z;
            }

            public void Normalize()
            {
                double t = (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
                if (t != 0.0)
                {
                    t = 1.0 / Math.Sqrt(t);
                    this.X *= t;
                    this.Y *= t;
                    this.Z *= t;
                }
            }
            
            public static Vector3D Down = new Vector3D(0.0, 1.0, 0.0);
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

            public void Rotate(Vector3D Direction, double cosa, double sina)
            {
                double t = 1.0 / Math.Sqrt((Direction.X * Direction.X) + (Direction.Y * Direction.Y) + (Direction.Z * Direction.Z));
                double dx = Direction.X * t;
                double dy = Direction.Y * t;
                double dz = Direction.Z * t;
                double oc = 1.0 - cosa;
                double x = ((cosa + (oc * dx * dx)) * (double)this.X) + (((oc * dx * dy) - (sina * dz)) * (double)this.Y) + (((oc * dx * dz) + (sina * dy)) * (double)this.Z);
                double y = ((cosa + (oc * dy * dy)) * (double)this.Y) + (((oc * dx * dy) + (sina * dz)) * (double)this.X) + (((oc * dy * dz) - (sina * dx)) * (double)this.Z);
                double z = ((cosa + (oc * dz * dz)) * (double)this.Z) + (((oc * dx * dz) - (sina * dy)) * (double)this.X) + (((oc * dy * dz) + (sina * dx)) * (double)this.Y);
                this.X = (float)x;
                this.Y = (float)y;
                this.Z = (float)z;
            }
            public void Rotate(Vector3D Direction, Vector3D Up, Vector3D Side)
            {
                double x, y, z;
                x = (Side.X * (double)this.X) + (Up.X * (double)this.Y) + (Direction.X * (double)this.Z);
                y = (Side.Y * (double)this.X) + (Up.Y * (double)this.Y) + (Direction.Y * (double)this.Z);
                z = (Side.Z * (double)this.X) + (Up.Z * (double)this.Y) + (Direction.Z * (double)this.Z);
                this.X = (float)x; 
                this.Y = (float)y; 
                this.Z = (float)z;
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
            double t = 1.0 / Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
            dx *= t; dy *= t; dz *= t;
            double oc = 1.0 - cosa;
            double x = ((cosa + (oc * dx * dx)) * px) + (((oc * dx * dy) - (sina * dz)) * py) + (((oc * dx * dz) + (sina * dy)) * pz);
            double y = ((cosa + (oc * dy * dy)) * py) + (((oc * dx * dy) + (sina * dz)) * px) + (((oc * dy * dz) - (sina * dx)) * pz);
            double z = ((cosa + (oc * dz * dz)) * pz) + (((oc * dx * dz) - (sina * dy)) * px) + (((oc * dy * dz) + (sina * dx)) * py);
            px = x; py = y; pz = z;
        }

        internal static void Rotate(ref float px, ref float py, ref float pz, double dx, double dy, double dz, double cosa, double sina)
        {
            double t = 1.0 / Math.Sqrt(dx * dx + dy * dy + dz * dz);
            dx *= t; dy *= t; dz *= t;
            double oc = 1.0 - cosa;
            double x = (cosa + oc * dx * dx) * (double)px + (oc * dx * dy - sina * dz) * (double)py + (oc * dx * dz + sina * dy) * (double)pz;
            double y = (cosa + oc * dy * dy) * (double)py + (oc * dx * dy + sina * dz) * (double)px + (oc * dy * dz - sina * dx) * (double)pz;
            double z = (cosa + oc * dz * dz) * (double)pz + (oc * dx * dz - sina * dy) * (double)px + (oc * dy * dz + sina * dx) * (double)py;
            px = (float)x; py = (float)y; pz = (float)z;
        }

        internal static void Normalize(ref double x, ref double y, ref double z)
        {
            double t = x * x + y * y + z * z;
            if (t != 0.0)
            {
                t = 1.0 / Math.Sqrt(t);
                x *= t;
                y *= t;
                z *= t;
            }
        }
    }
}
