using System;
using System.Drawing;

using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;

using log4net;

using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO;
using Ringtoets.Integration.Plugin.Properties;

using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Integration.Plugin.FileImporters
{
    /// <summary>
    /// Imports <see cref="FailureMechanismSection"/> instances from a shapefile that contains
    /// one or more polylines and stores them in a <see cref="IFailureMechanism"/>.
    /// </summary>
    public class FailureMechanismSectionsImporter : FileImporterBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FailureMechanismSectionsImporter));

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
            ReadResult<FailureMechanismSection> readResults = ReadFailureMechanismSections(filePath);

            if (readResults.CriticalErrorOccurred)
            {
                return false;
            }

            AddImportedDataToModel(targetItem, readResults);
            return true;
        }

        private ReadResult<FailureMechanismSection> ReadFailureMechanismSections(string filePath)
        {
            using (FailureMechanismSectionReader reader = CreateFileReader(filePath))
            {
                if (reader == null)
                {
                    return new ReadResult<FailureMechanismSection>(true);
                }

                return ReadFile(reader);
            }
        }

        private FailureMechanismSectionReader CreateFileReader(string filePath)
        {
            try
            {
                return new FailureMechanismSectionReader(filePath);
            }
            catch (ArgumentException e)
            {
                LogCriticalFileReadError(e);
            }
            catch (CriticalFileReadException e)
            {
                LogCriticalFileReadError(e);
            }
            return null;
        }

        private ReadResult<FailureMechanismSection> ReadFile(FailureMechanismSectionReader reader)
        {
            try
            {
                var count = reader.GetFailureMechanismSectionCount();

                var importedSections = new FailureMechanismSection[count];
                for (int i = 0; i < count; i++)
                {
                    importedSections[i] = reader.ReadFailureMechanismSection();
                }

                return new ReadResult<FailureMechanismSection>(false)
                {
                    ImportedItems = importedSections
                };
            }
            catch (CriticalFileReadException e)
            {
                LogCriticalFileReadError(e);
                return new ReadResult<FailureMechanismSection>(true);
            }
        }

        private void LogCriticalFileReadError(Exception exception)
        {
            var errorMessage = String.Format(Resources.FailureMechanismSectionsImporter_CriticalErrorMessage_0_No_sections_imported,
                                             exception.Message);
            log.Error(errorMessage);
        }

        private void AddImportedDataToModel(object targetItem, ReadResult<FailureMechanismSection> readResults)
        {
            var context = (FailureMechanismSectionsContext)targetItem;
            foreach (FailureMechanismSection section in readResults.ImportedItems)
            {
                context.ParentFailureMechanism.AddSection(section);
            }
        }
    }
}