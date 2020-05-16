namespace OpenBveApi.Runtime
{
	/// <summary>Represents the handle to a sound.</summary>
	public class SoundHandle
	{
		// --- members ---
		/// <summary>Whether the handle to the sound is valid.</summary>
		protected bool MyValid;
		/// <summary>The volume. A value of 1.0 represents nominal volume.</summary>
		protected double MyVolume;
		/// <summary>The pitch. A value of 1.0 represents nominal pitch.</summary>
		protected double MyPitch;
		// --- properties ---
		/// <summary>Gets whether the sound is still playing. Once this returns false, the sound handle is invalid.</summary>
		public readonly bool Playing;
		/// <summary>Gets whether the sound has stopped. Once this returns true, the sound handle is invalid.</summary>
		public bool Stopped
		{
			get
			{
				return !this.MyValid;
			}
		}
		/// <summary>Gets or sets the volume. A value of 1.0 represents nominal volume.</summary>
		public double Volume;
		/// <summary>Gets or sets the pitch. A value of 1.0 represents nominal pitch.</summary>
		public double Pitch;
		// functions
		/// <summary>Stops the sound and invalidates the handle.</summary>
		public void Stop()
		{
			this.MyValid = false;
		}
	}

	/// <summary>Plays a sound.</summary>
	/// <param name="index">The index to the sound to be played.</param>
	/// <param name="volume">The initial volume of the sound. A value of 1.0 represents nominal volume.</param>
	/// <param name="pitch">The initial pitch of the sound. A value of 1.0 represents nominal pitch.</param>
	/// <param name="looped">Whether the sound should be played in an indefinate loop.</param>
	/// <returns>The handle to the sound, or a null reference if the sound could not be played.</returns>
	/// <exception cref="System.InvalidOperationException">Raised when the host application does not allow the function to be called.</exception>
	public delegate SoundHandle PlaySoundDelegate(int index, double volume, double pitch, bool looped);
}