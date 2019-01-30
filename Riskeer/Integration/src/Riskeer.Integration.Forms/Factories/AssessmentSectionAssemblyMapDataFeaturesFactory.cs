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
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.Util;
using Core.Components.Gis.Features;
using Riskeer.AssemblyTool.Forms;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Forms.Factories;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Forms.Properties;
using Riskeer.Integration.Util;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.Factories
{
    /// <summary>
    /// Factory for creating collections of <see cref="MapFeature"/> for assembly results
    /// in an <see cref="AssessmentSection"/>.
    /// </summary>
    public static class AssessmentSectionAssemblyMapDataFeaturesFactory
    {
        /// <summary>
        /// Creates features for the combined failure mechanism section assembly.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to create the features for.</param>
        /// <returns>A collection of <see cref="MapFeature"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<MapFeature> CreateCombinedFailureMechanismSectionAssemblyFeatures(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            IEnumerable<CombinedFailureMechanismSectionAssemblyResult> assemblyResults;
            try
            {
                assemblyResults =
                    AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection, true);
            }
            catch (AssemblyException)
            {
                return new MapFeature[0];
            }

            var mapFeatures = new List<MapFeature>();
            foreach (CombinedFailureMechanismSectionAssemblyResult assemblyResult in assemblyResults)
            {
                IEnumerable<Point2D> geometry = FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(assessmentSection.ReferenceLine,
                                                                                                                 assemblyResult.SectionStart,
                                                                                                                 assemblyResult.SectionEnd);
                MapFeature mapFeature = RingtoetsMapDataFeaturesFactory.CreateSingleLineMapFeature(geometry);

                mapFeature.MetaData[RiskeerCommonFormsResources.AssemblyCategory_Group_DisplayName] =
                    new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyCategoryGroup>(
                        DisplayFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(assemblyResult.TotalResult)).DisplayName;
                mapFeature.MetaData[Resources.SectionNumber_DisplayName] = assemblyResult.SectionNumber;

                mapFeatures.Add(mapFeature);
            }

            return mapFeatures;
        }
    }
}