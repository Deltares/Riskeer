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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.TestUtil;

namespace Riskeer.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismTest
    {
        [Test]
        public void ConstructorWithSectionAssemblyResults_FailureMechanismAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                null, Enumerable.Empty<ExportableAggregatedFailureMechanismSectionAssemblyResultBase>(),
                random.NextEnumValue<ExportableFailureMechanismType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismAssembly", exception.ParamName);
        }

        [Test]
        public void ConstructorWithSectionAssemblyResults_SectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                null, random.NextEnumValue<ExportableFailureMechanismType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionAssemblyResults", exception.ParamName);
        }

        [Test]
        public void ConstructorWithSectionAssemblyResults_WithValidArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            ExportableFailureMechanismAssemblyResult failureMechanismAssembly =
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability();
            IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultBase> sectionAssemblyResults =
                Enumerable.Empty<ExportableAggregatedFailureMechanismSectionAssemblyResultBase>();
            var code = random.NextEnumValue<ExportableFailureMechanismType>();

            // Call
            var failureMechanism = new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                failureMechanismAssembly, sectionAssemblyResults, code);

            // Assert
            Assert.AreSame(failureMechanismAssembly, failureMechanism.FailureMechanismAssembly);
            Assert.AreSame(sectionAssemblyResults, failureMechanism.SectionAssemblyResults);
            Assert.AreEqual(code, failureMechanism.Code);
        }
    }
}