﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Integration.IO.TestUtil;
using Riskeer.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbabilityTest
    {
        [Test]
        public void Constructor_SimpleAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                null,
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("simpleAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_DetailedAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                null,
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("detailedAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_TailorMadeAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                null,
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("tailorMadeAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidArguments_ExpectedValues()
        {
            // Setup
            ExportableFailureMechanismSection failureMechanismSection = ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();
            ExportableSectionAssemblyResultWithProbability simpleAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability();
            ExportableSectionAssemblyResultWithProbability detailedAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability();
            ExportableSectionAssemblyResultWithProbability tailorMadeAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability();
            ExportableSectionAssemblyResultWithProbability combinedAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability();

            // Call
            var assemblyResult = new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(failureMechanismSection,
                                                                                                              simpleAssembly,
                                                                                                              detailedAssembly,
                                                                                                              tailorMadeAssembly,
                                                                                                              combinedAssembly);

            // Assert
            Assert.IsInstanceOf<ExportableAggregatedFailureMechanismSectionAssemblyResultBase>(assemblyResult);

            Assert.AreSame(failureMechanismSection, assemblyResult.FailureMechanismSection);
            Assert.AreSame(simpleAssembly, assemblyResult.SimpleAssembly);
            Assert.AreSame(detailedAssembly, assemblyResult.DetailedAssembly);
            Assert.AreSame(tailorMadeAssembly, assemblyResult.TailorMadeAssembly);
            Assert.AreSame(combinedAssembly, assemblyResult.CombinedAssembly);
        }
    }
}