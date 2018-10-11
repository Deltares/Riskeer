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
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="MapFeature"/> instances for assembly data.
    /// </summary>
    public static class AssemblyMapDataFeaturesFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="MapFeature"/> for instances of <see cref="FailureMechanismSectionAssembly"/>.
        /// </summary>
        /// <typeparam name="TFailureMechanism">The type of failure mechanism to create the features for.</typeparam>
        /// <typeparam name="TSectionResult">The type of section result to create the features for.</typeparam>
        /// <param name="failureMechanism">The failure mechanism to create the features for.</param>
        /// <param name="getAssemblyFunc">The <see cref="Func{T,T2}"/> used to assemble the result of a section result.</param>
        /// <returns>A collection of <see cref="MapFeature"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<MapFeature> CreateAssemblyFeatures<TFailureMechanism, TSectionResult>(
            TFailureMechanism failureMechanism, Func<TSectionResult, FailureMechanismSectionAssembly> getAssemblyFunc)
            where TFailureMechanism : IHasSectionResults<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (getAssemblyFunc == null)
            {
                throw new ArgumentNullException(nameof(getAssemblyFunc));
            }

            return CreateFeatures(failureMechanism, getAssemblyFunc).ToArray();
        }

        private static IEnumerable<MapFeature> CreateFeatures<TFailureMechanism, TSectionResult>(
            TFailureMechanism failureMechanism, Func<TSectionResult, FailureMechanismSectionAssembly> getAssemblyFunc)
            where TFailureMechanism : IHasSectionResults<TSectionResult> where TSectionResult : FailureMechanismSectionResult
        {
            foreach (TSectionResult sectionResult in failureMechanism.SectionResults)
            {
                MapFeature feature = RingtoetsMapDataFeaturesFactory.CreateSingleLineMapFeature(sectionResult.Section.Points);

                FailureMechanismSectionAssembly assemblyResult;
                try
                {
                    assemblyResult = getAssemblyFunc(sectionResult);
                }
                catch (AssemblyException)
                {
                    continue;
                }

                feature.MetaData[Resources.AssemblyCategory_Group_DisplayName] =
                    new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyCategoryGroup>(
                        DisplayFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(assemblyResult.Group)).DisplayName;

                feature.MetaData[Resources.MetaData_Probability] = assemblyResult.Probability;

                yield return feature;
            }
        }
    }
}