﻿using System;

namespace OpenBveApi.Colors 
{
	// --- color 24 ---
	/// <summary>Represents a 24-bit color with red, green and blue channels at 8 bits each.</summary>
	public struct Color24 
	{
		// --- members ---
		/// <summary>The red component.</summary>
		public byte R;
		/// <summary>The green component.</summary>
		public byte G;
		/// <summary>The blue component.</summary>
		public byte B;
		// --- constructors ---
		/// <summary>Creates a new color.</summary>
		/// <param name="r">The red component.</param>
		/// <param name="g">The green component.</param>
		/// <param name="b">The blue component.</param>
		public Color24(byte r, byte g, byte b) {
			this.R = r;
			this.G = g;
			this.B = b;
		}
		// --- operators ---
		/// <summary>Checks whether two colors are equal.</summary>
		/// <param name="a">The first color.</param>
		/// <param name="b">The second color.</param>
		/// <returns>Whether the two colors are equal.</returns>
		public static bool operator ==(Color24 a, Color24 b) {
			return a.R == b.R & a.G == b.G & a.B == b.B;
		}
		/// <summary>Checks whether two colors are unequal.</summary>
		/// <param name="a">The first color.</param>
		/// <param name="b">The second color.</param>
		/// <returns>Whether the two colors are unequal.</returns>
		public static bool operator !=(Color24 a, Color24 b) {
			return a.R != b.R | a.G != b.G | a.B != b.B;
		}
		// --- read-only fields ---
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
	
