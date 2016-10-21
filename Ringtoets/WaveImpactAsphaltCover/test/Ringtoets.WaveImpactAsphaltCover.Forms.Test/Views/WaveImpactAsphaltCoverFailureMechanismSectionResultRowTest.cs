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

using System;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.Views;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Test.Views
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);

            // Call
            var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<WaveImpactAsphaltCoverFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);
            Assert.IsTrue(TypeUtils.HasTypeConverter<WaveImpactAsphaltCoverFailureMechanismSectionResultRow,
                              NoValueRoundedDoubleConverter>(
                                  r => r.AssessmentLayerTwoA));
        }

        [Test]
        public void AssessmentLayerTwoA_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            var newValue = random.NextDouble();
            FailureMechanismSection section = CreateSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);
            var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result);

            // Call
            row.AssessmentLayerTwoA = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerTwoA, row.AssessmentLayerTwoA.GetAccuracy());
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("name", new[]
            {
                new Point2D(0, 0)
            });
        }
    }
}