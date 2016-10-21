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
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultRows
{
    [TestFixture]
    public class DuneErosionSectionResultRowTest
    {
        [Test]
        public void Constructor_WithSectionResult_ExpectedValues()
        {
            // Setup
            var section = CreateSection();
            var result = new DuneErosionFailureMechanismSectionResult(section);

            // Call
            var row = new DuneErosionSectionResultRow(result);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<DuneErosionFailureMechanismSectionResult>>(row);
        }

        [Test]
        public void Constructor_WithSectionResult_PropertiesFromSectionAndResult()
        {
            // Setup
            var section = CreateSection();
            var result = new DuneErosionFailureMechanismSectionResult(section);

            // Call
            var row = new DuneErosionSectionResultRow(result);

            // Assert
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);
            Assert.AreEqual(result.AssessmentLayerThree, row.AssessmentLayerThree);

            Assert.IsTrue(TypeUtils.HasTypeConverter<DuneErosionSectionResultRow,
                              NoValueRoundedDoubleConverter>(
                                  r => r.AssessmentLayerThree));
        }

        [Test]
        public void AssessmentLayerTwoA_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var newValue = AssessmentLayerTwoAResult.Successful;
            var section = CreateSection();
            var result = new DuneErosionFailureMechanismSectionResult(section);
            var row = new DuneErosionSectionResultRow(result);

            // Call
            row.AssessmentLayerTwoA = newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerTwoA);
        }

        [Test]
        public void AssessmentLayerThree_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            var newValue = random.NextDouble();
            var section = CreateSection();
            var result = new DuneErosionFailureMechanismSectionResult(section);
            var row = new DuneErosionSectionResultRow(result);

            // Call
            row.AssessmentLayerThree = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerThree, row.AssessmentLayerThree.GetAccuracy());
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