using System;

namespace OpenBve.Worlds
{
    public class Vectors
    {
        public static void Cross(double ax, double ay, double az, double bx, double by, double bz, out double cx, out double cy, out double cz)
        {
            cx = (ay * bz) - (az * by);
            cy = (az * bx) - (ax * bz);
            cz = (ax * by) - (ay * bx);
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
