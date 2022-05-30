namespace OpenBve.Worlds.Vector
{
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
}
