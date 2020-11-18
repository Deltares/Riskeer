// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Plugin.FileImporter;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.Piping.Plugin.UpdateInfos
{
    /// <summary>
    /// Factory for creating <see cref="UpdateInfo"/> objects. 
    /// </summary>
    public static class PipingUpdateInfoFactory
    {
        /// <summary>
        /// Creates a <see cref="UpdateInfo"/> object for a <see cref="PipingFailureMechanismSectionsContext"/>.
        /// </summary>
        /// <param name="verifyUpdatesFunc">The <see cref="Func{T1,TResult}"/> to verify whether changes that are
        /// induced by the importer are allowed.</param>
        /// <returns>An <see cref="UpdateInfo"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="verifyUpdatesFunc"/>
        /// is <c>null</c>.</exception>
        public static UpdateInfo<PipingFailureMechanismSectionsContext> CreateFailureMechanismSectionsUpdateInfo(
            Func<PipingFailureMechanismSectionsContext, bool> verifyUpdatesFunc)
        {
            if (verifyUpdatesFunc == null)
            {
                throw new ArgumentNullException(nameof(verifyUpdatesFunc));
            }

            return new UpdateInfo<PipingFailureMechanismSectionsContext>
            {
                Name = RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.SectionsIcon,
                FileFilterGenerator = new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                                              RiskeerCommonIOResources.Shape_file_filter_Description),
                IsEnabled = context => context.WrappedData.FailureMechanismSectionSourcePath != null,
                CurrentPath = context => context.WrappedData.FailureMechanismSectionSourcePath,
                CreateFileImporter = (context, filePath) => new FailureMechanismSectionsImporter(
                    context.WrappedData,
                    context.AssessmentSection.ReferenceLine,
                    filePath,
                    new PipingFailureMechanismSectionUpdateStrategy((PipingFailureMechanism) context.WrappedData,
                                                                    new PipingFailureMechanismSectionResultUpdateStrategy()),
                    new UpdateMessageProvider()),
                VerifyUpdates = verifyUpdatesFunc
            };
        }
    }
}