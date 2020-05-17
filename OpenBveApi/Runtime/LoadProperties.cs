namespace OpenBveApi.Runtime
{
    /// <summary>Represents properties supplied to the plugin on loading.</summary>
    public class LoadProperties
    {
        // --- members ---
        /// <summary>The absolute path to the plugin folder.</summary>
        public string PluginFolder { get; private set; }
        /// <summary>The absolute path to the train folder.</summary>
        public string TrainFolder { get; private set; }
        /// <summary>The array of panel variables.</summary>
        public int[] Panel { get; set; }
        /// <summary>The callback function for playing sounds.</summary>
        /// <exception cref="System.InvalidOperationException">Raised when the host application does not allow the function to be called.</exception>
        public PlaySoundDelegate PlaySound { get; private set; }
        /// <summary>The extent to which the plugin supports the AI.</summary>
        public AISupport AISupport { get; set; }
        /// <summary>The reason why the plugin failed loading.</summary>
        public string FailureReason { get; set; }

        // --- constructors ---
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="pluginFolder">The absolute path to the plugin folder.</param>
        /// <param name="trainFolder">The absolute path to the train folder.</param>
        /// <param name="playSound">The callback function for playing sounds.</param>
        public LoadProperties(string pluginFolder, string trainFolder, PlaySoundDelegate playSound)
        {
            this.PluginFolder = pluginFolder;
            this.TrainFolder = trainFolder;
            this.PlaySound = playSound;
            this.FailureReason = null;
        }
    }
}