using System;

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

        internal static void CreateNormals(ref Mesh Mesh)
        {
            for (int i = 0; i < Mesh.Faces.Length; i++)
            {
                CreateNormals(ref Mesh, i);
            }
        }
        internal static void CreateNormals(ref Mesh Mesh, int FaceIndex)
        {
            if (Mesh.Faces[FaceIndex].Vertices.Length >= 3)
            {
                int i0 = (int)Mesh.Faces[FaceIndex].Vertices[0].Index;
                int i1 = (int)Mesh.Faces[FaceIndex].Vertices[1].Index;
                int i2 = (int)Mesh.Faces[FaceIndex].Vertices[2].Index;
                double ax = Mesh.Vertices[i1].Coordinates.X - Mesh.Vertices[i0].Coordinates.X;
                double ay = Mesh.Vertices[i1].Coordinates.Y - Mesh.Vertices[i0].Coordinates.Y;
                double az = Mesh.Vertices[i1].Coordinates.Z - Mesh.Vertices[i0].Coordinates.Z;
                double bx = Mesh.Vertices[i2].Coordinates.X - Mesh.Vertices[i0].Coordinates.X;
                double by = Mesh.Vertices[i2].Coordinates.Y - Mesh.Vertices[i0].Coordinates.Y;
                double bz = Mesh.Vertices[i2].Coordinates.Z - Mesh.Vertices[i0].Coordinates.Z;
                double nx = (ay * bz) - (az * by);
                double ny = (az * bx) - (ax * bz);
                double nz = (ax * by) - (ay * bx);
                double t = (nx * nx) + (ny * ny) + (nz * nz);
                if (t != 0.0)
                {
                    t = 1.0 / Math.Sqrt(t);
                    float mx = (float)(nx * t);
                    float my = (float)(ny * t);
                    float mz = (float)(nz * t);
                    for (int j = 0; j < Mesh.Faces[FaceIndex].Vertices.Length; j++)
                    {
                        if (Mesh.Faces[FaceIndex].Vertices[j].Normal.IsZero())
                        {
                            Mesh.Faces[FaceIndex].Vertices[j].Normal = new Vectors.Vector3Df(mx, my, mz);
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < Mesh.Faces[FaceIndex].Vertices.Length; j++)
                    {
                        if (Mesh.Faces[FaceIndex].Vertices[j].Normal.IsZero())
                        {
                            Mesh.Faces[FaceIndex].Vertices[j].Normal = new Vectors.Vector3Df(0.0f, 1.0f, 0.0f);
                        }
                    }
                }
            }
        }
    }
}
