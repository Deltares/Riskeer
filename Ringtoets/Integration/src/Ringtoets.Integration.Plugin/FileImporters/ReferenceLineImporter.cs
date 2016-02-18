// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;

using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;

using log4net;

using Ringtoets.Common.Data;
using Ringtoets.Common.IO;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin.Properties;

using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.FileImporters
{
    /// <summary>
    /// Imports a <see cref="ReferenceLine"/> and stores in on a <see cref="AssessmentSectionBase"/>,
    /// taking data from a shapefile containing a single polyline.
    /// </summary>
    public class ReferenceLineImporter : IFileImporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReferenceLineImporter));

        public string Name
        {
            get
            {
                return RingtoetsIntegrationFormsResources.ReferenceLine_DisplayName;
            }
        }

        public string Category
        {
            get
            {
                return RingtoetsFormsResources.Ringtoets_Category;
            }
        }

        public Bitmap Image
        {
            get
            {
                return RingtoetsIntegrationFormsResources.ReferenceLineIcon;
            }
        }

        public Type SupportedItemType
        {
            get
            {
                return typeof(ReferenceLineContext);
            }
        }

        public string FileFilter
        {
            get
            {
                return String.Format("{0} shapefile (*.shp)|*.shp",
                                     RingtoetsIntegrationFormsResources.ReferenceLine_DisplayName);
            }
        }

        public ProgressChangedDelegate ProgressChanged { get; set; }

        public bool Import(object targetItem, string filePath)
        {
            try
            {
                var importTarget = (ReferenceLineContext)targetItem;
                var importedReferenceLine = new ReferenceLineReader().ReadReferenceLine(filePath);
                importTarget.Parent.ReferenceLine = importedReferenceLine;
                return true;
            }
            catch (ArgumentException e)
            {
                HandleCriticalFileReadError(e);
                return false;
            }
            catch (CriticalFileReadException e)
            {
                HandleCriticalFileReadError(e);
                return false;
            }
        }

        public void Cancel() {}

        private static void HandleCriticalFileReadError(Exception e)
        {
            var errorMessage = String.Format(Resources.ReferenceLineImporter_HandleCriticalFileReadError_Error_0_no_referenceline_imported,
                                             e.Message);
            log.Error(errorMessage);
        }
    }
}