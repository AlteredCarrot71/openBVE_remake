namespace OpenBveApi.Runtime
{
	/// <summary>Represents a speed.</summary>
	public class Speed
	{
		// --- members ---
		/// <summary>The speed in meters per second.</summary>
		private readonly double Mps;

		// --- properties ---
		/// <summary>Gets the speed in meters per second.</summary>
		public double MetersPerSecond
		{
			get
			{
				return this.Mps;
			}
		}
		/// <summary>Gets the speed in kilometes per hour.</summary>
		public double KilometersPerHour
		{
			get
			{
				return 3.6 * this.Mps;
			}
		}
		/// <summary>Gets the speed in miles per hour.</summary>
		public double MilesPerHour
		{
			get
			{
				return 2.236936 * this.Mps;
			}
		}

		// --- constructors ---
		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="value">The speed in meters per second.</param>
		public Speed(double value)
		{
			this.Mps = value;
		}
	}
}