﻿namespace OpenBveApi.Runtime
{
    /// <summary>Represents a time.</summary>
    public class Time
    {
        // --- members ---
        /// <summary>The time in seconds.</summary>
        private readonly double Second;

        // --- properties ---
        /// <summary>Gets the time in seconds.</summary>
        public double Seconds
        {
            get
            {
                return this.Second;
            }
        }
        /// <summary>Gets the time in milliseconds.</summary>
        public double Milliseconds
        {
            get
            {
                return 1000.0 * this.Second;
            }
        }

        // --- constructors ---
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="value">The time in seconds.</param>
        public Time(double value)
        {
            this.Second = value;
        }
    }
}