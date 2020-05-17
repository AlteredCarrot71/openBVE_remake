namespace OpenBveApi.Runtime
{
    /// <summary>Represents AI data.</summary>
    public class AIData
    {
        // --- members ---
        /// <summary>The driver handles.</summary>
        public Handles Handles { get; set; }
        /// <summary>The AI response.</summary>
        public AIResponse Response { get; set; }
        // --- constructors ---
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="handles">The driver handles.</param>
        public AIData(Handles handles)
        {
            this.Handles = handles;
            this.Response = AIResponse.None;
        }
    }
}