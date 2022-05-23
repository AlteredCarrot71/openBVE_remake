using System;

namespace OpenBve.Worlds.Vector
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

        public static void Rotate(ref Vector2D Vector, double cosa, double sina)
        {
            double u = (Vector.X * cosa) - (Vector.Y * sina);
            double v = (Vector.X * sina) + (Vector.Y * cosa);
            Vector.X = u;
            Vector.Y = v;
        }

        public static void Normalize(ref Vector2D Vector)
        {
            double t = (Vector.X * Vector.X) + (Vector.Y * Vector.Y);
            if (t != 0.0)
            {
                t = 1.0 / Math.Sqrt(t);
                Vector.X *= t;
                Vector.Y *= t;
            }
        }
    }
}
