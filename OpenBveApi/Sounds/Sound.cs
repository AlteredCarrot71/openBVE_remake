using System;

namespace OpenBveApi.Sounds
{
    /// <summary>Represents a sound.</summary>
    public class Sound
    {
        // --- members ---
        /// <summary>The number of samples per second.</summary>
        public int SampleRate { get; private set; }
        /// <summary>The number of bits per sample. Allowed values are 8 or 16.</summary>
        public int BitsPerSample { get; private set; }
        /// <summary>The PCM sound data per channel. For 8 bits per sample, samples are unsigned from 0 to 255. For 16 bits per sample, samples are signed from -32768 to 32767 and in little endian byte order.</summary>
        public byte[][] Bytes { get; private set; }

        // --- constructors ---
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="sampleRate">The number of samples per second.</param>
        /// <param name="bitsPerSample">The number of bits per sample. Allowed values are 8 or 16.</param>
        /// <param name="bytes">The PCM sound data per channel. For 8 bits per sample, samples are unsigned from 0 to 255. For 16 bits per sample, samples are signed from -32768 to 32767 and in little endian byte order.</param>
        /// <exception cref="System.ArgumentException">Raised when the number of samples per second is not positive.</exception>
        /// <exception cref="System.ArgumentException">Raised when the number of bits per samples is neither 8 nor 16.</exception>
        /// <exception cref="System.ArgumentNullException">Raised when the bytes array or any of its subarrays is a null reference.</exception>
        /// <exception cref="System.ArgumentException">Raised when the bytes array does not contain any elements.</exception>
        /// <exception cref="System.ArgumentException">Raised when the bytes' subarrays are of unequal length.</exception>
        public Sound(int sampleRate, int bitsPerSample, byte[][] bytes)
        {
            if (sampleRate <= 0)
            {
                throw new ArgumentException("The sample rate must be positive.");
            }
            if (bitsPerSample != 8 & bitsPerSample != 16)
            {
                throw new ArgumentException("The number of bits per sample is neither 8 nor 16.");
            }
            if (bytes == null)
            {
                throw new ArgumentNullException("The data bytes are a null reference.");
            }
            if (bytes.Length == 0)
            {
                throw new ArgumentException("There must be at least one channel.");
            }
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == null)
                {
                    throw new ArgumentNullException("The data bytes channel " + i.ToString() + " are a null reference.");
                }
            }
            for (int i = 1; i < bytes.Length; i++)
            {
                if (bytes[i].Length != bytes[0].Length)
                {
                    throw new ArgumentException("The data bytes of the channels are of unequal length.");
                }
            }
            this.SampleRate = sampleRate;
            this.BitsPerSample = bitsPerSample;
            this.Bytes = bytes;
        }

        // --- properties ---
        /// <summary>Gets the duration of the sound in seconds.</summary>
        public double Duration
        {
            get
            {
                return (double)(8 * this.Bytes[0].Length / this.BitsPerSample) / (double)this.SampleRate;
            }
        }
        // --- operators ---
        /// <summary>Checks whether two sound are equal.</summary>
        /// <param name="a">The first sound.</param>
        /// <param name="b">The second sound.</param>
        /// <returns>Whether the two sounds are equal.</returns>
        public static bool operator ==(Sound a, Sound b)
        {
            if (object.ReferenceEquals(a, b)) return true;
            if (a is null) return false;
            if (b is null) return false;
            if (a.SampleRate != b.SampleRate) return false;
            if (a.BitsPerSample != b.BitsPerSample) return false;
            if (a.Bytes.Length != b.Bytes.Length) return false;
            for (int i = 0; i < a.Bytes.Length; i++)
            {
                if (a.Bytes[i].Length != b.Bytes[i].Length) return false;
                for (int j = 0; j < a.Bytes[i].Length; j++)
                {
                    if (a.Bytes[i][j] != b.Bytes[i][j]) return false;
                }
            }
            return true;
        }
        /// <summary>Checks whether two sounds are unequal.</summary>
        /// <param name="a">The first sound.</param>
        /// <param name="b">The second sound.</param>
        /// <returns>Whether the two sounds are unequal.</returns>
        public static bool operator !=(Sound a, Sound b)
        {
            if (object.ReferenceEquals(a, b)) return false;
            if (a is null) return true;
            if (b is null) return true;
            if (a.SampleRate != b.SampleRate) return true;
            if (a.BitsPerSample != b.BitsPerSample) return true;
            if (a.Bytes.Length != b.Bytes.Length) return true;
            for (int i = 0; i < a.Bytes.Length; i++)
            {
                if (a.Bytes[i].Length != b.Bytes[i].Length) return true;
                for (int j = 0; j < a.Bytes[i].Length; j++)
                {
                    if (a.Bytes[i][j] != b.Bytes[i][j]) return true;
                }
            }
            return false;
        }
        /// <summary>Checks whether this instance is equal to the specified object.</summary>
        /// <param name="obj">The object.</param>
        /// <returns>Whether this instance is equal to the specified object.</returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj)) return true;
            if (this is null) return false;
            if (obj is null) return false;
            if (!(obj is Sound)) return false;
            Sound x = (Sound)obj;
            if (this.SampleRate != x.SampleRate) return false;
            if (this.BitsPerSample != x.BitsPerSample) return false;
            if (this.Bytes.Length != x.Bytes.Length) return false;
            for (int i = 0; i < this.Bytes.Length; i++)
            {
                if (this.Bytes[i].Length != x.Bytes[i].Length) return false;
                for (int j = 0; j < this.Bytes[i].Length; j++)
                {
                    if (this.Bytes[i][j] != x.Bytes[i][j]) return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}