using System.Collections.Generic;

namespace OpenBve.Worlds.Mesh
{
    public struct FaceVertex
    {
        /// <summary>A reference to an element in the Vertex array of the contained Mesh structure.</summary>
        internal short Index;
        /// <summary>The normal to be used at the vertex.</summary>
        internal Worlds.Vector.Vector3Df Normal;
        
        public FaceVertex(int Index)
        {
            this.Index = (short)Index;
            this.Normal = new Worlds.Vector.Vector3Df(0.0f, 0.0f, 0.0f);
        }
        public FaceVertex(int Index, Worlds.Vector.Vector3Df Normal)
        {
            this.Index = (short)Index;
            this.Normal = Normal;
        }

        // operators
        public static bool operator ==(FaceVertex A, FaceVertex B)
        {
            if (A.Index != B.Index) return false;
            if (A.Normal.X != B.Normal.X) return false;
            if (A.Normal.Y != B.Normal.Y) return false;
            if (A.Normal.Z != B.Normal.Z) return false;
            return true;
        }
        public static bool operator !=(FaceVertex A, FaceVertex B)
        {
            if (A.Index != B.Index) return true;
            if (A.Normal.X != B.Normal.X) return true;
            if (A.Normal.Y != B.Normal.Y) return true;
            if (A.Normal.Z != B.Normal.Z) return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is FaceVertex vertex &&
                   Index == vertex.Index &&
                   EqualityComparer<Worlds.Vector.Vector3Df>.Default.Equals(Normal, vertex.Normal);
        }

        public override int GetHashCode()
        {
            int hashCode = -643893131;
            hashCode = (hashCode * -1521134295) + Index.GetHashCode();
            hashCode = (hashCode * -1521134295) + Normal.GetHashCode();
            return hashCode;
        }
    }
}
