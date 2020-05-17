namespace OpenBveApi.Runtime
{
    /// <summary>Represents data given to the plugin in the Elapse call.</summary>
    public class ElapseData
    {
        // --- members ---
        /// <summary>The state of the train.</summary>
        public VehicleState Vehicle { get; private set; }
        /// <summary>The state of the preceding train, or a null reference if there is no preceding train.</summary>
        public PrecedingVehicleState PrecedingVehicle { get; private set; }
        /// <summary>The virtual handles.</summary>
        public Handles Handles { get; set; }
        /// <summary>The current absolute time.</summary>
        public Time TotalTime { get; private set; }
        /// <summary>The elapsed time since the last call to Elapse.</summary>
        public Time ElapsedTime { get; private set; }
        /// <summary>The debug message the plugin wants the host application to display.</summary>
        public string DebugMessage { get; set; }

        // --- constructors ---
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="vehicle">The state of the train.</param>
        /// <param name="precedingVehicle">The state of the preceding train, or a null reference if there is no preceding train.</param>
        /// <param name="handles">The virtual handles.</param>
        /// <param name="totalTime">The current absolute time.</param>
        /// <param name="elapsedTime">The elapsed time since the last call to Elapse.</param>
        public ElapseData(VehicleState vehicle, PrecedingVehicleState precedingVehicle, Handles handles, Time totalTime, Time elapsedTime)
        {
            this.Vehicle = vehicle;
            this.PrecedingVehicle = precedingVehicle;
            this.Handles = handles;
            this.TotalTime = totalTime;
            this.ElapsedTime = elapsedTime;
            this.DebugMessage = null;
        }
    }
}