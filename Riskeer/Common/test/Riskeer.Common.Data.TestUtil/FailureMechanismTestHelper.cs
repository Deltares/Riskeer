﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Class that contains helper methods for an <see cref="IFailureMechanism"/>.
    /// </summary>
    public static class FailureMechanismTestHelper
    {
        /// <summary>
        /// Sets a collection of <see cref="FailureMechanismSection"/> to <see cref="IFailureMechanism.Sections"/>
        /// with an empty source path.
        /// </summary>
        /// <param name="failurePath">The failure path to set the sections to.</param>
        /// <param name="sections">The sections to set.</param>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="sections"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sections"/> contains elements that
        /// are not properly connected.</exception>
        public static void SetSections(IFailureMechanism failurePath, IEnumerable<FailureMechanismSection> sections)
        {
            failurePath.SetSections(sections, string.Empty);
        }

        /// <summary>
        /// Adds a number of failure mechanism sections to <paramref name="failureMechanism"/>
        /// based on the <paramref name="numberOfSections"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to add sections to.</param>
        /// <param name="numberOfSections">The number of sections to add to the <paramref name="failureMechanism"/>.</param>
        public static void AddSections(IFailureMechanism failureMechanism, int numberOfSections)
        {
            AddSections(failureMechanism, numberOfSections, new Point2D(-1, -1), new Point2D(15, 15));
        }

        private static void AddSections(IFailureMechanism failureMechanism, int numberOfSections, Point2D startPoint, Point2D endPoint)
        {
            double endPointStepsX = (endPoint.X - startPoint.X) / numberOfSections;
            double endPointStepsY = (endPoint.Y - startPoint.Y) / numberOfSections;

            var sections = new List<FailureMechanismSection>();
            for (var i = 1; i <= numberOfSections; i++)
            {
                endPoint = new Point2D(startPoint.X + endPointStepsX, startPoint.Y + endPointStepsY);
                sections.Add(new FailureMechanismSection(i.ToString(),
                                                         new[]
                                                         {
                                                             startPoint,
                                                             endPoint
                                                         }));
                startPoint = endPoint;
            }

            SetSections(failureMechanism, sections);
        }
    }
}