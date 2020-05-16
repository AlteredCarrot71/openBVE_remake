namespace OpenBveApi.Runtime
{
	/// <summary>Represents properties supplied to the plugin on loading.</summary>
	public class LoadProperties
	{
		// --- members ---
		/// <summary>The absolute path to the plugin folder.</summary>
		private readonly string MyPluginFolder;
		/// <summary>The absolute path to the train folder.</summary>
		private readonly string MyTrainFolder;
		/// <summary>The array of panel variables.</summary>
		private int[] MyPanel;
		/// <summary>The callback function for playing sounds.</summary>
		/// <exception cref="System.InvalidOperationException">Raised when the host application does not allow the function to be called.</exception>
		private readonly PlaySoundDelegate MyPlaySound;
		/// <summary>The extent to which the plugin supports the AI.</summary>
		private AISupport MyAISupport;
		/// <summary>The reason why the plugin failed loading.</summary>
		private string MyFailureReason;
		// --- properties ---
		/// <summary>Gets the absolute path to the plugin folder.</summary>
		public string PluginFolder
		{
			get
			{
				return this.MyPluginFolder;
			}
		}
		/// <summary>Gets the absolute path to the train folder.</summary>
		public string TrainFolder
		{
			get
			{
				return this.MyTrainFolder;
			}
		}
		/// <summary>Gets or sets the array of panel variables.</summary>
		public int[] Panel
		{
			get
			{
				return this.MyPanel;
			}
			set
			{
				this.MyPanel = value;
			}
		}
		/// <summary>Gets the callback function for playing sounds.</summary>
		public PlaySoundDelegate PlaySound
		{
			get
			{
				return this.MyPlaySound;
			}
		}
		/// <summary>Gets or sets the extent to which the plugin supports the AI.</summary>
		public AISupport AISupport
		{
			get
			{
				return this.MyAISupport;
			}
			set
			{
				this.MyAISupport = value;
			}
		}
		/// <summary>Gets or sets the reason why the plugin failed loading.</summary>
		public string FailureReason
		{
			get
			{
				return this.MyFailureReason;
			}
			set
			{
				this.MyFailureReason = value;
			}
		}
		// --- constructors ---
		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="pluginFolder">The absolute path to the plugin folder.</param>
		/// <param name="trainFolder">The absolute path to the train folder.</param>
		/// <param name="playSound">The callback function for playing sounds.</param>
		public LoadProperties(string pluginFolder, string trainFolder, PlaySoundDelegate playSound)
		{
			this.MyPluginFolder = pluginFolder;
			this.MyTrainFolder = trainFolder;
			this.MyPlaySound = playSound;
			this.MyFailureReason = null;
		}
	}
}