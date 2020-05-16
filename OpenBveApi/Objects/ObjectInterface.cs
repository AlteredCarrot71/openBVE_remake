using System;

namespace OpenBveApi.Objects
{
	/// <summary>Represents the interface for loading objects. Plugins must implement this interface if they wish to expose objects.</summary>
	public abstract class ObjectInterface
	{
		/// <summary>Called when the plugin is loaded.</summary>
		/// <param name="host">The host that loaded the plugin.</param>
		public virtual void Load(Hosts.HostInterface host) { }

		/// <summary>Called when the plugin is unloaded.</summary>
		public virtual void Unload() { }

		/// <summary>Checks whether the plugin can load the specified object.</summary>
		/// <param name="path">The path to the file or folder that contains the object.</param>
		/// <returns>Whether the plugin can load the specified object.</returns>
		public abstract bool CanLoadObject(Path.PathReference path);

		/// <summary>Loads the specified object.</summary>
		/// <param name="path">The path to the file or folder that contains the object.</param>
		/// <param name="obj">Receives the object.</param>
		/// <returns>Whether loading the object was successful.</returns>
		public abstract bool LoadObject(Path.PathReference path, out Object obj);
	}
}