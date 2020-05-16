namespace OpenBveApi.Sounds
{
	/// <summary>Represents the interface for loading sounds. Plugins must implement this interface if they wish to expose sounds.</summary>
	public abstract class SoundInterface
	{
		/// <summary>Called when the plugin is loaded.</summary>
		/// <param name="host">The host that loaded the plugin.</param>
		public virtual void Load(Hosts.HostInterface host) { }

		/// <summary>Called when the plugin is unloaded.</summary>
		public virtual void Unload() { }

		/// <summary>Checks whether the plugin can load the specified sound.</summary>
		/// <param name="path">The path to the file or folder that contains the sound.</param>
		/// <returns>Whether the plugin can load the specified sound.</returns>
		public abstract bool CanLoadSound(Path.PathReference path);

		/// <summary>Loads the specified sound.</summary>
		/// <param name="path">The path to the file or folder that contains the sound.</param>
		/// <param name="sound">Receives the sound.</param>
		/// <returns>Whether loading the sound was successful.</returns>
		public abstract bool LoadSound(Path.PathReference path, out Sound sound);
	}
}
