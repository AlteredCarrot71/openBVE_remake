using System;

namespace OpenBveApi.Textures
{
	/// <summary>Represents a texture.</summary>
	public class Texture
	{
		// --- members ---
		/// <summary>The width of the texture in pixels.</summary>
		private readonly int MyWidth;
		/// <summary>The height of the texture in pixels.</summary>
		private readonly int MyHeight;
		/// <summary>The number of bits per pixel. Must be 32.</summary>
		private readonly int MyBitsPerPixel;
		/// <summary>The texture data. Pixels are stored row-based from top to bottom, and within a row from left to right. For 32 bits per pixel, four bytes are used in the order red, green, blue and alpha.</summary>
		private readonly byte[] MyBytes;
		// --- constructors ---
		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="width">The width of the texture in pixels.</param>
		/// <param name="height">The height of the texture in pixels.</param>
		/// <param name="bitsPerPixel">The number of bits per pixel. Must be 24 or 32.</param>
		/// <param name="bytes">The texture data. Pixels are stored row-based from top to bottom, and within a row from left to right. For 32 bits per pixel, four bytes are used in the order red, green, blue and alpha.</param>
		/// <exception cref="System.ArgumentException">Raised when the number of bits per pixel is not 32.</exception>
		/// <exception cref="System.ArgumentNullException">Raised when the byte array is a null reference.</exception>
		/// <exception cref="System.ArgumentException">Raised when the byte array is of unexpected length.</exception>
		public Texture(int width, int height, int bitsPerPixel, byte[] bytes)
		{
			if (bitsPerPixel != 32)
			{
				throw new ArgumentException("The number of bits per pixel is supported.");
			}
			else if (bytes == null)
			{
				throw new ArgumentNullException("The data bytes are a null reference.");
			}
			else if (bytes.Length != 4 * width * height)
			{
				throw new ArgumentException("The data bytes are not of the expected length.");
			}
			else
			{
				this.MyWidth = width;
				this.MyHeight = height;
				this.MyBitsPerPixel = bitsPerPixel;
				this.MyBytes = bytes;
			}
		}
		// --- properties ---
		/// <summary>Gets the width of the texture in pixels.</summary>
		public int Width
		{
			get
			{
				return this.MyWidth;
			}
		}
		/// <summary>Gets the height of the texture in pixels.</summary>
		public int Height
		{
			get
			{
				return this.MyHeight;
			}
		}
		/// <summary>Gets the number of bits per pixel.</summary>
		public int BitsPerPixel
		{
			get
			{
				return this.MyBitsPerPixel;
			}
		}
		/// <summary>Gets the texture data. Pixels are stored row-based from top to bottom, and within a row from left to right. For 32 bits per pixel, four bytes are used in the order red, green, blue and alpha.</summary>
		public byte[] Bytes
		{
			get
			{
				return this.MyBytes;
			}
		}
		// --- operators ---
		/// <summary>Checks whether two textures are equal.</summary>
		/// <param name="a">The first texture.</param>
		/// <param name="b">The second texture.</param>
		/// <returns>Whether the two textures are equal.</returns>
		public static bool operator ==(Texture a, Texture b)
		{
			if (object.ReferenceEquals(a, b)) return true;
			if (a is null) return false;
			if (b is null) return false;
			if (a.MyWidth != b.MyWidth) return false;
			if (a.MyHeight != b.MyHeight) return false;
			if (a.MyBitsPerPixel != b.MyBitsPerPixel) return false;
			if (a.MyBytes.Length != b.MyBytes.Length) return false;
			for (int i = 0; i < a.MyBytes.Length; i++)
			{
				if (a.MyBytes[i] != b.MyBytes[i]) return false;
			}
			return true;
		}
		/// <summary>Checks whether two textures are unequal.</summary>
		/// <param name="a">The first texture.</param>
		/// <param name="b">The second texture.</param>
		/// <returns>Whether the two textures are unequal.</returns>
		public static bool operator !=(Texture a, Texture b)
		{
			if (object.ReferenceEquals(a, b)) return false;
			if (a is null) return true;
			if (b is null) return true;
			if (a.MyWidth != b.MyWidth) return true;
			if (a.MyHeight != b.MyHeight) return true;
			if (a.MyBitsPerPixel != b.MyBitsPerPixel) return true;
			if (a.MyBytes.Length != b.MyBytes.Length) return true;
			for (int i = 0; i < a.MyBytes.Length; i++)
			{
				if (a.MyBytes[i] != b.MyBytes[i]) return true;
			}
			return false;
		}
		/// <summary>Checks whether this instance is equal to the specified object.</summary>
		/// <param name="obj">The object.</param>
		/// <returns>Whether this instance is equal to the specified object.</returns>
		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj)) return true;
			if (this is null) return false;
			if (obj is null) return false;
			if (!(obj is Texture)) return false;
			Texture x = (Texture)obj;
			if (this.MyWidth != x.MyWidth) return false;
			if (this.MyHeight != x.MyHeight) return false;
			if (this.MyBitsPerPixel != x.MyBitsPerPixel) return false;
			if (this.MyBytes.Length != x.MyBytes.Length) return false;
			for (int i = 0; i < this.MyBytes.Length; i++)
			{
				if (this.MyBytes[i] != x.MyBytes[i]) return false;
			}
			return true;
		}
		// --- functions ---
		/// <summary>Gets the type of transparency encountered in this texture.</summary>
		/// <returns>The type of transparency encountered in this texture.</returns>
		/// <exception cref="System.NotSupportedException">Raised when the bits per pixel in the texture is not supported.</exception>
		public TextureTransparencyType GetTransparencyType()
		{
			if (this.MyBitsPerPixel == 24)
			{
				return TextureTransparencyType.Opaque;
			}
			else if (this.MyBitsPerPixel == 32)
			{
				for (int i = 3; i < this.MyBytes.Length; i += 4)
				{
					if (this.MyBytes[i] != 255)
					{
						for (int j = i; j < this.MyBytes.Length; j += 4)
						{
							if (this.MyBytes[j] != 0)
							{
								return TextureTransparencyType.Alpha;
							}
						}
						return TextureTransparencyType.Partial;
					}
				}
				return TextureTransparencyType.Opaque;
			}
			else
			{
				throw new NotSupportedException();
			}
		}
		/// <summary>Applies the specified parameters onto a texture.</summary>
		/// <param name="texture">The original texture.</param>
		/// <param name="parameters">The parameters, or a null reference.</param>
		/// <returns>The texture with the parameters applied.</returns>
		/// <exception cref="System.ArgumentException">Raised when the clip region is outside the texture bounds.</exception>
		/// <exception cref="System.NotSupportedException">Raised when the bits per pixel in the texture is not supported.</exception>
		public static Texture ApplyParameters(Texture texture, TextureParameters parameters)
		{
			return Functions.ApplyParameters(texture, parameters);
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}
	}
}