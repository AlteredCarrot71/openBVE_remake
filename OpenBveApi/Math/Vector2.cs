using System;

namespace OpenBveApi.Math
{
    /// <summary>Represents a two-dimensional vector.</summary>
    public class Vector2
    {
        #region members
        /// <summary>The x-coordinate.</summary>
        public double X { get; private set; }

        /// <summary>The y-coordinate.</summary>
        public double Y { get; private set; }
        #endregion

        #region constructors
        /// <summary>Creates a new two-dimensional vector.</summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        public Vector2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        #endregion

        #region operators
        /// <summary>Checks whether the two specified vectors are equal.</summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>Whether the two vectors are equal.</returns>
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            if (a.X != b.X) return false;
            if (a.Y != b.Y) return false;
            return true;
        }

        /// <summary>Checks whether the two specified vectors are unequal.</summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>Whether the two vectors are unequal.</returns>
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            if (a.X != b.X) return true;
            if (a.Y != b.Y) return true;
            return false;
        }
        #endregion

        #region read-only fields
        /// <summary>Represents a null vector.</summary>
        public static readonly Vector2 Null = new Vector2(0.0, 0.0);

        /// <summary>Represents a vector pointing left.</summary>
        public static readonly Vector2 Left = new Vector2(-1.0, 0.0);

        /// <summary>Represents a vector pointing right.</summary>
        public static readonly Vector2 Right = new Vector2(1.0, 0.0);

        /// <summary>Represents a vector pointing up.</summary>
        public static readonly Vector2 Up = new Vector2(0.0, -1.0);

        /// <summary>Represents a vector pointing down.</summary>
        public static readonly Vector2 Down = new Vector2(0.0, 1.0);
        #endregion

        #region methods
        /// <summary>
        ///  
        /// </summary>
        /// <param name="cosa"></param>
        /// <param name="sina"></param>
        public void Rotate(double cosa, double sina)
        {
            this.X = (this.X * cosa) - (this.Y * sina);
            this.Y = (this.X * sina) + (this.Y * cosa);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Normalize()
        {
            double t = (this.X * this.X) + (this.Y * this.Y);
            if (t != 0.0)
            {
                t = 1.0 / System.Math.Sqrt(t);
                this.X *= t;
                this.Y *= t;
            }
        }
        #endregion

        #region common override
        /// <summary>Check whether the specified vectors are equal.</summary>
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns the hash code for this instance.</summary>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
