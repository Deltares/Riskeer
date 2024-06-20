// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Util;
using Core.Gui.Plugin;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.Common.Plugin.ImportInfos
{
    /// <summary>
    /// Factory for creating standard <see cref="ImportInfo"/> objects. 
    /// </summary>
    public static class RiskeerImportInfoFactory
    {
        /// <summary>
        /// Creates an <see cref="ImportInfo"/> object for a calculation configuration 
        /// of the type <typeparamref name="TCalculationGroupContext"/>.
        /// </summary>
        /// <typeparam name="TCalculationGroupContext">The type of calculation group context
        /// to create the <see cref="ImportInfo"/> for.</typeparam>
        /// <param name="createFileImporterFunc">The function to create the relevant importer.</param>
        /// <returns>An <see cref="ImportInfo"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="createFileImporterFunc"/> is <c>null</c>.</exception>
        public static ImportInfo<TCalculationGroupContext> CreateCalculationConfigurationImportInfo<TCalculationGroupContext>(
            Func<TCalculationGroupContext, string, IFileImporter> createFileImporterFunc)
            where TCalculationGroupContext : ICalculationContext<CalculationGroup, ICalculatableFailureMechanism>
        {
            if (createFileImporterFunc == null)
            {
                throw new ArgumentNullException(nameof(createFileImporterFunc));
            }

            return new ImportInfo<TCalculationGroupContext>
            {
                Name = RiskeerCommonFormsResources.DataTypeDisplayName_xml_file_filter_Description,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.GeneralFolderIcon,
                FileFilterGenerator = new FileFilterGenerator(RiskeerCommonFormsResources.DataTypeDisplayName_xml_file_filter_Extension,
                                                              RiskeerCommonFormsResources.DataTypeDisplayName_xml_file_filter_Description),
                IsEnabled = context => true,
                CreateFileImporter = createFileImporterFunc
            };
        }

        /// <summary>
        /// Creates an <see cref="ImportInfo"/> object for a <see cref="TSectionContext"/>.
        /// </summary>
        /// <typeparam name="TSectionContext">The type of the failure mechanism sections context
        /// to create the <see cref="ImportInfo"/> for.</typeparam>
        /// <typeparam name="TFailureMechanism">The type of the failure mechanism to create
        /// the <see cref="ImportInfo"/> for.</typeparam>
        /// <param name="createSectionReplaceStrategyFunc">The function to get the <see cref="FailureMechanismSectionReplaceStrategy"/>
        /// for the created <see cref="ImportInfo"/>.</param>
        /// <returns>An <see cref="ImportInfo"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="createSectionReplaceStrategyFunc"/>
        /// is <c>null</c>.</exception>
        public static ImportInfo<TSectionContext> CreateFailureMechanismSectionsImportInfo<TSectionContext>(
            Func<TSectionContext, FailureMechanismSectionReplaceStrategy> createSectionReplaceStrategyFunc)
            where TSectionContext : FailureMechanismSectionsContext
        {
            if (createSectionReplaceStrategyFunc == null)
            {
                throw new ArgumentNullException(nameof(createSectionReplaceStrategyFunc));
            }

            return new ImportInfo<TSectionContext>
            {
                Name = RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.SectionsIcon,
                FileFilterGenerator = new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                                              RiskeerCommonIOResources.Shape_file_filter_Description),
                IsEnabled = context => context.AssessmentSection.ReferenceLine.Points.Any(),
                CreateFileImporter = (context, filePath) => new FailureMechanismSectionsImporter(
                    context.WrappedData, context.AssessmentSection.ReferenceLine,
                    filePath,
                    createSectionReplaceStrategyFunc(context),
                    new ImportMessageProvider())
            };
        }
    }
}