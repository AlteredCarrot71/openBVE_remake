using System;

namespace OpenBve.Worlds
{
    public struct Transformation
    {
        internal Vectors.Vector3D X;
        internal Vectors.Vector3D Y;
        internal Vectors.Vector3D Z;
        public Transformation(double Yaw, double Pitch, double Roll)
        {
            if (Yaw == 0.0 & Pitch == 0.0 & Roll == 0.0)
            {
                this.X = new Vectors.Vector3D(1.0, 0.0, 0.0);
                this.Y = new Vectors.Vector3D(0.0, 1.0, 0.0);
                this.Z = new Vectors.Vector3D(0.0, 0.0, 1.0);
            }
            else if (Pitch == 0.0 & Roll == 0.0)
            {
                double cosYaw = Math.Cos(Yaw);
                double sinYaw = Math.Sin(Yaw);
                this.X = new Vectors.Vector3D(cosYaw, 0.0, -sinYaw);
                this.Y = new Vectors.Vector3D(0.0, 1.0, 0.0);
                this.Z = new Vectors.Vector3D(sinYaw, 0.0, cosYaw);
            }
            else
            {
                double sx = 1.0, sy = 0.0, sz = 0.0;
                double ux = 0.0, uy = 1.0, uz = 0.0;
                double dx = 0.0, dy = 0.0, dz = 1.0;
                double cosYaw = Math.Cos(Yaw);
                double sinYaw = Math.Sin(Yaw);
                double cosPitch = Math.Cos(-Pitch);
                double sinPitch = Math.Sin(-Pitch);
                double cosRoll = Math.Cos(-Roll);
                double sinRoll = Math.Sin(-Roll);
                Vectors.Rotate(ref sx, ref sy, ref sz, ux, uy, uz, cosYaw, sinYaw);
                Vectors.Rotate(ref dx, ref dy, ref dz, ux, uy, uz, cosYaw, sinYaw);
                Vectors.Rotate(ref ux, ref uy, ref uz, sx, sy, sz, cosPitch, sinPitch);
                Vectors.Rotate(ref dx, ref dy, ref dz, sx, sy, sz, cosPitch, sinPitch);
                Vectors.Rotate(ref sx, ref sy, ref sz, dx, dy, dz, cosRoll, sinRoll);
                Vectors.Rotate(ref ux, ref uy, ref uz, dx, dy, dz, cosRoll, sinRoll);
                this.X = new Vectors.Vector3D(sx, sy, sz);
                this.Y = new Vectors.Vector3D(ux, uy, uz);
                this.Z = new Vectors.Vector3D(dx, dy, dz);
            }
        }
        public Transformation(Transformation Transformation, double Yaw, double Pitch, double Roll)
        {
            double sx = Transformation.X.X, sy = Transformation.X.Y, sz = Transformation.X.Z;
            double ux = Transformation.Y.X, uy = Transformation.Y.Y, uz = Transformation.Y.Z;
            double dx = Transformation.Z.X, dy = Transformation.Z.Y, dz = Transformation.Z.Z;
            double cosYaw = Math.Cos(Yaw);
            double sinYaw = Math.Sin(Yaw);
            double cosPitch = Math.Cos(-Pitch);
            double sinPitch = Math.Sin(-Pitch);
            double cosRoll = Math.Cos(Roll);
            double sinRoll = Math.Sin(Roll);
            Vectors.Rotate(ref sx, ref sy, ref sz, ux, uy, uz, cosYaw, sinYaw);
            Vectors.Rotate(ref dx, ref dy, ref dz, ux, uy, uz, cosYaw, sinYaw);
            Vectors.Rotate(ref ux, ref uy, ref uz, sx, sy, sz, cosPitch, sinPitch);
            Vectors.Rotate(ref dx, ref dy, ref dz, sx, sy, sz, cosPitch, sinPitch);
            Vectors.Rotate(ref sx, ref sy, ref sz, dx, dy, dz, cosRoll, sinRoll);
            Vectors.Rotate(ref ux, ref uy, ref uz, dx, dy, dz, cosRoll, sinRoll);
            this.X = new Vectors.Vector3D(sx, sy, sz);
            this.Y = new Vectors.Vector3D(ux, uy, uz);
            this.Z = new Vectors.Vector3D(dx, dy, dz);
        }
        public Transformation(Transformation BaseTransformation, Transformation AuxTransformation)
        {
            Vectors.Vector3D x = BaseTransformation.X;
            Vectors.Vector3D y = BaseTransformation.Y;
            Vectors.Vector3D z = BaseTransformation.Z;
            Vectors.Vector3D s = AuxTransformation.X;
            Vectors.Vector3D u = AuxTransformation.Y;
            Vectors.Vector3D d = AuxTransformation.Z;
            Vectors.Rotate(ref x.X, ref x.Y, ref x.Z, d.X, d.Y, d.Z, u.X, u.Y, u.Z, s.X, s.Y, s.Z);
            Vectors.Rotate(ref y.X, ref y.Y, ref y.Z, d.X, d.Y, d.Z, u.X, u.Y, u.Z, s.X, s.Y, s.Z);
            Vectors.Rotate(ref z.X, ref z.Y, ref z.Z, d.X, d.Y, d.Z, u.X, u.Y, u.Z, s.X, s.Y, s.Z);
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
