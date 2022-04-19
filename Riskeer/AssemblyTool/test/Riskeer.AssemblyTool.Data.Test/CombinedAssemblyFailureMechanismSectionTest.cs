﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.AssemblyTool.Data.Test
{
    [TestFixture]
    public class CombinedAssemblyFailureMechanismSectionTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            var assemblyGroup = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();

            // Call
            var assembly = new CombinedAssemblyFailureMechanismSection(sectionStart, sectionEnd, assemblyGroup);

            // Assert
            Assert.AreEqual(sectionStart, assembly.SectionStart);
            Assert.AreEqual(sectionEnd, assembly.SectionEnd);
            Assert.AreEqual(assemblyGroup, assembly.FailureMechanismSectionAssemblyGroup);
        }
    }
}