	// --- color 32 ---
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
		public Color32(byte r, byte g, byte b, byte a) {
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
		public Color32(byte r, byte g, byte b) {
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = 255;
		}
		/// <summary>Creates a new color.</summary>
		/// <param name="color">The solid color.</param>
		/// <param name="a">The alpha component.</param>
		public Color32(Color24 color, byte a) {
			this.R = color.R;
			this.G = color.G;
			this.B = color.B;
			this.A = a;
		}
		/// <summary>Creates a new color.</summary>
		/// <param name="color">The solid color.</param>
		/// <remarks>The alpha component is set to full opacity.</remarks>
		public Color32(Color24 color) {
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
		public static bool operator ==(Color32 a, Color32 b) {
			return a.R == b.R & a.G == b.G & a.B == b.B & a.A == b.A;
		}
		/// <summary>Checks whether two colors are unequal.</summary>
		/// <param name="a">The first color.</param>
		/// <param name="b">The second color.</param>
		/// <returns>Whether the two colors are unequal.</returns>
		public static bool operator !=(Color32 a, Color32 b) {
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
	
	// --- color 96 ---
	/// <summary>Represents a 96-bit color with red, green and blue channels at 32 bits each.</summary>
	public struct Color96 
	{
		// --- members ---
		/// <summary>The red component.</summary>
		public float R;
		/// <summary>The green component.</summary>
		public float G;
		/// <summary>The blue component.</summary>
		public float B;
		// --- constructors ---
		/// <summary>Creates a new color.</summary>
		/// <param name="r">The red component.</param>
		/// <param name="g">The green component.</param>
		/// <param name="b">The blue component.</param>
		public Color96(float r, float g, float b) {
			this.R = r;
			this.G = g;
			this.B = b;
		}
		// --- operators ---
		/// <summary>Checks whether two colors are equal.</summary>
		/// <param name="a">The first color.</param>
		/// <param name="b">The second color.</param>
		/// <returns>Whether the two colors are equal.</returns>
		public static bool operator ==(Color96 a, Color96 b) {
			return a.R == b.R & a.G == b.G & a.B == b.B;
		}
		/// <summary>Checks whether two colors are unequal.</summary>
		/// <param name="a">The first color.</param>
		/// <param name="b">The second color.</param>
		/// <returns>Whether the two colors are unequal.</returns>
		public static bool operator !=(Color96 a, Color96 b) {
			return a.R != b.R | a.G != b.G | a.B != b.B;
		}
		// --- read-only fields ---
		/// <summary>Represents a black color.</summary>
		public static readonly Color96 Black = new Color96(0.0f, 0.0f, 0.0f);
		/// <summary>Represents a red color.</summary>
		public static readonly Color96 Red = new Color96(1.0f, 0.0f, 0.0f);
		/// <summary>Represents a green color.</summary>
		public static readonly Color96 Green = new Color96(0.0f, 1.0f, 0.0f);
		/// <summary>Represents a blue color.</summary>
		public static readonly Color96 Blue = new Color96(0.0f, 0.0f, 1.0f);
		/// <summary>Represents a cyan color.</summary>
		public static readonly Color96 Cyan = new Color96(0.0f, 1.0f, 1.0f);
		/// <summary>Represents a magenta color.</summary>
		public static readonly Color96 Magenta = new Color96(1.0f, 0.0f, 1.0f);
		/// <summary>Represents a yellow color.</summary>
		public static readonly Color96 Yellow = new Color96(1.0f, 1.0f, 0.0f);
		/// <summary>Represents a white color.</summary>
		public static readonly Color96 White = new Color96(1.0f, 1.0f, 1.0f);

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
	
	// --- color 128 ---
	/// <summary>Represents a 128-bit color with red, green, blue and alpha channels at 32 bits each.</summary>
	public struct Color128 
	{
		// --- members ---
		/// <summary>The red component.</summary>
		public float R;
		/// <summary>The green component.</summary>
		public float G;
		/// <summary>The blue component.</summary>
		public float B;
		/// <summary>The alpha component.</summary>
		public float A;
		// --- constructors ---
		/// <summary>Creates a new color.</summary>
		/// <param name="r">The red component.</param>
		/// <param name="g">The green component.</param>
		/// <param name="b">The blue component.</param>
		/// <param name="a">The alpha component.</param>
		public Color128(float r, float g, float b, float a) {
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
		public Color128(float r, float g, float b) {
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = 1.0f;
		}
		/// <summary>Creates a new color.</summary>
		/// <param name="color">The solid color.</param>
		/// <param name="a">The alpha component.</param>
		public Color128(Color24 color, float a) {
			this.R = color.R;
			this.G = color.G;
			this.B = color.B;
			this.A = a;
		}
		/// <summary>Creates a new color.</summary>
		/// <param name="color">The solid color.</param>
		/// <remarks>The alpha component is set to full opacity.</remarks>
		public Color128(Color24 color) {
			this.R = color.R;
			this.G = color.G;
			this.B = color.B;
			this.A = 1.0f;
		}
		// --- operators ---
		/// <summary>Checks whether two colors are equal.</summary>
		/// <param name="a">The first color.</param>
		/// <param name="b">The second color.</param>
		/// <returns>Whether the two colors are equal.</returns>
		public static bool operator ==(Color128 a, Color128 b) {
			return a.R == b.R & a.G == b.G & a.B == b.B & a.A == b.A;
		}
		/// <summary>Checks whether two colors are unequal.</summary>
		/// <param name="a">The first color.</param>
		/// <param name="b">The second color.</param>
		/// <returns>Whether the two colors are unequal.</returns>
		public static bool operator !=(Color128 a, Color128 b) {
			return a.R != b.R | a.G != b.G | a.B != b.B | a.A != b.A;
		}
		// --- read-only fields ---
		/// <summary>Represents a black color.</summary>
		public static readonly Color128 Black = new Color128(0.0f, 0.0f, 0.0f);
		/// <summary>Represents a red color.</summary>
		public static readonly Color128 Red = new Color128(1.0f, 0.0f, 0.0f);
		/// <summary>Represents a green color.</summary>
		public static readonly Color128 Green = new Color128(0.0f, 1.0f, 0.0f);
		/// <summary>Represents a blue color.</summary>
		public static readonly Color128 Blue = new Color128(0.0f, 0.0f, 1.0f);
		/// <summary>Represents a cyan color.</summary>
		public static readonly Color128 Cyan = new Color128(0.0f, 1.0f, 1.0f);
		/// <summary>Represents a magenta color.</summary>
		public static readonly Color128 Magenta = new Color128(1.0f, 0.0f, 1.0f);
		/// <summary>Represents a yellow color.</summary>
		public static readonly Color128 Yellow = new Color128(1.0f, 1.0f, 0.0f);
		/// <summary>Represents a white color.</summary>
		public static readonly Color128 White = new Color128(1.0f, 1.0f, 1.0f);
		/// <summary>Represents a transparent black color.</summary>
		public static readonly Color128 Transparent = new Color128(0.0f, 0.0f, 0.0f, 0.0f);

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