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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Data.StandAlone.SectionResult;
using Ringtoets.Integration.Forms.Views.SectionResultRow;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultRow
{
    [TestFixture]
    public class StrengthStabilityPointConstructionSectionResultRowTest
    {
        [Test]
        public void Constructor_WithoutSectionResult_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StrengthStabilityPointConstructionSectionResultRow(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Constructor_WithSectionResult_PropertiesFromSectionAndResult()
        {
            // Setup
            var section = CreateSection();
            var result = new StrengthStabilityPointConstructionFailureMechanismSectionResult(section);

            // Call
            var row = new StrengthStabilityPointConstructionSectionResultRow(result);

            // Assert
            Assert.AreEqual(section.Name, row.Name);
            Assert.AreEqual(result.AssessmentLayerOne, row.AssessmentLayerOne);

            var expected2AValue = (RoundedDouble) (1 / result.AssessmentLayerTwoA);
            var expected2AValueString = string.Format(
                CoreCommonBaseResources.ProbabilityPerYearFormat, 
                expected2AValue
            );
            Assert.AreEqual(
                expected2AValueString, 
                row.AssessmentLayerTwoA
            );

            Assert.AreEqual(result.AssessmentLayerThree, row.AssessmentLayerThree);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssessmentLayerOne_AlwaysOnChange_NotifyObserversOfResultAndResultPropertyChanged(bool newValue)
        {
            // Setup
            var section = CreateSection();
            var result = new StrengthStabilityPointConstructionFailureMechanismSectionResult(section);
            var row = new StrengthStabilityPointConstructionSectionResultRow(result);

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
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(0.5)]
        [TestCase(1e-6)]
        public void AssessmentLayerTwoA_ForValidValues_ResultPropertyChanged(double value)
        {
            // Setup
            var newValue = (RoundedDouble) value;
            var section = CreateSection();
            var result = new StrengthStabilityPointConstructionFailureMechanismSectionResult(section);
            var row = new StrengthStabilityPointConstructionSectionResultRow(result);

            // Call
            row.AssessmentLayerTwoA = newValue.ToString();

            // Assert
            var expected = string.Format(CoreCommonBaseResources.ProbabilityPerYearFormat, newValue);
            var actual = string.Format(CoreCommonBaseResources.ProbabilityPerYearFormat, result.AssessmentLayerTwoA);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void AssessmentLayerTwoA_ForInvalidValues_ThrowsArgumentException(double value)
        {
            // Setup
            var newValue = (RoundedDouble)value;
            var section = CreateSection();
            var result = new StrengthStabilityPointConstructionFailureMechanismSectionResult(section);
            var row = new StrengthStabilityPointConstructionSectionResultRow(result);

            // Call
            TestDelegate test = () => row.AssessmentLayerTwoA = newValue.ToString();

            // Assert
            var message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual(
                Common.Data.Properties.Resources.ArbitraryProbabilityFailureMechanismSectionResult_AssessmentLayerTwoA_Value_needs_to_be_between_0_and_1,
                message
            );
        }

        [Test]
        [TestCase(double.MaxValue + 1)]
        [TestCase(double.MinValue - 1)]
        public void AssessmentLayerTwoA_ForTooLargeValues_ThrowsArgumentException(double value)
        {
            // Setup
            var newValue = (RoundedDouble)value;
            var section = CreateSection();
            var result = new StrengthStabilityPointConstructionFailureMechanismSectionResult(section);
            var row = new StrengthStabilityPointConstructionSectionResultRow(result);

            // Call
            TestDelegate test = () => row.AssessmentLayerTwoA = newValue.ToString();

            // Assert
            var message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual(
                Common.Forms.Properties.Resources.ArbitraryProbabilityFailureMechanismSectionResultRow_AssessmentLayerTwoA_Value_too_large,
                message
            );
        }

        [Test]
        [TestCase("many")]
        [TestCase("")]
        public void AssessmentLayerTwoA_ForNonNumericValues_ThrowsArgumentException(string value)
        {
            // Setup
            var section = CreateSection();
            var result = new StrengthStabilityPointConstructionFailureMechanismSectionResult(section);
            var row = new StrengthStabilityPointConstructionSectionResultRow(result);

            // Call
            TestDelegate test = () => row.AssessmentLayerTwoA = value;

            // Assert
            var message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual(
                Common.Forms.Properties.Resources.ArbitraryProbabilityFailureMechanismSectionResultRow_AssessmentLayerTwoA_Could_not_parse_string_to_double_value,
                message
            );
        }

        [Test]
        public void AssessmentLayerTwoA_ForNullValue_ThrowsArgumentException()
        {
            // Setup
            var section = CreateSection();
            var result = new StrengthStabilityPointConstructionFailureMechanismSectionResult(section);
            var row = new StrengthStabilityPointConstructionSectionResultRow(result);

            // Call
            TestDelegate test = () => row.AssessmentLayerTwoA = (string) null;

            // Assert
            var expectedMessage = Common.Forms.Properties.Resources.ArbitraryProbabilityFailureMechanismSectionResultRow_AssessmentLayerTwoA_Value_cannot_be_null;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void AssessmentLayerThree_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            var newValue = random.NextDouble();
            var section = CreateSection();
            var result = new StrengthStabilityPointConstructionFailureMechanismSectionResult(section);
            var row = new StrengthStabilityPointConstructionSectionResultRow(result);

            // Call
            row.AssessmentLayerThree = (RoundedDouble)newValue;

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