namespace OpenBveApi.Runtime
{
    /// <summary>Represents the current state of the train.</summary>
    public class VehicleState
    {
        // --- members ---
        /// <summary>The location of the front of the train, in meters.</summary>
        public double Location { get; private set; }
        /// <summary>The speed of the train.</summary>
        public Speed Speed { get; private set; }
        /// <summary>The pressure in the brake cylinder, in pascal.</summary>
        public double BcPressure { get; private set; }
        /// <summary>The pressure in the main reservoir, in pascal.</summary>
        public double MrPressure { get; private set; }
        /// <summary>The pressure in the emergency reservoir, in pascal.</summary>
        public double ErPressure { get; private set; }
        /// <summary>The pressure in the brake pipe, in pascal.</summary>
        public double BpPressure { get; private set; }
        /// <summary>The pressure in the straight air pipe, in pascal.</summary>
        public double SapPressure { get; private set; }

        // --- constructors ---
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="location">The location of the front of the train, in meters.</param>
        /// <param name="speed">The speed of the train.</param>
        /// <param name="bcPressure">The pressure in the brake cylinder, in pascal.</param>
        /// <param name="mrPressure">The pressure in the main reservoir, in pascal.</param>
        /// <param name="erPressure">The pressure in the emergency reservoir, in pascal.</param>
        /// <param name="bpPressure">The pressure in the brake pipe, in pascal.</param>
        /// <param name="sapPressure">The pressure in the straight air pipe, in pascal.</param>
        public VehicleState(double location, Speed speed, double bcPressure, double mrPressure, double erPressure, double bpPressure, double sapPressure)
        {
            this.Location = location;
            this.Speed = speed;
            this.BcPressure = bcPressure;
            this.MrPressure = mrPressure;
            this.ErPressure = erPressure;
            this.BpPressure = bpPressure;
            this.SapPressure = sapPressure;
        }
    }
}