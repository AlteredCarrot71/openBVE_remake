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
            /// <summary>Returns the cross of two vectors.</summary>
            public static Vector3D Cross(Vector3D A, Vector3D B)
            {
                return new Vector3D((A.Y * B.Z) - (A.Z * B.Y), (A.Z * B.X) - (A.X * B.Z), (A.X * B.Y) - (A.Y * B.X));
            }

            public static Vector3D operator +(Vector3D a, Vector3D b)
            {
                return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
            }
            public static Vector3D operator -(Vector3D a, Vector3D b)
            {
                return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
            }
            public static Vector3D operator *(Vector3D a, double b)
            {
                return new Vector3D(a.X * b, a.Y * b, a.Z * b);
            }
            public static Vector3D operator *(double a, Vector3D b)
            {
                return new Vector3D(a * b.X, a * b.Y, a * b.Z);
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
            public void Rotate(Transformation t)
            {
                double x, y, z;
                x = (t.X.X * this.X) + (t.Y.X * this.Y) + (t.Z.X * this.Z);
                y = (t.X.Y * this.X) + (t.Y.Y * this.Y) + (t.Z.Y * this.Z);
                z = (t.X.Z * this.X) + (t.Y.Z * this.Y) + (t.Z.Z * this.Z);
                this.X = x; 
                this.Y = y; 
                this.Z = z;
            }

            public void Normalize()
            {
                double Magnitude = (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
                if (Magnitude != 0.0)
                {
                    Magnitude = 1.0 / Math.Sqrt(Magnitude);
                    this.X *= Magnitude;
                    this.Y *= Magnitude;
                    this.Z *= Magnitude;
                }
            }

            public static Vector3D Null = new Vector3D(0.0, 0.0, 0.0);

            public static Vector3D Right = new Vector3D(1.0, 0.0, 0.0);

            public static Vector3D Down = new Vector3D(0.0, 1.0, 0.0);

            public static Vector3D Forward = new Vector3D(0.0, 0.0, 1.0);
        }
        /// <summary>Represents a 3D vector of System.Float coordinates.</summary>
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
            public void Rotate(Transformation t)
            {
                double x, y, z;
                x = (t.X.X * (double)this.X) + (t.Y.X * (double)this.Y) + (t.Z.X * (double)this.Z);
                y = (t.X.Y * (double)this.X) + (t.Y.Y * (double)this.Y) + (t.Z.Y * (double)this.Z);
                z = (t.X.Z * (double)this.X) + (t.Y.Z * (double)this.Y) + (t.Z.Z * (double)this.Z);
                this.X = (float)x; 
                this.Y = (float)y; 
                this.Z = (float)z;
            }

            public void Normalize()
            {
                float t = (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
                if (t != 0.0)
                {
                    t = (float)((float)1.0 / Math.Sqrt(t));
                    this.X *= t;
                    this.Y *= t;
                    this.Z *= t;
                }
            }
        }
        
        public struct Transformation
        {
            public Vector3D X;
            public Vector3D Y;
            public Vector3D Z;

            public Transformation(double Yaw, double Pitch, double Roll)
            {
                if (Yaw == 0.0 & Pitch == 0.0 & Roll == 0.0)
                {
                    this.X = Vector3D.Right;
                    this.Y = Vector3D.Down;
                    this.Z = Vector3D.Forward;
                }
                else if (Pitch == 0.0 & Roll == 0.0)
                {
                    double cosYaw = Math.Cos(Yaw);
                    double sinYaw = Math.Sin(Yaw);
                    this.X = new Vector3D(cosYaw, 0.0, -sinYaw);
                    this.Y = new Vector3D(0.0, 1.0, 0.0);
                    this.Z = new Vector3D(sinYaw, 0.0, cosYaw);
                }
                else
                {
                    double cosYaw = Math.Cos(Yaw);
                    double sinYaw = Math.Sin(Yaw);
                    double cosPitch = Math.Cos(-Pitch);
                    double sinPitch = Math.Sin(-Pitch);
                    double cosRoll = Math.Cos(-Roll);
                    double sinRoll = Math.Sin(-Roll);
                    Vector3D s = Vector3D.Right;
                    Vector3D u = Vector3D.Down;
                    Vector3D d = Vector3D.Forward;
                    s.Rotate(u, cosYaw, sinYaw);
                    d.Rotate(u, cosYaw, sinYaw);
                    u.Rotate(s, cosPitch, sinPitch);
                    d.Rotate(s, cosPitch, sinPitch);
                    s.Rotate(d, cosRoll, sinRoll);
                    u.Rotate(d, cosRoll, sinRoll);
                    this.X = s;
                    this.Y = u;
                    this.Z = d;
                }
            }
            public Transformation(Transformation Transformation, double Yaw, double Pitch, double Roll)
            {
                double cosYaw = Math.Cos(Yaw);
                double sinYaw = Math.Sin(Yaw);
                double cosPitch = Math.Cos(-Pitch);
                double sinPitch = Math.Sin(-Pitch);
                double cosRoll = Math.Cos(Roll);
                double sinRoll = Math.Sin(Roll);
                Vector3D s = Transformation.X;
                Vector3D u = Transformation.Y;
                Vector3D d = Transformation.Z;
                s.Rotate(u, cosYaw, sinYaw);
                d.Rotate(u, cosYaw, sinYaw);
                u.Rotate(s, cosPitch, sinPitch);
                d.Rotate(s, cosPitch, sinPitch);
                s.Rotate(d, cosRoll, sinRoll);
                u.Rotate(d, cosRoll, sinRoll);
                this.X = s;
                this.Y = u;
                this.Z = d;
            }
            public Transformation(Transformation BaseTransformation, Transformation AuxTransformation)
            {
                Vector3D x = BaseTransformation.X;
                Vector3D y = BaseTransformation.Y;
                Vector3D z = BaseTransformation.Z;
                Vector3D s = AuxTransformation.X;
                Vector3D u = AuxTransformation.Y;
                Vector3D d = AuxTransformation.Z;
                x.Rotate(d, u, s);
                y.Rotate(d, u, s);
                z.Rotate(d, u, s);
                this.X = x;
                this.Y = y;
                this.Z = z;
            }
        }

        public static void Cross(double ax, double ay, double az, double bx, double by, double bz, out double cx, out double cy, out double cz)
        {
            cx = (ay * bz) - (az * by);
            cy = (az * bx) - (ax * bz);
            cz = (ax * by) - (ay * bx);
        }

        public static void Rotate(ref double px, ref double py, ref double pz, double dx, double dy, double dz, double cosa, double sina)
        {
            double t = 1.0 / Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
            dx *= t; dy *= t; dz *= t;
            double oc = 1.0 - cosa;
            double x = ((cosa + (oc * dx * dx)) * px) + (((oc * dx * dy) - (sina * dz)) * py) + (((oc * dx * dz) + (sina * dy)) * pz);
            double y = ((cosa + (oc * dy * dy)) * py) + (((oc * dx * dy) + (sina * dz)) * px) + (((oc * dy * dz) - (sina * dx)) * pz);
            double z = ((cosa + (oc * dz * dz)) * pz) + (((oc * dx * dz) - (sina * dy)) * px) + (((oc * dy * dz) + (sina * dx)) * py);
            px = x; py = y; pz = z;
        }

        internal static void Normalize(ref double x, ref double y, ref double z)
        {   
            double t = (x * x) + (y * y) + (z * z);
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
