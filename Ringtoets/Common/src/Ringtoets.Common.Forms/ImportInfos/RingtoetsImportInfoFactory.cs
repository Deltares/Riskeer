// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.IO;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.ImportInfos
{
    /// <summary>
    /// Factory for creating standard <see cref="ImportInfo"/> objects. 
    /// </summary>
    public static class RingtoetsImportInfoFactory
    {
        /// <summary>
        /// Creates a <see cref="ImportInfo"/> object for a calculation configuration 
        /// of the type <typeparamref name="TCalculationGroupContext"/>.
        /// </summary>
        /// <typeparam name="TCalculationGroupContext">The type of calculation group context
        /// to create the <see cref="ImportInfo"/> for.</typeparam>
        /// <param name="createFileImporter">The function to create the relevant importer.</param>
        /// <returns>An <see cref="ImportInfo"/> object.</returns>
        public static ImportInfo<TCalculationGroupContext> CreateCalculationConfigurationImportInfo<TCalculationGroupContext>(
            Func<TCalculationGroupContext, string, IFileImporter> createFileImporter)
            where TCalculationGroupContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            return new ImportInfo<TCalculationGroupContext>
            {
                Name = Resources.DataTypeDisplayName_xml_file_filter_Description,
                Category = Resources.Ringtoets_Category,
                Image = Resources.GeneralFolderIcon,
                FileFilterGenerator = new FileFilterGenerator(Resources.DataTypeDisplayName_xml_file_filter_Extension,
                                                              Resources.DataTypeDisplayName_xml_file_filter_Description),
                IsEnabled = context => true,
                CreateFileImporter = createFileImporter
            };
        }
    }
}