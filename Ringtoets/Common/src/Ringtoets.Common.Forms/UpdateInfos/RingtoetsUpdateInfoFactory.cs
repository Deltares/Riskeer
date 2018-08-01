// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Common.Forms.UpdateInfos
{
    /// <summary>
    /// Factory for creating standard <see cref="UpdateInfo"/> objects. 
    /// </summary>
    public static class RingtoetsUpdateInfoFactory
    {
        /// <summary>
        /// Creates a <see cref="UpdateInfo"/> object for a <see cref="TSectionContext"/>.
        /// </summary>
        /// <typeparam name="TSectionContext">The type of the failure mechanism sections context
        /// to create the <see cref="UpdateInfo"/> for.</typeparam>
        /// <typeparam name="TFailureMechanism">The type of the failure mechanism to create
        /// the <see cref="UpdateInfo"/> for.</typeparam>
        /// <typeparam name="TSectionResult">The type of the failure mechanism section result
        /// to create the <see cref="UpdateInfo"/> for.</typeparam>
        /// <param name="sectionResultUpdateStrategy">The <see cref="IFailureMechanismSectionResultUpdateStrategy{T}"/>
        /// to use for the created <see cref="UpdateInfo"/>.</param>
        /// <returns>An <see cref="UpdateInfo"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResultUpdateStrategy"/>
        /// is <c>null</c>.</exception>
        public static UpdateInfo<TSectionContext> CreateFailureMechanismSectionsUpdateInfo<TSectionContext, TFailureMechanism, TSectionResult>(
            IFailureMechanismSectionResultUpdateStrategy<TSectionResult> sectionResultUpdateStrategy)
            where TSectionContext : FailureMechanismSectionsContext
            where TFailureMechanism : IHasSectionResults<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            if (sectionResultUpdateStrategy == null)
            {
                throw new ArgumentNullException(nameof(sectionResultUpdateStrategy));
            }

            return new UpdateInfo<TSectionContext>
            {
                Name = Resources.FailureMechanismSections_DisplayName,
                Category = Resources.Ringtoets_Category,
                Image = Resources.SectionsIcon,
                FileFilterGenerator = new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                                              RingtoetsCommonIOResources.Shape_file_filter_Description),
                IsEnabled = context => context.WrappedData.FailureMechanismSectionSourcePath != null,
                CurrentPath = context => context.WrappedData.FailureMechanismSectionSourcePath,
                CreateFileImporter = (context, filePath) => new FailureMechanismSectionsImporter(
                    context.WrappedData,
                    context.AssessmentSection.ReferenceLine,
                    filePath,
                    new FailureMechanismSectionUpdateStrategy<TSectionResult>((TFailureMechanism) context.WrappedData,
                                                                              sectionResultUpdateStrategy),
                    new UpdateMessageProvider())
            };
        }
    }
}