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
using System.Collections.Generic;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for displaying failure mechanism sections.
    /// </summary>
    public static class FailureMechanismSectionPresentationHelper
    {
        /// <summary>
        /// Creates presentation objects for the provided <paramref name="failureMechanismSections"/>,
        /// taking into account the start and the end of the sections in relation to the beginning of
        /// the reference line.
        /// </summary>
        /// <typeparam name="T">The type of the presentation objects.</typeparam>
        /// <param name="failureMechanismSections">The failure mechanism sections to create presentation
        /// objects for.</param>
        /// <param name="createPresentableFailureMechanismSectionFunc"><see cref="Func{T1,T2,T3,TResult}"/>
        /// for creating the presentation objects of type <typeparamref name="T"/>, in which:
        /// <list type="bullet">
        /// <item>T1 represents the failure mechanism section at stake;</item>
        /// <item>T2 represents the start of the section in relation to the beginning of the reference line;</item>
        /// <item>T3 represents the end of the section in relation to the beginning of the reference line.</item>
        /// </list>
        /// </param>
        /// <returns>The created presentation objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static T[] CreatePresentableFailureMechanismSections<T>(IEnumerable<FailureMechanismSection> failureMechanismSections,
                                                                       Func<FailureMechanismSection, double, double, T> createPresentableFailureMechanismSectionFunc)
        {
            if (failureMechanismSections == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSections));
            }

            if (createPresentableFailureMechanismSectionFunc == null)
            {
                throw new ArgumentNullException(nameof(createPresentableFailureMechanismSectionFunc));
            }

            return CreatePresentableFailureMechanismSections(failureMechanismSections, section => section.Length, createPresentableFailureMechanismSectionFunc);
        }

        /// <summary>
        /// Creates presentation objects for the provided <paramref name="failureMechanismSectionConfigurations"/>,
        /// taking into account the start and the end of the sections in relation to the beginning of
        /// the reference line.
        /// </summary>
        /// <typeparam name="T">The type of the presentation objects.</typeparam>
        /// <typeparam name="TFailureMechanismSectionConfiguration">The type of failure mechanism section configuration.</typeparam>
        /// <param name="failureMechanismSectionConfigurations">The failure mechanism section configurations to create presentation
        /// objects for.</param>
        /// <param name="createPresentableFailureMechanismSectionConfigurationFunc"><see cref="Func{T1,T2,T3,TResult}"/>
        /// for creating the presentation objects of type <typeparamref name="T"/>, in which:
        /// <list type="bullet">
        /// <item>T1 represents the failure mechanism section configuration at stake;</item>
        /// <item>T2 represents the start of the section in relation to the beginning of the reference line;</item>
        /// <item>T3 represents the end of the section in relation to the beginning of the reference line.</item>
        /// </list>
        /// </param>
        /// <returns>The created presentation objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static T[] CreatePresentableFailureMechanismSectionConfigurations<T, TFailureMechanismSectionConfiguration>(
            IEnumerable<TFailureMechanismSectionConfiguration> failureMechanismSectionConfigurations,
            Func<TFailureMechanismSectionConfiguration, double, double, T> createPresentableFailureMechanismSectionConfigurationFunc)
            where TFailureMechanismSectionConfiguration : FailureMechanismSectionConfiguration
        {
            if (failureMechanismSectionConfigurations == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionConfigurations));
            }

            if (createPresentableFailureMechanismSectionConfigurationFunc == null)
            {
                throw new ArgumentNullException(nameof(createPresentableFailureMechanismSectionConfigurationFunc));
            }

            return CreatePresentableFailureMechanismSections(failureMechanismSectionConfigurations, configuration => configuration.Section.Length, createPresentableFailureMechanismSectionConfigurationFunc);
        }

        private static T[] CreatePresentableFailureMechanismSections<T, TSection>(IEnumerable<TSection> sections,
                                                                                  Func<TSection, double> getSectionLengthFunc,
                                                                                  Func<TSection, double, double, T> createPresentableFailureMechanismSectionFunc)
        {
            double start = 0;

            var presentableFailureMechanismSections = new List<T>();

            foreach (TSection section in sections)
            {
                double end = start + getSectionLengthFunc(section);

                presentableFailureMechanismSections.Add(createPresentableFailureMechanismSectionFunc(section, start, end));

                start = end;
            }

            return presentableFailureMechanismSections.ToArray();
        }
    }
}