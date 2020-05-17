namespace OpenBveApi.Runtime
{
    /// <summary>Represents data trasmitted by a beacon.</summary>
    public class BeaconData
    {
        // --- members ---
        /// <summary>The type of beacon.</summary>
        public int Type { get; private set; }
        /// <summary>Optional data the beacon transmits.</summary>
        public int Optional { get; private set; }
        /// <summary>The section the beacon is attached to.</summary>
        public SignalData Signal { get; private set; }

        // --- constructors ---
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="type">The type of beacon.</param>
        /// <param name="optional">Optional data the beacon transmits.</param>
        /// <param name="signal">The section the beacon is attached to.</param>
        public BeaconData(int type, int optional, SignalData signal)
        {
            this.Type = type;
            this.Optional = optional;
            this.Signal = signal;
        }
    }
}