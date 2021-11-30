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
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Factory for creating a valid <see cref="FailureMechanismSectionResultOld"/> which can 
    /// be used for testing.
    /// </summary>
    public static class FailureMechanismSectionResultTestFactory
    {
        /// <summary>
        /// Creates a default <see cref="TestFailureMechanismSectionResultOld"/>.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <returns>A valid <see cref="TestFailureMechanismSectionResultOld"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <param name="name"></param>
        /// is <c>null</c>.</exception>
        public static TestFailureMechanismSectionResultOld CreateFailureMechanismSectionResultOld(string name = "test")
        {
            return new TestFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(name));
        }
        
        /// <summary>
        /// Creates a default <see cref="TestFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <returns>A valid <see cref="TestFailureMechanismSectionResultOld"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <param name="name"></param>
        /// is <c>null</c>.</exception>
        public static TestFailureMechanismSectionResult CreateFailureMechanismSectionResult(string name = "test")
        {
            return new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(name));
        }
    }
}