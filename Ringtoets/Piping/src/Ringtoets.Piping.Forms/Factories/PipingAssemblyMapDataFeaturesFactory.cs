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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Util;
using Core.Components.Gis.Features;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Piping.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Factories
{
    /// <summary>
    /// Factory for creating collections of <see cref="MapFeature"/> for assembly results.
    /// </summary>
    public static class PipingAssemblyMapDataFeaturesFactory
    {
        /// <summary>
        /// Creates features for the simple assembly results in <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> to create the features for.</param>
        /// <returns>A collection of <see cref="MapFeature"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when a <see cref="MapFeature"/> could not be created.</exception>
        public static IEnumerable<MapFeature> CreateSimpleAssemblyFeatures(PipingFailureMechanism failureMechanism)
        {
            return CreateAssemblyFeatures(failureMechanism,
                                          PipingFailureMechanismAssemblyFactory.AssembleSimpleAssessment).ToArray();
        }

        /// <summary>
        /// Creates features for the detailed assembly results in <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> to create the features for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the <paramref name="failureMechanism"/> belongs to.</param>
        /// <returns>A collection of <see cref="MapFeature"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when a <see cref="MapFeature"/> could not be created.</exception>
        public static IEnumerable<MapFeature> CreateDetailedAssemblyFeatures(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return CreateAssemblyFeatures(failureMechanism,
                                          sectionResult => PipingFailureMechanismAssemblyFactory.AssembleDetailedAssessment(sectionResult,
                                                                                                                            failureMechanism.Calculations.Cast<PipingCalculationScenario>(),
                                                                                                                            failureMechanism,
                                                                                                                            assessmentSection)).ToArray();
        }

        /// <summary>
        /// Creates features for the tailor made assembly results in <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> to create the features for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the <paramref name="failureMechanism"/> belongs to.</param>
        /// <returns>A collection of <see cref="MapFeature"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when a <see cref="MapFeature"/> could not be created.</exception>
        public static IEnumerable<MapFeature> CreateTailorMadeAssemblyFeatures(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return CreateAssemblyFeatures(failureMechanism,
                                          sectionResult => PipingFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(sectionResult,
                                                                                                                              failureMechanism,
                                                                                                                              assessmentSection)).ToArray();
        }

        private static IEnumerable<MapFeature> CreateAssemblyFeatures(PipingFailureMechanism failureMechanism,
                                                                      Func<PipingFailureMechanismSectionResult, FailureMechanismSectionAssembly> getAssemblyFunc)
        {
            for (var i = 0; i < failureMechanism.SectionResults.Count(); i++)
            {
                PipingFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.ElementAt(i);
                MapFeature feature = RingtoetsMapDataFeaturesFactory.CreateSingleLineMapFeature(sectionResult.Section.Points);
                FailureMechanismSectionAssembly assemblyResult = getAssemblyFunc(sectionResult);

                feature.MetaData[RingtoetsCommonFormsResources.AssemblyCategory_Group_DisplayName] = 
                    new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyCategoryGroup>(
                        DisplayFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(assemblyResult.Group)).DisplayName;

                feature.MetaData[RingtoetsCommonFormsResources.MetaData_Probability] = assemblyResult.Probability;

                yield return feature;
            }
        }
    }
}