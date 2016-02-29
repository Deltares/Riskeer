using System;
using System.Drawing;

using Core.Common.Base.IO;

using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO;

using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Integration.Plugin.FileImporters
{
    public class FailureMechanismSectionsImporter : FileImporterBase
    {
        public override string Name
        {
            get
            {
                return RingtoetsCommonDataResources.FailureMechanism_Sections_DisplayName;
            }
        }

        public override string Category
        {
            get
            {
                return RingtoetsCommonFormsResources.Ringtoets_Category;
            }
        }

        public override Bitmap Image
        {
            get
            {
                return RingtoetsCommonFormsResources.Sections;
            }
        }

        public override Type SupportedItemType
        {
            get
            {
                return typeof(FailureMechanismSectionsContext);
            }
        }

        public override string FileFilter
        {
            get
            {
                return string.Format("{0} shapefile (*.shp)|*.shp", Name);
            }
        }

        public override ProgressChangedDelegate ProgressChanged { protected get; set; }

        public override bool Import(object targetItem, string filePath)
        {
            var context = (FailureMechanismSectionsContext)targetItem;
            using (var reader = new FailureMechanismSectionReader(filePath))
            {
                var count = reader.GetFailureMechanismSectionCount();
                for (int i = 0; i < count; i++)
                {
                    var section = reader.ReadFailureMechanismSection();
                    context.ParentFailureMechanism.AddSection(section);
                }
            }
            return true;
        }
    }
}