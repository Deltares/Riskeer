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
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class FailureMechanismSectionPresentationHelperTest
    {
        [Test]
        public void CreatePresentableFailureMechanismSections_FailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSections(null, (section, start, end) => new object());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSections", exception.ParamName);
        }

        [Test]
        public void CreatePresentableFailureMechanismSections_CreatePresentableFailureMechanismSectionFuncNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSections<object>(Enumerable.Empty<FailureMechanismSection>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("createPresentableFailureMechanismSectionFunc", exception.ParamName);
        }

        [Test]
        public void CreatePresentableFailureMechanismSections_ValidInputParameters_ReturnsExpectedPresentationObjects()
        {
            // Setup
            var failureMechanismSections = new[]
            {
                CreateFailureMechanismSection("a", 0.0, 0.0, 1.0, 1.0),
                CreateFailureMechanismSection("b", 1.0, 1.0, 2.0, 2.0),
                CreateFailureMechanismSection("c", 2.0, 2.0, 5.0, 5.0)
            };

            // Call
            TestPresentableFailureMechanismSection[] presentationObjects = FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSections(
                failureMechanismSections,
                (section, start, end) => new TestPresentableFailureMechanismSection(section, start, end));

            // Assert
            Assert.AreEqual(3, presentationObjects.Length);

            double sectionOffset = 0;

            Assert.AreSame(failureMechanismSections[0], presentationObjects[0].Section);
            Assert.AreEqual(sectionOffset, presentationObjects[0].SectionStart);
            sectionOffset += failureMechanismSections[0].Length;
            Assert.AreEqual(sectionOffset, presentationObjects[0].SectionEnd);

            Assert.AreSame(failureMechanismSections[1], presentationObjects[1].Section);
            Assert.AreEqual(sectionOffset, presentationObjects[1].SectionStart);
            sectionOffset += failureMechanismSections[1].Length;
            Assert.AreEqual(sectionOffset, presentationObjects[1].SectionEnd);

            Assert.AreSame(failureMechanismSections[2], presentationObjects[2].Section);
            Assert.AreEqual(sectionOffset, presentationObjects[2].SectionStart);
            sectionOffset += failureMechanismSections[2].Length;
            Assert.AreEqual(sectionOffset, presentationObjects[2].SectionEnd);
        }

        private static FailureMechanismSection CreateFailureMechanismSection(string name, double x1, double y1, double x2, double y2)
        {
            return new FailureMechanismSection(name, new[]
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