namespace OpenBve.Worlds.Mesh
{
    /// <summary>Represents a mesh consisting of a series of vertices, faces and material properties.</summary>
    internal struct Mesh
    {
        internal Vertex[] Vertices;
        internal Material[] Materials;
        internal Face[] Faces;
        /// <summary>Creates a mesh consisting of one face, which is represented by individual vertices, and a color.</summary>
        /// <param name="Vertices">The vertices that make up one face.</param>
        /// <param name="Color">The color to be applied on the face.</param>
        public Mesh(Vertex[] Vertices, Colors.ColorRGBA Color)
        {
            this.Vertices = Vertices;
            this.Materials = new Worlds.Mesh.Material[1];
            this.Materials[0].Color = Color;
            this.Materials[0].DaytimeTextureIndex = -1;
            this.Materials[0].NighttimeTextureIndex = -1;
            this.Faces = new Worlds.Mesh.Face[1];
            this.Faces[0].Material = 0;
            this.Faces[0].Vertices = new Worlds.Mesh.FaceVertex[Vertices.Length];
            for (int i = 0; i < Vertices.Length; i++)
            {
                this.Faces[0].Vertices[i].Index = (short)i;
            }
        }
        /// <summary>Creates a mesh consisting of the specified vertices, faces and color.</summary>
        /// <param name="Vertices">The vertices used.</param>
        /// <param name="FaceVertices">A list of faces represented by a list of references to vertices.</param>
        /// <param name="Color">The color to be applied on all of the faces.</param>
        public Mesh(Vertex[] Vertices, int[][] FaceVertices, Colors.ColorRGBA Color)
        {
            this.Vertices = Vertices;
            this.Materials = new Worlds.Mesh.Material[1];
            this.Materials[0].Color = Color;
            this.Materials[0].DaytimeTextureIndex = -1;
            this.Materials[0].NighttimeTextureIndex = -1;
            this.Faces = new Worlds.Mesh.Face[FaceVertices.Length];
            for (int i = 0; i < FaceVertices.Length; i++)
            {
                this.Faces[i] = new Worlds.Mesh.Face(FaceVertices[i]);
            }
        }
    }
}
