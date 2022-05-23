namespace OpenBve.Worlds.Mesh
{
    /// <summary>Represents a face consisting of vertices and material attributes.</summary>
    internal struct Face
    {
        internal Worlds.Mesh.FaceVertex[] Vertices;
        /// <summary>A reference to an element in the Material array of the containing Mesh structure.</summary>
        internal short Material;
        /// <summary>A bit mask combining constants of the Face structure.</summary>
        internal byte Flags;
        internal Face(int[] Vertices)
        {
            this.Vertices = new Worlds.Mesh.FaceVertex[Vertices.Length];
            for (int i = 0; i < Vertices.Length; i++)
            {
                this.Vertices[i] = new Worlds.Mesh.FaceVertex(Vertices[i]);
            }
            this.Material = 0;
            this.Flags = 0;
        }
        internal void Flip()
        {
            if ((this.Flags & FaceTypeMask) == FaceTypeQuadStrip)
            {
                for (int i = 0; i < this.Vertices.Length; i += 2)
                {
                    (this.Vertices[i + 1], this.Vertices[i]) = (this.Vertices[i], this.Vertices[i + 1]);
                }
            }
            else
            {
                int n = this.Vertices.Length;
                for (int i = 0; i < (n >> 1); i++)
                {
                    (this.Vertices[n - i - 1], this.Vertices[i]) = (this.Vertices[i], this.Vertices[n - i - 1]);
                }
            }
        }
        internal const int FaceTypeMask = 7;
        internal const int FaceTypePolygon = 0;
        internal const int FaceTypeTriangles = 1;
        internal const int FaceTypeTriangleStrip = 2;
        internal const int FaceTypeQuads = 3;
        internal const int FaceTypeQuadStrip = 4;
        internal const int Face2Mask = 8;
    }
}
