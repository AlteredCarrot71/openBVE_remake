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
            public Vector3D(Vector.Vector2D Vector, double Y)
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

        public static void Rotate(ref double px, ref double py, ref double pz, double dx, double dy, double dz, double cosa, double sina)
        {
            double t = 1.0 / Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
            dx *= t; 
            dy *= t; 
            dz *= t;
            double oc = 1.0 - cosa;
            double x = ((cosa + (oc * dx * dx)) * px) + (((oc * dx * dy) - (sina * dz)) * py) + (((oc * dx * dz) + (sina * dy)) * pz);
            double y = ((cosa + (oc * dy * dy)) * py) + (((oc * dx * dy) + (sina * dz)) * px) + (((oc * dy * dz) - (sina * dx)) * pz);
            double z = ((cosa + (oc * dz * dz)) * pz) + (((oc * dx * dz) - (sina * dy)) * px) + (((oc * dy * dz) + (sina * dx)) * py);
            px = x; 
            py = y; 
            pz = z;
        }
        public static void Rotate(ref float px, ref float py, ref float pz, double dx, double dy, double dz, double cosa, double sina)
        {
            double t = 1.0 / Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
            dx *= t; 
            dy *= t; 
            dz *= t;
            double oc = 1.0 - cosa;
            double x = ((cosa + (oc * dx * dx)) * (double)px) + (((oc * dx * dy) - (sina * dz)) * (double)py) + (((oc * dx * dz) + (sina * dy)) * (double)pz);
            double y = ((cosa + (oc * dy * dy)) * (double)py) + (((oc * dx * dy) + (sina * dz)) * (double)px) + (((oc * dy * dz) - (sina * dx)) * (double)pz);
            double z = ((cosa + (oc * dz * dz)) * (double)pz) + (((oc * dx * dz) - (sina * dy)) * (double)px) + (((oc * dy * dz) + (sina * dx)) * (double)py);
            px = (float)x; 
            py = (float)y; 
            pz = (float)z;
        }
        public static void Rotate(ref float px, ref float py, ref float pz, double dx, double dy, double dz, double ux, double uy, double uz, double sx, double sy, double sz)
        {
            double x, y, z;
            x = (sx * (double)px) + (ux * (double)py) + (dx * (double)pz);
            y = (sy * (double)px) + (uy * (double)py) + (dy * (double)pz);
            z = (sz * (double)px) + (uz * (double)py) + (dz * (double)pz);
            px = (float)x; 
            py = (float)y; 
            pz = (float)z;
        }
        public static void Rotate(ref double px, ref double py, ref double pz, double dx, double dy, double dz, double ux, double uy, double uz, double sx, double sy, double sz)
        {
            double x, y, z;
            x = (sx * px) + (ux * py) + (dx * pz);
            y = (sy * px) + (uy * py) + (dy * pz);
            z = (sz * px) + (uz * py) + (dz * pz);
            px = x; 
            py = y; 
            pz = z;
        }
        public static void Rotate(ref float px, ref float py, ref float pz, Transformation t)
        {
            double x, y, z;
            x = (t.X.X * (double)px) + (t.Y.X * (double)py) + (t.Z.X * (double)pz);
            y = (t.X.Y * (double)px) + (t.Y.Y * (double)py) + (t.Z.Y * (double)pz);
            z = (t.X.Z * (double)px) + (t.Y.Z * (double)py) + (t.Z.Z * (double)pz);
            px = (float)x; 
            py = (float)y; 
            pz = (float)z;
        }
        public static void Rotate(ref double px, ref double py, ref double pz, Transformation t)
        {
            double x, y, z;
            x = (t.X.X * px) + (t.Y.X * py) + (t.Z.X * pz);
            y = (t.X.Y * px) + (t.Y.Y * py) + (t.Z.Y * pz);
            z = (t.X.Z * px) + (t.Y.Z * py) + (t.Z.Z * pz);
            px = x; 
            py = y; 
            pz = z;
        }
        public static void RotatePlane(ref Vectors.Vector3D Vector, double cosa, double sina)
        {
            double u = (Vector.X * cosa) - (Vector.Z * sina);
            double v = (Vector.X * sina) + (Vector.Z * cosa);
            Vector.X = u;
            Vector.Z = v;
        }
        public static void RotatePlane(ref Vectors.Vector3Df Vector, double cosa, double sina)
        {
            double u = ((double)Vector.X * cosa) - ((double)Vector.Z * sina);
            double v = ((double)Vector.X * sina) + ((double)Vector.Z * cosa);
            Vector.X = (float)u;
            Vector.Z = (float)v;
        }
        public static void RotateUpDown(ref Vectors.Vector3D Vector, Vector.Vector2D Direction, double cosa, double sina)
        {
            double dx = Direction.X, dy = Direction.Y;
            double x = Vector.X, y = Vector.Y, z = Vector.Z;
            double u = (dy * x) - (dx * z);
            double v = (dx * x) + (dy * z);
            Vector.X = (dy * u) + (dx * v * cosa) - (dx * y * sina);
            Vector.Y = (y * cosa) + (v * sina);
            Vector.Z = (-dx * u) + (dy * v * cosa) - (dy * y * sina);
        }
        public static void RotateUpDown(ref Vectors.Vector3D Vector, double dx, double dy, double cosa, double sina)
        {
            double x = Vector.X, y = Vector.Y, z = Vector.Z;
            double u = (dy * x) - (dx * z);
            double v = (dx * x) + (dy * z);
            Vector.X = (dy * u) + (dx * v * cosa) - (dx * y * sina);
            Vector.Y = (y * cosa) + (v * sina);
            Vector.Z = (-dx * u) + (dy * v * cosa) - (dy * y * sina);
        }
        public static void RotateUpDown(ref Vectors.Vector3Df Vector, double dx, double dy, double cosa, double sina)
        {
            double x = (double)Vector.X, y = (double)Vector.Y, z = (double)Vector.Z;
            double u = (dy * x) - (dx * z);
            double v = (dx * x) + (dy * z);
            Vector.X = (float)((dy * u) + (dx * v * cosa) - (dx * y * sina));
            Vector.Y = (float)((y * cosa) + (v * sina));
            Vector.Z = (float)((-dx * u) + (dy * v * cosa) - (dy * y * sina));
        }
        public static void RotateUpDown(ref double px, ref double py, ref double pz, double dx, double dz, double cosa, double sina)
        {
            double x = px, y = py, z = pz;
            double u = (dz * x) - (dx * z);
            double v = (dx * x) + (dz * z);
            px = (dz * u) + (dx * v * cosa) - (dx * y * sina);
            py = (y * cosa) + (v * sina);
            pz = (-dx * u) + (dz * v * cosa) - (dz * y * sina);
        }

        public static void Normalize(ref double x, ref double y)
        {
            double t = (x * x) + (y * y);
            if (t != 0.0)
            {
                t = 1.0 / Math.Sqrt(t);
                x *= t;
                y *= t;
            }
        }
        public static void Normalize(ref double x, ref double y, ref double z)
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
