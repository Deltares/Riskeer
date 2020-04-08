using Shared.Components.Persistence;

namespace Riskeer.MacroStabilityInwards.IO.TestUtil
{
    /// <summary>
    /// Persister that can be used in tests.
    /// </summary>
    public class MacroStabilityInwardsTestPersister : IPersister
    {
        /// <summary>
        /// Gets whether persist is called.
        /// </summary>
        public bool PersistCalled { get; private set; }

        public void Persist()
        {
            PersistCalled = true;
        }

        public void Dispose() {}
    }
}