namespace OpenBveApi.Runtime
{
    /// <summary>Represents the current state of the preceding train.</summary>
    public class PrecedingVehicleState
    {
        // --- members ---
        /// <summary>The location of the back of the preceding train, in meters.</summary>
        public double Location { get; private set; }
        /// <summary>The distance from the front of the current train to the back of the preceding train, in meters.</summary>
        public double Distance { get; private set; }
        /// <summary>The current speed of the preceding train.</summary>
        public Speed Speed { get; private set; }

        // --- constructors ---
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="location">Gets the location of the back of the preceding train, in meters.</param>
        /// <param name="distance">The distance from the front of the current train to the back of the preceding train, in meters.</param>
        /// <param name="speed">Gets the speed of the preceding train.</param>
        public PrecedingVehicleState(double location, double distance, Speed speed)
        {
            this.Location = location;
            this.Distance = distance;
            this.Speed = speed;
        }
    }
}