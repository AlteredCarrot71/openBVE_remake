namespace OpenBve.Worlds.Vector
{
    /// <summary>Represents a 2D vector of System.Single coordinates.</summary>
    public struct Vector2Df
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2Df(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
