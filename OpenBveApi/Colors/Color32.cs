using System;

namespace OpenBveApi.Colors
{
	/// <summary>Represents a 32-bit color with red, green, blue and alpha channels at 8 bits each.</summary>
	public struct Color32
	{
		// --- members ---
		/// <summary>The red component.</summary>
		public byte R;
		/// <summary>The green component.</summary>
		public byte G;
		/// <summary>The blue component.</summary>
		public byte B;
		/// <summary>The alpha component.</summary>
		public byte A;
		// --- constructors ---
		/// <summary>Creates a new color.</summary>
		/// <param name="r">The red component.</param>
		/// <param name="g">The green component.</param>
		/// <param name="b">The blue component.</param>
		/// <param name="a">The alpha component.</param>
		public Color32(byte r, byte g, byte b, byte a)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
		}
		/// <summary>Creates a new color.</summary>
		/// <param name="r">The red component.</param>
		/// <param name="g">The green component.</param>
		/// <param name="b">The blue component.</param>
		/// <remarks>The alpha component is set to full opacity.</remarks>
		public Color32(byte r, byte g, byte b)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = 255;
		}
		/// <summary>Creates a new color.</summary>
		/// <param name="color">The solid color.</param>
		/// <param name="a">The alpha component.</param>
		public Color32(Color24 color, byte a)
		{
			this.R = color.R;
			this.G = color.G;
			this.B = color.B;
			this.A = a;
		}
		/// <summary>Creates a new color.</summary>
		/// <param name="color">The solid color.</param>
		/// <remarks>The alpha component is set to full opacity.</remarks>
		public Color32(Color24 color)
		{
			this.R = color.R;
			this.G = color.G;
			this.B = color.B;
			this.A = 255;
		}
		// --- operators ---
		/// <summary>Checks whether two colors are equal.</summary>
		/// <param name="a">The first color.</param>
		/// <param name="b">The second color.</param>
		/// <returns>Whether the two colors are equal.</returns>
		public static bool operator ==(Color32 a, Color32 b)
		{
			return a.R == b.R & a.G == b.G & a.B == b.B & a.A == b.A;
		}
		/// <summary>Checks whether two colors are unequal.</summary>
		/// <param name="a">The first color.</param>
		/// <param name="b">The second color.</param>
		/// <returns>Whether the two colors are unequal.</returns>
		public static bool operator !=(Color32 a, Color32 b)
		{
			return a.R != b.R | a.G != b.G | a.B != b.B | a.A != b.A;
		}
		// --- read-only fields ---
		/// <summary>Represents a black color.</summary>
		public static readonly Color32 Black = new Color32(0, 0, 0);
		/// <summary>Represents a red color.</summary>
		public static readonly Color32 Red = new Color32(255, 0, 0);
		/// <summary>Represents a green color.</summary>
		public static readonly Color32 Green = new Color32(0, 255, 0);
		/// <summary>Represents a blue color.</summary>
		public static readonly Color32 Blue = new Color32(0, 0, 255);
		/// <summary>Represents a cyan color.</summary>
		public static readonly Color32 Cyan = new Color32(0, 255, 255);
		/// <summary>Represents a magenta color.</summary>
		public static readonly Color32 Magenta = new Color32(255, 0, 255);
		/// <summary>Represents a yellow color.</summary>
		public static readonly Color32 Yellow = new Color32(255, 255, 0);
		/// <summary>Represents a white color.</summary>
		public static readonly Color32 White = new Color32(255, 255, 255);
		/// <summary>Represents a transparent black color.</summary>
		public static readonly Color32 Transparent = new Color32(0, 0, 0, 0);

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
	}
}
