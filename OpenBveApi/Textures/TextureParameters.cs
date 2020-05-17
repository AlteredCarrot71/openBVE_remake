using OpenBveApi.Colors;
using System;

namespace OpenBveApi.Textures
{
    /// <summary>Represents additional parameters that specify how to process the texture.</summary>
    public class TextureParameters
    {
        // --- members ---
        /// <summary>The region in the texture to be extracted, or a null reference for the entire texture.</summary>
        public TextureClipRegion ClipRegion { get; private set; }
        /// <summary>The color in the texture that should become transparent, or a null reference for no transparent color.</summary>
        public Nullable<Color24> TransparentColor { get; private set; }

        // --- constructors ---
        /// <summary>Creates new texture parameters.</summary>
        /// <param name="clipRegion">The region in the texture to be extracted, or a null reference for the entire texture.</param>
        /// <param name="transparentColor">The color in the texture that should become transparent, or a null reference for no transparent color.</param>
        public TextureParameters(TextureClipRegion clipRegion, Nullable<Color24> transparentColor)
        {
            this.ClipRegion = clipRegion;
            this.TransparentColor = transparentColor;
        }
        // --- operators ---
        /// <summary>Checks whether two texture parameters are equal.</summary>
        /// <param name="a">The first texture parameter.</param>
        /// <param name="b">The second texture parameter.</param>
        /// <returns>Whether the two texture parameters are equal.</returns>
        public static bool operator ==(TextureParameters a, TextureParameters b)
        {
            if (object.ReferenceEquals(a, b)) return true;
            if (a is null) return false;
            if (b is null) return false;
            if (a.ClipRegion != b.ClipRegion) return false;
            if (a.TransparentColor != b.TransparentColor) return false;
            return true;
        }
        /// <summary>Checks whether two texture parameters are unequal.</summary>
        /// <param name="a">The first texture parameter.</param>
        /// <param name="b">The second texture parameter.</param>
        /// <returns>Whether the two texture parameters are unequal.</returns>
        public static bool operator !=(TextureParameters a, TextureParameters b)
        {
            if (object.ReferenceEquals(a, b)) return false;
            if (a is null) return true;
            if (b is null) return true;
            if (a.ClipRegion != b.ClipRegion) return true;
            if (a.TransparentColor != b.TransparentColor) return true;
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
            if (!(obj is TextureParameters)) return false;
            TextureParameters x = (TextureParameters)obj;
            if (this.ClipRegion != x.ClipRegion) return false;
            if (this.TransparentColor != x.TransparentColor) return false;
            return true;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}