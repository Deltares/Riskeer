using System.Collections.Generic;
using Core.Common.Base;

using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Piping.Plugin
{
    /// <summary>
    /// The application plugin for the piping failure mechanism.
    /// </summary>
    public class PipingApplicationPlugin : ApplicationPlugin
    {
        public override IEnumerable<IFileImporter> GetFileImporters()
        {
            yield return new PipingSurfaceLinesCsvImporter();
            yield return new PipingSoilProfilesImporter();
        }
    }
}