using System;
using Components.Persistence.Stability;
using Components.Persistence.Stability.Data;
using Shared.Components.Persistence;

namespace Riskeer.MacroStabilityInwards.IO.TestUtil
{
    /// <summary>
    /// Persistence factory that can be used in tests.
    /// </summary>
    public class MacroStabilityInwardsTestPersistenceFactory : IPersistenceFactory
    {
        /// <summary>
        /// Gets the <see cref="PersistableDataModel"/>.
        /// </summary>
        public PersistableDataModel PersistableDataModel { get; private set; }

        /// <summary>
        /// Gets the FilePath.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets or sets whether an exception should be thrown.
        /// </summary>
        public bool ThrowException { get; set; }

        public Persister CreateArchivePersister(string path, PersistableDataModel dataModel)
        {
            if (ThrowException)
            {
                throw new Exception("Exception in persistor.");
            }

            FilePath = path;
            PersistableDataModel = dataModel;

            return null;
        }

        public Reader<PersistableDataModel> CreateArchiveReader(string path)
        {
            throw new NotImplementedException();
        }
    }
}