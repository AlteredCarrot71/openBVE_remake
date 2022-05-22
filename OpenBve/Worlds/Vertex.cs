using System.Collections.Generic;

namespace OpenBve.Worlds
{
    /// <summary>Represents a vertex consisting of 3D coordinates and 2D texture coordinates.</summary>
    public struct Vertex
    {
        internal Vectors.Vector3D Coordinates;
        internal Vectors.Vector2Df TextureCoordinates;
        public Vertex(Vectors.Vector3D Coordinates, Vectors.Vector2Df TextureCoordinates)
        {
            this.Coordinates = Coordinates;
            this.TextureCoordinates = TextureCoordinates;
        }

        // operators
        public static bool operator ==(Vertex A, Vertex B)
        {
            if (A.Coordinates.X != B.Coordinates.X | A.Coordinates.Y != B.Coordinates.Y | A.Coordinates.Z != B.Coordinates.Z) return false;
            if (A.TextureCoordinates.X != B.TextureCoordinates.X | A.TextureCoordinates.Y != B.TextureCoordinates.Y) return false;
            return true;
        }
        public static bool operator !=(Vertex A, Vertex B)
        {
            if (A.Coordinates.X != B.Coordinates.X | A.Coordinates.Y != B.Coordinates.Y | A.Coordinates.Z != B.Coordinates.Z) return true;
            if (A.TextureCoordinates.X != B.TextureCoordinates.X | A.TextureCoordinates.Y != B.TextureCoordinates.Y) return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is Vertex vertex &&
                   EqualityComparer<Vectors.Vector3D>.Default.Equals(Coordinates, vertex.Coordinates) &&
                   EqualityComparer<Vectors.Vector2Df>.Default.Equals(TextureCoordinates, vertex.TextureCoordinates);
        }

        public override int GetHashCode()
        {
            int hashCode = -1543404925;
            hashCode = (hashCode * -1521134295) + Coordinates.GetHashCode();
            hashCode = (hashCode * -1521134295) + TextureCoordinates.GetHashCode();
            return hashCode;
        }
    }
}
