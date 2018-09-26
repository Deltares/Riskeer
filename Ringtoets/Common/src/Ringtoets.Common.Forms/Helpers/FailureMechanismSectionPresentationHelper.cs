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
using System.Collections.Generic;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for displaying <see cref="FailureMechanismSection"/>.
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
        /// for creating the presentation objects of type <typeparam name="T"/>, in which:
        /// <list type="bullet">
        /// <item>T1 represents the failure mechanism section at stake;</item>
        /// <item>T2 represents the start of the section in relation to the beginning of the reference line;</item>
        /// <item>T3 represents the end of the section in relation to the beginning of the reference line.</item>
        /// </list>
        /// </param>
        /// <returns>The created presentation objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSections"/> or
        /// <paramref name="createPresentableFailureMechanismSectionFunc"/> is <c>null</c>.</exception>
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

            double start = 0;

            var presentableFailureMechanismSections = new List<T>();

            foreach (FailureMechanismSection failureMechanismSection in failureMechanismSections)
            {
                double end = start + failureMechanismSection.Length;

                presentableFailureMechanismSections.Add(createPresentableFailureMechanismSectionFunc(failureMechanismSection, start, end));

                start = end;
            }

            return presentableFailureMechanismSections.ToArray();
        }
    }
}