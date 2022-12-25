namespace OpenBveApi.Math
{
    public struct Transformation
    {
        public Vector3 X;
        public Vector3 Y;
        public Vector3 Z;

        public Transformation(double Yaw, double Pitch, double Roll)
        {
            if (Yaw == 0.0 & Pitch == 0.0 & Roll == 0.0)
            {
                this.X = Vector3.Right;
                this.Y = Vector3.Down;
                this.Z = Vector3.Forward;
            }
            else if (Pitch == 0.0 & Roll == 0.0)
            {
                double cosYaw = System.Math.Cos(Yaw);
                double sinYaw = System.Math.Sin(Yaw);
                this.X = new Vector3(cosYaw, 0.0, -sinYaw);
                this.Y = new Vector3(0.0, 1.0, 0.0);
                this.Z = new Vector3(sinYaw, 0.0, cosYaw);
            }
            else
            {
                double cosYaw = System.Math.Cos(Yaw);
                double sinYaw = System.Math.Sin(Yaw);
                double cosPitch = System.Math.Cos(-Pitch);
                double sinPitch = System.Math.Sin(-Pitch);
                double cosRoll = System.Math.Cos(-Roll);
                double sinRoll = System.Math.Sin(-Roll);
                Vector3 s = Vector3.Right;
                Vector3 u = Vector3.Down;
                Vector3 d = Vector3.Forward;
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
            double cosYaw = System.Math.Cos(Yaw);
            double sinYaw = System.Math.Sin(Yaw);
            double cosPitch = System.Math.Cos(-Pitch);
            double sinPitch = System.Math.Sin(-Pitch);
            double cosRoll = System.Math.Cos(Roll);
            double sinRoll = System.Math.Sin(Roll);
            Vector3 s = Transformation.X;
            Vector3 u = Transformation.Y;
            Vector3 d = Transformation.Z;
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
            Vector3 x = BaseTransformation.X;
            Vector3 y = BaseTransformation.Y;
            Vector3 z = BaseTransformation.Z;
            Orientation3 Orientation = new Orientation3(AuxTransformation.Z, AuxTransformation.Y, AuxTransformation.X);
            x.Rotate(Orientation);
            y.Rotate(Orientation);
            z.Rotate(Orientation);
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
