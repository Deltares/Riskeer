namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for unique IDs.
    /// </summary>
    internal class IdFactory
    {
        private int lastId;

        /// <summary>
        /// Creates a new unique ID.
        /// </summary>
        /// <returns>The created ID.</returns>
        public string Create()
        {
            return lastId++.ToString();
        }
    }
}