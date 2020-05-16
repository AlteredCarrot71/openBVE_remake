namespace OpenBveApi.Runtime
{
	/// <summary>Represents the specification of the train.</summary>
	public class VehicleSpecs
	{
		// --- members ---
		/// <summary>The number of power notches the train has.</summary>
		public int PowerNotches { get; private set; }
		/// <summary>The type of brake the train uses.</summary>
		public BrakeTypes BrakeType { get; private set; }
		/// <summary>The number of brake notches the train has, including the hold brake, but excluding the emergency brake.</summary>
		public int BrakeNotches { get; private set; }
		/// <summary>Whether the train has a hold brake.</summary>
		public bool HasHoldBrake { get; private set; }
		/// <summary>The number of cars the train has.</summary>
		public int Cars { get; private set; }
		
		// --- properties ---
		/// <summary>Gets the index of the brake notch that corresponds to B1 or LAP.</summary>
		/// <remarks>For trains without a hold brake, this returns 1. For trains with a hold brake, this returns 2.</remarks>
		public int AtsNotch
		{
			get
			{
				if (this.HasHoldBrake)
				{
					return 2;
				}
				else
				{
					return 1;
				}
			}
		}
		/// <summary>Gets the index of the brake notch that corresponds to 70% of the available brake notches.</summary>
		public int B67Notch
		{
			get
			{
				return (int)System.Math.Round(0.7 * this.BrakeNotches);
			}
		}
		// --- constructors ---
		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="powerNotches">The number of power notches the train has.</param>
		/// <param name="brakeType">The type of brake the train uses.</param>
		/// <param name="brakeNotches">The number of brake notches the train has, including the hold brake, but excluding the emergency brake.</param>
		/// <param name="hasHoldBrake">Whether the train has a hold brake.</param>
		/// <param name="cars">The number of cars the train has.</param>
		public VehicleSpecs(int powerNotches, BrakeTypes brakeType, int brakeNotches, bool hasHoldBrake, int cars)
		{
			this.PowerNotches = powerNotches;
			this.BrakeType = brakeType;
			this.BrakeNotches = brakeNotches;
			this.HasHoldBrake = hasHoldBrake;
			this.Cars = cars;
		}
	}
}