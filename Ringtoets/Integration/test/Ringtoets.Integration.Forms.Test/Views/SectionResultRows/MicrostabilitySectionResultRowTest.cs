﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultRows
{
    [TestFixture]
    public class MicrostabilitySectionResultRowTest
    {
        [Test]
        public void Constructor_WithoutSectionResult_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MicrostabilitySectionResultRow(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Constructor_WithSectionResult_PropertiesFromSectionAndResult()
        {
            // Setup
            var section = CreateSection();
            var result = new MicrostabilityFailureMechanismSectionResult(section);

            // Call
            var row = new MicrostabilitySectionResultRow(result);

            // Assert
            Assert.AreEqual(section.Name, row.Name);
            Assert.AreEqual(result.AssessmentLayerOne, row.AssessmentLayerOne);
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);
            Assert.AreEqual(result.AssessmentLayerThree, row.AssessmentLayerThree);

            Assert.IsTrue(TypeUtils.HasTypeConverter<MicrostabilitySectionResultRow,
                              FailureMechanismSectionResultNoValueRoundedDoubleConverter>(
                                  r => r.AssessmentLayerThree));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssessmentLayerOne_AlwaysOnChange_NotifyObserversOfResultAndResultPropertyChanged(bool newValue)
        {
            // Setup
            var section = CreateSection();
            var result = new MicrostabilityFailureMechanismSectionResult(section);
            var row = new MicrostabilitySectionResultRow(result);

            int counter = 0;
            using (new Observer(() => counter++)
            {
                Observable = result
            })
            {
                // Call
                row.AssessmentLayerOne = newValue;

                // Assert
                Assert.AreEqual(1, counter);
                Assert.AreEqual(newValue, result.AssessmentLayerOne);
            }
        }

        [Test]
        public void AssessmentLayerTwoA_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var newValue = AssessmentLayerTwoAResult.Successful;
            var section = CreateSection();
            var result = new MicrostabilityFailureMechanismSectionResult(section);
            var row = new MicrostabilitySectionResultRow(result);

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
            var result = new MicrostabilityFailureMechanismSectionResult(section);
            var row = new MicrostabilitySectionResultRow(result);

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