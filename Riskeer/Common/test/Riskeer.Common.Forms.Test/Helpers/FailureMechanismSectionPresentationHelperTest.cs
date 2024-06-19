﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class FailureMechanismSectionPresentationHelperTest
    {
        [Test]
        public void CreatePresentableFailureMechanismSections_FailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSections(null, (section, start, end) => new object());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSections", exception.ParamName);
        }

        [Test]
        public void CreatePresentableFailureMechanismSections_CreatePresentableFailureMechanismSectionFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSections<object>(Enumerable.Empty<FailureMechanismSection>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("createPresentableFailureMechanismSectionFunc", exception.ParamName);
        }

        [Test]
        public void CreatePresentableFailureMechanismSections_ValidInputParameters_ReturnsExpectedPresentationObjects()
        {
            // Setup
            FailureMechanismSection[] failureMechanismSections =
            {
                CreateFailureMechanismSection(0.0, 0.0, 1.0, 1.0),
                CreateFailureMechanismSection(1.0, 1.0, 2.0, 2.0),
                CreateFailureMechanismSection(2.0, 2.0, 5.0, 5.0)
            };

            // Call
            TestPresentableFailureMechanismSection[] presentationObjects = FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSections(
                failureMechanismSections,
                (section, start, end) => new TestPresentableFailureMechanismSection(section, start, end));

            // Assert
            Assert.AreEqual(failureMechanismSections.Length, presentationObjects.Length);

            double sectionOffset = 0;

            for (var i = 0; i < presentationObjects.Length; i++)
            {
                Assert.AreSame(failureMechanismSections[i], presentationObjects[i].Section);
                Assert.AreEqual(sectionOffset, presentationObjects[i].SectionStart);
                sectionOffset += failureMechanismSections[i].Length;
                Assert.AreEqual(sectionOffset, presentationObjects[i].SectionEnd);
            }
        }

        [Test]
        public void CreatePresentableFailureMechanismSectionConfigurations_FailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSectionConfigurations<object, FailureMechanismSectionConfiguration>(
                null, (section, start, end) => new object());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionConfigurations", exception.ParamName);
        }

        [Test]
        public void CreatePresentableFailureMechanismSectionConfigurations_CreatePresentableFailureMechanismSectionConfigurationFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSectionConfigurations<object, FailureMechanismSectionConfiguration>(
                Enumerable.Empty<FailureMechanismSectionConfiguration>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("createPresentableFailureMechanismSectionConfigurationFunc", exception.ParamName);
        }

        [Test]
        public void CreatePresentableFailureMechanismSectionConfigurations_ValidInputParameters_ReturnsExpectedPresentationObjects()
        {
            // Setup
            FailureMechanismSection[] failureMechanismSections =
            {
                CreateFailureMechanismSection(0.0, 0.0, 1.0, 1.0),
                CreateFailureMechanismSection(1.0, 1.0, 2.0, 2.0),
                CreateFailureMechanismSection(2.0, 2.0, 5.0, 5.0)
            };

            IEnumerable<FailureMechanismSectionConfiguration> failureMechanismSectionConfigurations =
                failureMechanismSections.Select(c => new FailureMechanismSectionConfiguration(c));

            // Call
            TestPresentableFailureMechanismSection[] presentationObjects = FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSectionConfigurations(
                failureMechanismSectionConfigurations,
                (configuration, start, end) => new TestPresentableFailureMechanismSection(configuration.Section, start, end));

            // Assert
            Assert.AreEqual(failureMechanismSections.Length, presentationObjects.Length);

            double sectionOffset = 0;

            for (var i = 0; i < presentationObjects.Length; i++)
            {
                Assert.AreSame(failureMechanismSections[i], presentationObjects[i].Section);
                Assert.AreEqual(sectionOffset, presentationObjects[i].SectionStart);
                sectionOffset += failureMechanismSections[i].Length;
                Assert.AreEqual(sectionOffset, presentationObjects[i].SectionEnd);
            }
        }

        private static FailureMechanismSection CreateFailureMechanismSection(double x1, double y1, double x2, double y2)
        {
            return FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(x1, y1),
                new Point2D(x2, y2)
            });
        }

        private class TestPresentableFailureMechanismSection
        {
            public TestPresentableFailureMechanismSection(FailureMechanismSection section,
                                                          double sectionStart,
                                                          double sectionEnd)
            {
                Section = section;
                SectionStart = sectionStart;
                SectionEnd = sectionEnd;
            }

            public FailureMechanismSection Section { get; }

            public double SectionStart { get; }

            public double SectionEnd { get; }
        }
    }
}