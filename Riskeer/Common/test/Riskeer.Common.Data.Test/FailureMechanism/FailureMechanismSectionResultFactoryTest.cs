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
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismSectionResultFactoryTest
    {
        [Test]
        public void Create_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultFactory.Create<FailureMechanismSectionResult>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Create_TypeAdoptableFailureMechanismSectionResultAndWithSection_ReturnsExpectedFailureMechanismSectionResult()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var sectionResult = FailureMechanismSectionResultFactory.Create<AdoptableFailureMechanismSectionResult>(section);

            // Assert
            Assert.AreSame(section, sectionResult.Section);
        }

        [Test]
        public void Create_TypeAdoptableWithProfileProbabilityFailureMechanismSectionResultAndWithSection_ReturnsExpectedFailureMechanismSectionResult()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var sectionResult = FailureMechanismSectionResultFactory.Create<AdoptableWithProfileProbabilityFailureMechanismSectionResult>(section);

            // Assert
            Assert.AreSame(section, sectionResult.Section);
        }

        [Test]
        public void Create_TypeNonAdoptableFailureMechanismSectionResultAndWithSection_ReturnsExpectedFailureMechanismSectionResult()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var sectionResult = FailureMechanismSectionResultFactory.Create<NonAdoptableFailureMechanismSectionResult>(section);

            // Assert
            Assert.AreSame(section, sectionResult.Section);
        }

        [Test]
        public void Create_TypeNonAdoptableWithProfileProbabilityFailureMechanismSectionResultAndWithSection_ReturnsExpectedFailureMechanismSectionResult()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var sectionResult = FailureMechanismSectionResultFactory.Create<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(section);

            // Assert
            Assert.AreSame(section, sectionResult.Section);
        }

        [Test]
        public void Create_InvalidType_ThrowsNotSupportedException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            void Call() => FailureMechanismSectionResultFactory.Create<FailureMechanismSectionResult>(section);

            // Assert
            Assert.Throws<NotSupportedException>(Call);
        }
    }
}