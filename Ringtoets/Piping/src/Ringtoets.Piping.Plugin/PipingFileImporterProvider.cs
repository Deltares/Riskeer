using System.Collections.Generic;

using Core.Common.Base;
using Core.Common.Base.IO;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Piping.Plugin
{
    /// <summary>
    /// Class providing all file imported related to piping classes.
    /// </summary>
    public static class PipingFileImporterProvider
    {
        /// <summary>
        /// Gets the file importers available for Piping related data.
        /// </summary>
        public static IEnumerable<IFileImporter> GetFileImporters()
        {
            yield return new PipingSurfaceLinesCsvImporter();
            yield return new PipingSoilProfilesImporter();
        }
    }
}