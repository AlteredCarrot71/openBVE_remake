using System.Collections.Generic;

namespace OpenBve.Worlds.Mesh
{
    public struct Material
    {
        /// <summary>A bit mask combining constants of the MeshMaterial structure.</summary>
        internal byte Flags;
        internal Colors.ColorRGBA Color;
        internal Colors.ColorRGB TransparentColor;
        internal Colors.ColorRGB EmissiveColor;
        internal int DaytimeTextureIndex;
        internal int NighttimeTextureIndex;
        /// <summary>A value between 0 (daytime) and 255 (nighttime).</summary>
        internal byte DaytimeNighttimeBlend;
        internal Mesh.MaterialBlendMode BlendMode;
        /// <summary>A bit mask specifying the glow properties. Use GetGlowAttenuationData to create valid data for this field.</summary>
        internal short GlowAttenuationData;
        internal const int EmissiveColorMask = 1;
        internal const int TransparentColorMask = 2;

        // constructor
        public Material(byte flags, Colors.ColorRGBA color, Colors.ColorRGB transparentColor, Colors.ColorRGB emissiveColor, int daytimeTextureIndex, int nighttimeTextureIndex, byte daytimeNighttimeBlend, MaterialBlendMode blendMode, short glowAttenuationData)
        {
            Flags = flags;
            Color = color;
            TransparentColor = transparentColor;
            EmissiveColor = emissiveColor;
            DaytimeTextureIndex = daytimeTextureIndex;
            NighttimeTextureIndex = nighttimeTextureIndex;
            DaytimeNighttimeBlend = daytimeNighttimeBlend;
            BlendMode = blendMode;
            GlowAttenuationData = glowAttenuationData;
        }

        // operators
        public static bool operator ==(Material A, Material B)
        {
            if (A.Flags != B.Flags) return false;
            if (A.Color.R != B.Color.R | A.Color.G != B.Color.G | A.Color.B != B.Color.B | A.Color.A != B.Color.A) return false;
            if (A.TransparentColor.R != B.TransparentColor.R | A.TransparentColor.G != B.TransparentColor.G | A.TransparentColor.B != B.TransparentColor.B) return false;
            if (A.EmissiveColor.R != B.EmissiveColor.R | A.EmissiveColor.G != B.EmissiveColor.G | A.EmissiveColor.B != B.EmissiveColor.B) return false;
            if (A.DaytimeTextureIndex != B.DaytimeTextureIndex) return false;
            if (A.NighttimeTextureIndex != B.NighttimeTextureIndex) return false;
            if (A.BlendMode != B.BlendMode) return false;
            if (A.GlowAttenuationData != B.GlowAttenuationData) return false;
            return true;
        }
        public static bool operator !=(Material A, Material B)
        {
            if (A.Flags != B.Flags) return true;
            if (A.Color.R != B.Color.R | A.Color.G != B.Color.G | A.Color.B != B.Color.B | A.Color.A != B.Color.A) return true;
            if (A.TransparentColor.R != B.TransparentColor.R | A.TransparentColor.G != B.TransparentColor.G | A.TransparentColor.B != B.TransparentColor.B) return true;
            if (A.EmissiveColor.R != B.EmissiveColor.R | A.EmissiveColor.G != B.EmissiveColor.G | A.EmissiveColor.B != B.EmissiveColor.B) return true;
            if (A.DaytimeTextureIndex != B.DaytimeTextureIndex) return true;
            if (A.NighttimeTextureIndex != B.NighttimeTextureIndex) return true;
            if (A.BlendMode != B.BlendMode) return true;
            if (A.GlowAttenuationData != B.GlowAttenuationData) return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is Material material &&
                   Flags == material.Flags &&
                   EqualityComparer<Colors.ColorRGBA>.Default.Equals(Color, material.Color) &&
                   EqualityComparer<Colors.ColorRGB>.Default.Equals(TransparentColor, material.TransparentColor) &&
                   EqualityComparer<Colors.ColorRGB>.Default.Equals(EmissiveColor, material.EmissiveColor) &&
                   DaytimeTextureIndex == material.DaytimeTextureIndex &&
                   NighttimeTextureIndex == material.NighttimeTextureIndex &&
                   DaytimeNighttimeBlend == material.DaytimeNighttimeBlend &&
                   EqualityComparer<MaterialBlendMode>.Default.Equals(BlendMode, material.BlendMode) &&
                   GlowAttenuationData == material.GlowAttenuationData;
        }

        public override int GetHashCode()
        {
            int hashCode = -672346328;
            hashCode = hashCode * -1521134295 + Flags.GetHashCode();
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            hashCode = hashCode * -1521134295 + TransparentColor.GetHashCode();
            hashCode = hashCode * -1521134295 + EmissiveColor.GetHashCode();
            hashCode = hashCode * -1521134295 + DaytimeTextureIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + NighttimeTextureIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + DaytimeNighttimeBlend.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<MaterialBlendMode>.Default.GetHashCode(BlendMode);
            hashCode = hashCode * -1521134295 + GlowAttenuationData.GetHashCode();
            return hashCode;
        }
    }
}
