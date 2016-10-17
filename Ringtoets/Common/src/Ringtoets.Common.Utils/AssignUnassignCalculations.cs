﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Utils
{
    /// <summary>
    /// Utility class for data synchronization of the <see cref="ICalculation"/> 
    /// of <see cref="FailureMechanismSectionResult"/> objects.
    /// </summary>
    public static class AssignUnassignCalculations
    {
        /// <summary>
        /// Update <see cref="FailureMechanismSectionResult"/> objects which used or can use the <see cref="ICalculation"/>.
        /// </summary>
        /// <param name="sectionResults">The <see cref="SectionResultWithCalculationAssignment"/> objects which contain the
        /// information about assigning calculations to sections.</param>
        /// <param name="calculation">The <see cref="CalculationWithLocation"/> which's location is used to match with the
        /// location of the section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sectionResults"/> contains a <c>null</c> element.</exception>
        public static void Update(IEnumerable<SectionResultWithCalculationAssignment> sectionResults, CalculationWithLocation calculation)
        {
            ValidateSectionResults(sectionResults);
            ValidateCalculationWithLocation(calculation);

            var sectionResultsArray = sectionResults.ToArray();

            FailureMechanismSection failureMechanismSectionContainingCalculation =
                FailureMechanismSectionForCalculation(sectionResultsArray.Select(sr => sr.Result.Section), calculation);

            UnassignCalculationInSectionResultsNotContainingCalculation(calculation, sectionResultsArray, failureMechanismSectionContainingCalculation);

            AssignCalculationIfContainingSectionResultHasNoCalculationAssigned(calculation, sectionResultsArray, failureMechanismSectionContainingCalculation);
        }

        /// <summary>
        /// Update <see cref="FailureMechanismSectionResult"/> objects which use the deleted <see cref="ICalculation"/>.
        /// </summary>
        /// <param name="sectionResults">The <see cref="SectionResultWithCalculationAssignment"/> objects which contain the
        /// information about assigning calculations to sections.</param>
        /// <param name="calculation">The <see cref="ICalculation"/> that was removed.</param>
        /// <param name="calculations">All the remaining calculations after deletion of the <paramref name="calculation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sectionResults"/> or <paramref name="calculations"/>
        /// contains a <c>null</c> element.</exception>
        public static void Delete(
            IEnumerable<SectionResultWithCalculationAssignment> sectionResults,
            ICalculation calculation,
            IEnumerable<CalculationWithLocation> calculations)
        {
            ValidateSectionResults(sectionResults);
            ValidateCalculations(calculations);
            ValidateCalculation(calculation);

            var sectionResultsArray = sectionResults.ToArray();

            Dictionary<string, IList<ICalculation>> calculationsPerSegmentName =
                CollectCalculationsPerSection(sectionResultsArray.Select(sr => sr.Result.Section), calculations);

            UnassignCalculationInAllSectionResultsAndAssignSingleRemainingCalculation(sectionResultsArray, calculation, calculationsPerSegmentName);
        }

        /// <summary>
        /// Determine which <see cref="ICalculation"/> objects are available for a
        /// <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="sections">The <see cref="FailureMechanismSection"/> objects.</param>
        /// <param name="calculations">The <see cref="CalculationWithLocation"/> objects.</param>
        /// <returns>A <see cref="Dictionary{K, V}"/> containing a <see cref="IList{T}"/> 
        /// of <see cref="FailureMechanismSectionResult"/> objects 
        /// for each section name which has calculations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sections"/> or <paramref name="calculations"/>
        /// contains a <c>null</c> element.</exception>
        public static Dictionary<string, IList<ICalculation>> CollectCalculationsPerSection(
            IEnumerable<FailureMechanismSection> sections,
            IEnumerable<CalculationWithLocation> calculations)
        {
            ValidateSections(sections);
            ValidateCalculations(calculations);

            SectionSegments[] sectionSegments = SectionSegmentsHelper.MakeSectionSegments(sections);

            var calculationsPerSegment = new Dictionary<string, IList<ICalculation>>();

            foreach (var calculationWithLocation in calculations)
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

        private static void UnassignCalculationInAllSectionResultsAndAssignSingleRemainingCalculation(
            IEnumerable<SectionResultWithCalculationAssignment> sectionResults,
            ICalculation calculation, Dictionary<string, IList<ICalculation>> calculationsPerSegmentName)
        {
            IEnumerable<SectionResultWithCalculationAssignment> sectionResultsUsingCalculation =
                sectionResults.Where(sr => sr.Calculation != null && ReferenceEquals(sr.Calculation, calculation));

            foreach (var sectionResult in sectionResultsUsingCalculation)
            {
                string sectionName = sectionResult.Result.Section.Name;
                if (calculationsPerSegmentName.ContainsKey(sectionName) && calculationsPerSegmentName[sectionName].Count == 1)
                {
                    sectionResult.Calculation = calculationsPerSegmentName[sectionName].Single();
                    continue;
                }
                sectionResult.Calculation = null;
            }
        }

        private static void AssignCalculationIfContainingSectionResultHasNoCalculationAssigned(
            CalculationWithLocation calculationWithLocation,
            IEnumerable<SectionResultWithCalculationAssignment> sectionResults,
            FailureMechanismSection failureMechanismSection)
        {
            foreach (var sectionResult in sectionResults)
            {
                if (ReferenceEquals(sectionResult.Result.Section, failureMechanismSection) && sectionResult.Calculation == null)
                {
                    sectionResult.Calculation = calculationWithLocation.Calculation;
                }
            }
        }

        private static void UnassignCalculationInSectionResultsNotContainingCalculation(
            CalculationWithLocation calculationWithLocation,
            IEnumerable<SectionResultWithCalculationAssignment> sectionResults,
            FailureMechanismSection failureMechanismSection)
        {
            IEnumerable<SectionResultWithCalculationAssignment> sectionResultsUsingCalculation =
                sectionResults.Where(sr => sr.Calculation != null && ReferenceEquals(sr.Calculation, calculationWithLocation.Calculation));
            foreach (var sectionResult in sectionResultsUsingCalculation)
            {
                if (!ReferenceEquals(sectionResult.Result.Section, failureMechanismSection))
                {
                    sectionResult.Calculation = null;
                }
            }
        }

        private static FailureMechanismSection FindSectionAtLocation(IEnumerable<SectionSegments> sectionSegmentsCollection,
                                                                     Point2D location)
        {
            return SectionSegmentsHelper.GetSectionForPoint(sectionSegmentsCollection, location);
        }

        private static void UpdateCalculationsOfSegment(Dictionary<string, IList<ICalculation>> calculationsPerSegment,
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
                throw new ArgumentNullException("sections");
            }
            if (sections.Any(s => s == null))
            {
                throw new ArgumentException("Sections contains an entry without value.", "sections");
            }
        }

        private static void ValidateCalculation(ICalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }
        }

        private static void ValidateCalculationWithLocation(CalculationWithLocation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }
        }

        private static void ValidateCalculations(IEnumerable<CalculationWithLocation> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException("calculations");
            }
            if (calculations.Any(s => s == null))
            {
                throw new ArgumentException("Calculations contains an entry without value.", "calculations");
            }
        }

        private static void ValidateSectionResults(IEnumerable<SectionResultWithCalculationAssignment> sectionResults)
        {
            if (sectionResults == null)
            {
                throw new ArgumentNullException("sectionResults");
            }
            if (sectionResults.Any(s => s == null))
            {
                throw new ArgumentException("SectionResults contains an entry without value.", "sectionResults");
            }
        }

        #endregion
    }
}