﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using Core.Components.Gis.Features;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating collections of <see cref="MapFeature"/> for assembly results in a <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
    /// </summary>
    public static class GrassCoverErosionInwardsAssemblyMapDataFeaturesFactory
    {
        /// <summary>
        /// Creates features for the simple assembly results in <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionInwardsFailureMechanism"/> to create the features for.</param>
        /// <returns>A collection of <see cref="MapFeature"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<MapFeature> CreateSimpleAssemblyFeatures(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return AssemblyMapDataFeaturesFactory.CreateAssemblyFeatures<GrassCoverErosionInwardsFailureMechanism,
                GrassCoverErosionInwardsFailureMechanismSectionResult>(
                failureMechanism,
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSimpleAssessment);
        }

        /// <summary>
        /// Creates features for the detailed assembly results in <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionInwardsFailureMechanism"/> to create the features for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the <paramref name="failureMechanism"/> belongs to.</param>
        /// <returns>A collection of <see cref="MapFeature"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<MapFeature> CreateDetailedAssemblyFeatures(GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                                             IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return AssemblyMapDataFeaturesFactory.CreateAssemblyFeatures<GrassCoverErosionInwardsFailureMechanism,
                GrassCoverErosionInwardsFailureMechanismSectionResult>(
                failureMechanism,
                sectionResult => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection));
        }

        /// <summary>
        /// Creates features for the tailor made assembly results in <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionInwardsFailureMechanism"/> to create the features for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the <paramref name="failureMechanism"/> belongs to.</param>
        /// <returns>A collection of <see cref="MapFeature"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<MapFeature> CreateTailorMadeAssemblyFeatures(GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return AssemblyMapDataFeaturesFactory.CreateAssemblyFeatures<GrassCoverErosionInwardsFailureMechanism,
                GrassCoverErosionInwardsFailureMechanismSectionResult>(
                failureMechanism,
                sectionResult => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(sectionResult,
                                                                                                                      failureMechanism,
                                                                                                                      assessmentSection));
        }

        /// <summary>
        /// Creates features for the combined assembly results in <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionInwardsFailureMechanism"/> to create the features for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the <paramref name="failureMechanism"/> belongs to.</param>
        /// <returns>A collection of <see cref="MapFeature"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<MapFeature> CreateCombinedAssemblyFeatures(GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                                             IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return AssemblyMapDataFeaturesFactory.CreateAssemblyFeatures<GrassCoverErosionInwardsFailureMechanism, GrassCoverErosionInwardsFailureMechanismSectionResult>(
                failureMechanism,
                sectionResult => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection));
        }
    }
}