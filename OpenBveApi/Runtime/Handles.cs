namespace OpenBveApi.Runtime
{
	/// <summary>Represents the handles of the cab.</summary>
	public class Handles
	{
		// --- members ---
		/// <summary>The reverser position.</summary>
		public int Reverser { get; set; }
		/// <summary>The power notch.</summary>
		public int PowerNotch { get; set; }
		/// <summary>The brake notch.</summary>
		public int BrakeNotch { get; set; }
		/// <summary>Whether the const speed system is enabled.</summary>
		public bool ConstSpeed { get; set; }

		// --- constructors ---
		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="reverser">The current reverser position.</param>
		/// <param name="powerNotch">The current power notch.</param>
		/// <param name="brakeNotch">The current brake notch.</param>
		/// <param name="constSpeed">Whether the const speed system is enabled.</param>
		public Handles(int reverser, int powerNotch, int brakeNotch, bool constSpeed)
		{
			this.Reverser = reverser;
			this.PowerNotch = powerNotch;
			this.BrakeNotch = brakeNotch;
			this.ConstSpeed = constSpeed;
		}
	}
}