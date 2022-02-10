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
using Assembly.Kernel.Model.FailureMechanismSections;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionListCreatorTest
    {
        [Test]
        public void Create_FailureMechanismSectionsCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionListCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionsCollection", exception.ParamName);
        }

        [Test]
        public void Create_WithFailureMechanism_ReturnFailureMechanismSectionLists()
        {
            // Setup
            var random = new Random(21);
            var combinedAssemblyFailureMechanismInputs = new[]
            {
                new[]
                {
                    new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>()),
                    new CombinedAssemblyFailureMechanismSection(1, 2, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>())
                }
            };

            // Call
            IEnumerable<FailureMechanismSectionList> failureMechanismSectionLists = FailureMechanismSectionListCreator.Create(combinedAssemblyFailureMechanismInputs);

            // Assert
            CombinedFailureMechanismSectionsInputAssert.AssertCombinedFailureMechanismInput(combinedAssemblyFailureMechanismInputs, failureMechanismSectionLists);
        }
    }
}