// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Util.Test
{
    [TestFixture]
    public class SectionSegmentsHelperTest
    {
        [Test]
        public void MakeSectionSegments_SectionsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => SectionSegmentsHelper.MakeSectionSegments(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void MakeSectionSegments_SectionsElementNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => SectionSegmentsHelper.MakeSectionSegments(new FailureMechanismSection[]
            {
                null
            });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void MakeSectionSegments_ValidFailureMechanismSections_ReturnSectionSegmentsFromFailureMechanismSection()
        {
            // Setup
            FailureMechanismSection[] failureMechanismSections =
            {
                new FailureMechanismSection(string.Empty, new[]
                {
                    new Point2D(0, 0)
                }),
                new FailureMechanismSection(string.Empty, new[]
                {
                    new Point2D(1, 1)
                }),
                new FailureMechanismSection(string.Empty, new[]
                {
                    new Point2D(2, 2)
                }),
                new FailureMechanismSection(string.Empty, new[]
                {
                    new Point2D(3, 3)
                })
            };

            // Call
            SectionSegments[] segmentSections = SectionSegmentsHelper.MakeSectionSegments(failureMechanismSections);

            // Assert
            Assert.AreEqual(failureMechanismSections.Length, segmentSections.Length);
            CollectionAssert.AreEqual(failureMechanismSections, segmentSections.Select(ss => ss.Section));
        }

        [Test]
        public void GetSectionForPoint_SectionSegmentsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => SectionSegmentsHelper.GetSectionForPoint(null, new Point2D(0, 0));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("sectionSegmentsCollection", exception.ParamName);
        }

        [Test]
        public void GetSectionForPoint_PointNull_ThrowsArgumentNullException()
        {
            // Setup
            SectionSegments[] sectionSegments =
            {
                new SectionSegments(new FailureMechanismSection(string.Empty, new[]
                {
                    new Point2D(0, 0),
                    new Point2D(2, 2)
                }))
            };

            // Call
            TestDelegate test = () => SectionSegmentsHelper.GetSectionForPoint(sectionSegments, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("point", exception.ParamName);
        }

        [Test]
        public void GetSectionForPoint_SectionSegmentsEmpty_ReturnNull()
        {
            // Call
            FailureMechanismSection section = SectionSegmentsHelper.GetSectionForPoint(Enumerable.Empty<SectionSegments>(), new Point2D(0, 0));

            // Assert
            Assert.IsNull(section);
        }

        [Test]
        public void GetSectionForPoint_PointNotOnSection_ReturnClosestSection()
        {
            // Setup
            var failureMechanismSection1 = new FailureMechanismSection(string.Empty, new[]
            {
                new Point2D(0, 0),
                new Point2D(2, 2)
            });
            var failureMechanismSection2 = new FailureMechanismSection(string.Empty, new[]
            {
                new Point2D(10, 10),
                new Point2D(12, 12)
            });

            SectionSegments[] sectionSegments =
            {
                new SectionSegments(failureMechanismSection1),
                new SectionSegments(failureMechanismSection2)
            };

            // Call
            FailureMechanismSection sectionForPoint = SectionSegmentsHelper.GetSectionForPoint(sectionSegments, new Point2D(3, 4));

            // Assert
            Assert.AreSame(failureMechanismSection1, sectionForPoint);
        }

        [Test]
        public void GetSectionForPoint_PointOnSection_ReturnSection()
        {
            // Setup
            var failureMechanismSection1 = new FailureMechanismSection(string.Empty, new[]
            {
                new Point2D(0, 0),
                new Point2D(2, 2)
            });
            var failureMechanismSection2 = new FailureMechanismSection(string.Empty, new[]
            {
                new Point2D(10, 10),
                new Point2D(12, 12)
            });

            SectionSegments[] sectionSegments =
            {
                new SectionSegments(failureMechanismSection1),
                new SectionSegments(failureMechanismSection2)
            };

            // Call
            FailureMechanismSection sectionForPoint = SectionSegmentsHelper.GetSectionForPoint(sectionSegments, new Point2D(11, 11));

            // Assert
            Assert.AreSame(failureMechanismSection2, sectionForPoint);
        }
    }
}