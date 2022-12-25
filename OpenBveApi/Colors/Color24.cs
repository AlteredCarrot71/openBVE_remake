using System;

namespace OpenBveApi.Colors
{
    /// <summary>Represents a 24-bit color with red, green and blue channels at 8 bits each.</summary>
    public struct Color24
    {
        #region
        /// <summary>The red component.</summary>
        public byte R { get; set; } 
        /// <summary>The green component.</summary>
        public byte G { get; set; }
        /// <summary>The blue component.</summary>
        public byte B { get; set; }
        #endregion

        #region constructors
        /// <summary>Creates a new color.</summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public Color24(byte r, byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }
        #endregion

        #region operators
        /// <summary>Checks whether two colors are equal.</summary>
        /// <param name="a">The first color.</param>
        /// <param name="b">The second color.</param>
        /// <returns>Whether the two colors are equal.</returns>
        public static bool operator ==(Color24 a, Color24 b)
        {
            return a.R == b.R & a.G == b.G & a.B == b.B;
        }
        /// <summary>Checks whether two colors are unequal.</summary>
        /// <param name="a">The first color.</param>
        /// <param name="b">The second color.</param>
        /// <returns>Whether the two colors are unequal.</returns>
        public static bool operator !=(Color24 a, Color24 b)
        {
            return a.R != b.R | a.G != b.G | a.B != b.B;
        }
        #endregion

        #region read-only fields
        /// <summary>Represents a black color.</summary>
        public static readonly Color24 Black = new Color24(0, 0, 0);
        /// <summary>Represents a red color.</summary>
        public static readonly Color24 Red = new Color24(255, 0, 0);
        /// <summary>Represents a green color.</summary>
        public static readonly Color24 Green = new Color24(0, 255, 0);
        /// <summary>Represents a blue color.</summary>
        public static readonly Color24 Blue = new Color24(0, 0, 255);
        /// <summary>Represents a cyan color.</summary>
        public static readonly Color24 Cyan = new Color24(0, 255, 255);
        /// <summary>Represents a magenta color.</summary>
        public static readonly Color24 Magenta = new Color24(255, 0, 255);
        /// <summary>Represents a yellow color.</summary>
        public static readonly Color24 Yellow = new Color24(255, 255, 0);
        /// <summary>Represents a white color.</summary>
        public static readonly Color24 White = new Color24(255, 255, 255);
        #endregion

        #region common overrides
        /// <summary>Checks whether two colors are equal.</summary>
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns the hash code for this instance.</summary>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
