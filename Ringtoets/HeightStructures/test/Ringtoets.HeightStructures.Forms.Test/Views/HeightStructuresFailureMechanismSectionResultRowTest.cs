// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.Views;

namespace Ringtoets.HeightStructures.Forms.Test.Views
{
    [TestFixture]
    public class HeightStructuresFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var section = new FailureMechanismSection("name", new[]
            {
                new Point2D(0, 0)
            });
            var result = new HeightStructuresFailureMechanismSectionResult(section);

            // Call
            var row = new HeightStructuresFailureMechanismSectionResultRow(result);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<HeightStructuresFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);
            Assert.IsTrue(TypeUtils.HasTypeConverter<HeightStructuresFailureMechanismSectionResultRow,
                              FailureMechanismSectionResultNoProbabilityValueDoubleConverter>(
                                  r => r.AssessmentLayerTwoA));
        }
    }
}