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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Riskeer.Common.Util
{
    /// <summary>
    /// Utility class for data synchronization of the <see cref="ICalculation"/> 
    /// of <see cref="FailureMechanismSectionResult"/> objects.
    /// </summary>
    public static class AssignUnassignCalculations
    {
        /// <summary>
        /// Update <see cref="FailureMechanismSectionResult"/> objects with the <paramref name="calculations"/>.
        /// </summary>
        /// <param name="sectionResults">The <see cref="SectionResultWithCalculationAssignment"/> objects which contain the
        /// information about assigning calculations to sections.</param>
        /// <param name="calculations">All the currently known calculations to try and match with section results.</param>
        /// <returns>All affected objects by the update.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sectionResults"/> or <paramref name="calculations"/>
        /// contains a <c>null</c> element.</exception>
        public static IEnumerable<FailureMechanismSectionResult> Update(
            IEnumerable<SectionResultWithCalculationAssignment> sectionResults,
            IEnumerable<CalculationWithLocation> calculations)
        {
            ValidateSectionResults(sectionResults);
            ValidateCalculations(calculations);

            SectionResultWithCalculationAssignment[] sectionResultsArray = sectionResults.ToArray();

            IDictionary<string, List<ICalculation>> calculationsPerSegmentName =
                CollectCalculationsPerSection(sectionResultsArray.Select(sr => sr.Result.Section), calculations);

            return UnassignCalculationInAllSectionResultsAndAssignSingleRemainingCalculation(
                sectionResultsArray,
                calculationsPerSegmentName);
        }

        /// <summary>
        /// Determine which <see cref="ICalculation"/> objects are available for a
        /// <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="sections">The <see cref="FailureMechanismSection"/> objects.</param>
        /// <param name="calculations">The <see cref="CalculationWithLocation"/> objects.</param>
        /// <returns>A <see cref="IDictionary{K, V}"/> containing a <see cref="List{T}"/> 
        /// of <see cref="FailureMechanismSectionResult"/> objects 
        /// for each section name which has calculations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sections"/> or <paramref name="calculations"/>
        /// contains a <c>null</c> element.</exception>
        public static IDictionary<string, List<ICalculation>> CollectCalculationsPerSection(
            IEnumerable<FailureMechanismSection> sections,
            IEnumerable<CalculationWithLocation> calculations)
        {
            ValidateSections(sections);
            ValidateCalculations(calculations);

            SectionSegments[] sectionSegments = SectionSegmentsHelper.MakeSectionSegments(sections);

            var calculationsPerSegment = new Dictionary<string, List<ICalculation>>();

            foreach (CalculationWithLocation calculationWithLocation in calculations)
            {
                FailureMechanismSection section = FindSectionAtLocation(sectionSegments, calculationWithLocation.Location);
                if (section == null)
                {
                    continue;
                }

                UpdateCalculationsOfSegment(calculationsPerSegment, section.Name, calculationWithLocation.Calculation);
            }

            return calculationsPerSegment;
        }

        /// <summary>
        /// Determine which <see cref="FailureMechanismSection"/> geometrically contains the <see cref="ICalculation"/>.
        /// </summary>
        /// <param name="sections">The <see cref="FailureMechanismSection"/> objects 
        /// whose <see cref="FailureMechanismSection"/> are considered.</param>
        /// <param name="calculation">The <see cref="ICalculation"/>.</param>
        /// <returns>The containing <see cref="FailureMechanismSection"/>, or <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c> or when 
        /// an element in <paramref name="sections"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sections"/> contains a <c>null</c> element.</exception>
        public static FailureMechanismSection FailureMechanismSectionForCalculation(
            IEnumerable<FailureMechanismSection> sections,
            CalculationWithLocation calculation)
        {
            ValidateSections(sections);
            ValidateCalculationWithLocation(calculation);

            SectionSegments[] sectionSegments = SectionSegmentsHelper.MakeSectionSegments(sections);

            return FindSectionAtLocation(sectionSegments, calculation.Location);
        }

        private static IEnumerable<FailureMechanismSectionResult> UnassignCalculationInAllSectionResultsAndAssignSingleRemainingCalculation(
            IEnumerable<SectionResultWithCalculationAssignment> sectionResults,
            IDictionary<string, List<ICalculation>> calculationsPerSegmentName)
        {
            var affectedObjects = new Collection<FailureMechanismSectionResult>();
            foreach (SectionResultWithCalculationAssignment sectionResult in sectionResults)
            {
                string sectionName = sectionResult.Result.Section.Name;
                var affected = false;
                if (!calculationsPerSegmentName.ContainsKey(sectionName))
                {
                    sectionResult.Calculation = null;
                    affected = true;
                }

                if (calculationsPerSegmentName.ContainsKey(sectionName))
                {
                    IEnumerable<ICalculation> calculationsInCurrentSection = calculationsPerSegmentName[sectionName];
                    if (!calculationsInCurrentSection.Contains(sectionResult.Calculation))
                    {
                        sectionResult.Calculation = null;
                        affected = true;
                    }

                    if (sectionResult.Calculation == null && calculationsInCurrentSection.Count() == 1)
                    {
                        sectionResult.Calculation = calculationsInCurrentSection.Single();
                        affected = true;
                    }
                }

                if (affected)
                {
                    affectedObjects.Add(sectionResult.Result);
                }
            }

            return affectedObjects;
        }

        private static FailureMechanismSection FindSectionAtLocation(IEnumerable<SectionSegments> sectionSegmentsCollection,
                                                                     Point2D location)
        {
            return SectionSegmentsHelper.GetSectionForPoint(sectionSegmentsCollection, location);
        }

        private static void UpdateCalculationsOfSegment(IDictionary<string, List<ICalculation>> calculationsPerSegment,
                                                        string sectionName, ICalculation calculation)
        {
            if (!calculationsPerSegment.ContainsKey(sectionName))
            {
                calculationsPerSegment.Add(sectionName, new List<ICalculation>());
            }

            calculationsPerSegment[sectionName].Add(calculation);
        }

        #region Validate inputs

        private static void ValidateSections(IEnumerable<FailureMechanismSection> sections)
        {
            if (sections == null)
            {
                throw new ArgumentNullException(nameof(sections));
            }

            if (sections.Any(s => s == null))
            {
                throw new ArgumentException(@"Sections contains an entry without value.", nameof(sections));
            }
        }

        private static void ValidateCalculationWithLocation(CalculationWithLocation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
        }

        private static void ValidateCalculations(IEnumerable<CalculationWithLocation> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (calculations.Any(s => s == null))
            {
                throw new ArgumentException(@"Calculations contains an entry without value.", nameof(calculations));
            }
        }

        private static void ValidateSectionResults(IEnumerable<SectionResultWithCalculationAssignment> sectionResults)
        {
            if (sectionResults == null)
            {
                throw new ArgumentNullException(nameof(sectionResults));
            }

            if (sectionResults.Any(s => s == null))
            {
                throw new ArgumentException(@"SectionResults contains an entry without value.", nameof(sectionResults));
            }
        }

        #endregion
    }
}