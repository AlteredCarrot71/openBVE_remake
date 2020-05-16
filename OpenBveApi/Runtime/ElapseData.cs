namespace OpenBveApi.Runtime
{
	/// <summary>Represents data given to the plugin in the Elapse call.</summary>
	public class ElapseData
	{
		// --- members ---
		/// <summary>The state of the train.</summary>
		private readonly VehicleState MyVehicle;
		/// <summary>The state of the preceding train, or a null reference if there is no preceding train.</summary>
		private readonly PrecedingVehicleState MyPrecedingVehicle;
		/// <summary>The virtual handles.</summary>
		private Handles MyHandles;
		/// <summary>The current absolute time.</summary>
		private readonly Time MyTotalTime;
		/// <summary>The elapsed time since the last call to Elapse.</summary>
		private readonly Time MyElapsedTime;
		/// <summary>The debug message the plugin wants the host application to display.</summary>
		private string MyDebugMessage;
		// --- constructors ---
		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="vehicle">The state of the train.</param>
		/// <param name="precedingVehicle">The state of the preceding train, or a null reference if there is no preceding train.</param>
		/// <param name="handles">The virtual handles.</param>
		/// <param name="totalTime">The current absolute time.</param>
		/// <param name="elapsedTime">The elapsed time since the last call to Elapse.</param>
		public ElapseData(VehicleState vehicle, PrecedingVehicleState precedingVehicle, Handles handles, Time totalTime, Time elapsedTime)
		{
			this.MyVehicle = vehicle;
			this.MyPrecedingVehicle = precedingVehicle;
			this.MyHandles = handles;
			this.MyTotalTime = totalTime;
			this.MyElapsedTime = elapsedTime;
			this.MyDebugMessage = null;
		}
		// --- properties ---
		/// <summary>Gets the state of the train.</summary>
		public VehicleState Vehicle
		{
			get
			{
				return this.MyVehicle;
			}
		}
		/// <summary>Gets the state of the preceding train, or a null reference if there is no preceding train.</summary>
		public PrecedingVehicleState PrecedingVehicle
		{
			get
			{
				return this.MyPrecedingVehicle;
			}
		}
		/// <summary>Gets or sets the virtual handles.</summary>
		public Handles Handles
		{
			get
			{
				return this.MyHandles;
			}
			set
			{
				this.MyHandles = value;
			}
		}
		/// <summary>Gets the absolute in-game time.</summary>
		public Time TotalTime
		{
			get
			{
				return this.MyTotalTime;
			}
		}
		/// <summary>Gets the time that elapsed since the last call to Elapse.</summary>
		public Time ElapsedTime
		{
			get
			{
				return this.MyElapsedTime;
			}
		}
		/// <summary>Gets or sets the debug message the plugin wants the host application to display.</summary>
		public string DebugMessage
		{
			get
			{
				return this.MyDebugMessage;
			}
			set
			{
				this.MyDebugMessage = value;
			}
		}
	}
}