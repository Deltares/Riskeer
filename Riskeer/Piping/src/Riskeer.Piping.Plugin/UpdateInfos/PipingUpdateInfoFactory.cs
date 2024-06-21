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
using Core.Gui.Helpers;
using Core.Gui.Plugin;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.UpdateInfos;
using Riskeer.Common.Plugin.FileImporters;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.ChangeHandlers;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Plugin.FileImporter;
using Riskeer.Piping.Plugin.Properties;
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
        /// Creates an <see cref="UpdateInfo"/> object for a <see cref="PipingFailureMechanismSectionsContext"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring required data.</param>
        /// <returns>An <see cref="UpdateInfo"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inquiryHelper"/>
        /// is <c>null</c>.</exception>
        public static UpdateInfo<PipingFailureMechanismSectionsContext> CreateFailureMechanismSectionsUpdateInfo(
            IInquiryHelper inquiryHelper)
        {
            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            UpdateInfo<PipingFailureMechanismSectionsContext> updateInfo = RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                PipingFailureMechanismSectionsContext, PipingFailureMechanism, AdoptableFailureMechanismSectionResult>(
                context => new PipingFailureMechanismSectionUpdateStrategy((PipingFailureMechanism) context.WrappedData,
                                                                           new AdoptableFailureMechanismSectionResultUpdateStrategy()));

            updateInfo.VerifyUpdates = context =>
            {
                var changeHandler = new PipingFailureMechanismCalculationChangeHandler(
                    (PipingFailureMechanism) context.WrappedData,
                    Resources.PipingUpdateInfoFactory_CreateFailureMechanismSectionsUpdateInfo_When_updating_sections_probabilistic_calculation_output_will_be_cleared_confirm,
                    inquiryHelper);
                return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
            };

            return updateInfo;
        }
    }
}