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
using System.Collections.Generic;
using System.Linq;
using Core.Components.Gis.Features;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="MapFeature"/> instances for assembly data.
    /// </summary>
    public static class AssemblyMapDataFeaturesFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="MapFeature"/> for instances of <see cref="FailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <typeparam name="TSectionResult">The type of section result to create the features for.</typeparam>
        /// <param name="failureMechanism">The failure mechanism to create the features for.</param>
        /// <param name="performAssemblyFunc">The <see cref="Func{T,T2}"/> used to assemble the result of a section result.</param>
        /// <returns>A collection of <see cref="MapFeature"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<MapFeature> CreateAssemblyGroupFeatures<TSectionResult>(
            IHasSectionResults<TSectionResult> failureMechanism,
            Func<TSectionResult, FailureMechanismSectionAssemblyResult> performAssemblyFunc)
            where TSectionResult : FailureMechanismSectionResult
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (performAssemblyFunc == null)
            {
                throw new ArgumentNullException(nameof(performAssemblyFunc));
            }

            return CreateAssemblyGroupFeatures(failureMechanism.SectionResults, performAssemblyFunc).ToArray();
        }

        private static IEnumerable<MapFeature> CreateAssemblyGroupFeatures<TSectionResult>(
            IEnumerable<TSectionResult> sectionResults, Func<TSectionResult, FailureMechanismSectionAssemblyResult> performAssemblyFunc)
            where TSectionResult : FailureMechanismSectionResult
        {
            foreach (TSectionResult sectionResult in sectionResults)
            {
                MapFeature feature = RiskeerMapDataFeaturesFactory.CreateSingleLineMapFeature(sectionResult.Section.Points);

                FailureMechanismSectionAssemblyResult assemblyResult;
                try
                {
                    assemblyResult = performAssemblyFunc(sectionResult);
                }
                catch (AssemblyException)
                {
                    continue;
                }

                feature.MetaData[Resources.AssemblyGroup_DisplayName] =
                    FailureMechanismSectionAssemblyGroupDisplayHelper.GetAssemblyGroupDisplayName(assemblyResult.AssemblyGroup);
                feature.MetaData[Resources.AssemblyMapDataFeaturesFactory_ProbabilityPerSection_DisplayName] = assemblyResult.SectionProbability;

                yield return feature;
            }
        }
    }
}