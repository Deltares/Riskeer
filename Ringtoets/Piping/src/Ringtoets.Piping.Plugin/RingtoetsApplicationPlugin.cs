using System.Collections.Generic;
using Core.Common.Base;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;

using RingtoetsFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin
{
    public class RingtoetsApplicationPlugin : ApplicationPlugin
    {
        public override IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            yield return new DataItemInfo<RingtoetsProject>
            {
                Name = RingtoetsFormsResources.RingtoetsProjectProperties_DisplayName,
                Category = RingtoetsFormsResources.RingtoetsProjectProperties_Category,
                Image = RingtoetsFormsResources.RingtoetsProjectFolderIcon,
                CreateData = owner => new RingtoetsProject()
            };
        }

        public override IEnumerable<IFileImporter> GetFileImporters()
        {
            yield return new PipingSurfaceLinesCsvImporter();
            yield return new PipingSoilProfilesImporter();
        }
    }
}