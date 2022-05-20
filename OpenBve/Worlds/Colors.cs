namespace OpenBve.Worlds
{
    public class Colors
    {
        /// <summary>Represents an RGB color with 8-bit precision per channel.</summary>
        public struct ColorRGB
        {
            internal byte R;
            internal byte G;
            internal byte B;
            public ColorRGB(byte R, byte G, byte B)
            {
                this.R = R;
                this.G = G;
                this.B = B;
            }
        }
        /// <summary>Represents an RGBA color with 8-bit precision per channel.</summary>
        public struct ColorRGBA
        {
            internal byte R;
            internal byte G;
            internal byte B;
            internal byte A;
            public ColorRGBA(byte R, byte G, byte B, byte A)
            {
                this.R = R;
                this.G = G;
                this.B = B;
                this.A = A;
            }
        }
    }
}
