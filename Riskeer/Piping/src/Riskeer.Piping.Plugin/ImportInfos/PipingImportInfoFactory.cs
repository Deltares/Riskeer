// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Gui.Helpers;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.ChangeHandlers;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Plugin.FileImporter;
using Riskeer.Piping.Plugin.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.Piping.Plugin.ImportInfos
{
    /// <summary>
    /// Factory for creating standard <see cref="ImportInfo"/> objects. 
    /// </summary>
    public static class PipingImportInfoFactory
    {
        /// <summary>
        /// Creates a <see cref="ImportInfo"/> object for a <see cref="PipingFailureMechanismSectionsContext"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring required data.</param>
        /// <returns>An <see cref="ImportInfo"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inquiryHelper"/>
        /// is <c>null</c>.</exception>
        public static ImportInfo<PipingFailureMechanismSectionsContext> CreateFailureMechanismSectionsImportInfo(
            IInquiryHelper inquiryHelper)
        {
            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            return new ImportInfo<PipingFailureMechanismSectionsContext>
            {
                Name = RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.SectionsIcon,
                FileFilterGenerator = new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                                              RiskeerCommonIOResources.Shape_file_filter_Description),
                IsEnabled = context => context.AssessmentSection.ReferenceLine.Points.Any(),
                CreateFileImporter = (context, filePath) => new FailureMechanismSectionsImporter(
                    context.WrappedData, context.AssessmentSection.ReferenceLine,
                    filePath, new PipingFailureMechanismSectionReplaceStrategy((PipingFailureMechanism) context.WrappedData),
                    new ImportMessageProvider()),
                VerifyUpdates = context =>
                {
                    var changeHandler = new PipingFailureMechanismCalculationChangeHandler(
                        (PipingFailureMechanism) context.WrappedData,
                        Resources.PipingImportInfoFactory_CreateFailureMechanismSectionsImportInfo_When_importing_sections_probabilistic_calculation_output_will_be_cleared_confirm,
                        inquiryHelper);
                    return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
                }
            };
        }
    }
}