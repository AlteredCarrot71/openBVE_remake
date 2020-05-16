using System;

namespace OpenBveApi.Textures
{
	/// <summary>Represents a region in a texture to be extracted.</summary>
	public class TextureClipRegion
	{
		// --- members ---
		/// <summary>The left coordinate.</summary>
		public int Left { get; private set; }
		/// <summary>The top coordinate.</summary>
		public int Top { get; private set; }
		/// <summary>The width.</summary>
		public int Width { get; private set; }
		/// <summary>The height.</summary>
		public int Height { get; private set; }
		
		// --- constructors ---
		/// <summary>Creates a new clip region.</summary>
		/// <param name="left">The left coordinate.</param>
		/// <param name="top">The top coordinate.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <exception cref="System.ArgumentException">Raised when the left or top are negative.</exception>
		/// <exception cref="System.ArgumentException">Raised when the width or height are non-positive.</exception>
		public TextureClipRegion(int left, int top, int width, int height)
		{
			if (left < 0 | top < 0)
			{
				throw new ArgumentException("The left or top coordinates are negative.");
			}
			else if (width <= 0 | height <= 0)
			{
				throw new ArgumentException("The width or height are non-positive.");
			}
			else
			{
				this.Left = left;
				this.Top = top;
				this.Width = width;
				this.Height = height;
			}
		}
		// --- operators ---
		/// <summary>Checks whether two clip regions are equal.</summary>
		/// <param name="a">The first clip region.</param>
		/// <param name="b">The second clip region.</param>
		/// <returns>Whether the two clip regions are equal.</returns>
		public static bool operator ==(TextureClipRegion a, TextureClipRegion b)
		{
			if (object.ReferenceEquals(a, b)) return true;
			if (a is null) return false;
			if (b is null) return false;
			if (a.Left != b.Left) return false;
			if (a.Top != b.Top) return false;
			if (a.Width != b.Width) return false;
			if (a.Height != b.Height) return false;
			return true;
		}
		/// <summary>Checks whether two clip regions are unequal.</summary>
		/// <param name="a">The first clip region.</param>
		/// <param name="b">The second clip region.</param>
		/// <returns>Whether the two clip regions are unequal.</returns>
		public static bool operator !=(TextureClipRegion a, TextureClipRegion b)
		{
			if (object.ReferenceEquals(a, b)) return false;
			if (a is null) return true;
			if (b is null) return true;
			if (a.Left != b.Left) return true;
			if (a.Top != b.Top) return true;
			if (a.Width != b.Width) return true;
			if (a.Height != b.Height) return true;
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
			if (!(obj is TextureClipRegion)) return false;
			TextureClipRegion x = (TextureClipRegion)obj;
			if (this.Left != x.Left) return false;
			if (this.Top != x.Top) return false;
			if (this.Width != x.Width) return false;
			if (this.Height != x.Height) return false;
			return true;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}
	}
}