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

using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.TestUtil.Test
{
    [TestFixture]
    public class ExportableFailureMechanismSectionTestFactoryTest
    {
        [Test]
        public void CreateFailureMechanismSection_Always_ReturnsFailureMechanismSection()
        {
            // Call
            ExportableFailureMechanismSection section = ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, section.Geometry);
            Assert.AreEqual(1, section.StartDistance);
            Assert.AreEqual(2, section.EndDistance);
        }

        [Test]
        public void CreateCombinedFailureMechanismSection_Always_ReturnsCombinedFailureMechanismSection()
        {
            // Call
            ExportableCombinedFailureMechanismSection section = ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, 1),
                new Point2D(3, 3)
            }, section.Geometry);
            Assert.AreEqual(1, section.StartDistance);
            Assert.AreEqual(3, section.EndDistance);
            Assert.AreEqual(ExportableAssemblyMethod.WBI3A1, section.AssemblyMethod);
        }
    }
}