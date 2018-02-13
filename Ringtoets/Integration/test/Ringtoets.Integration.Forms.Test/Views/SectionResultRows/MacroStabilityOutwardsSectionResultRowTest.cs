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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultRows
{
    [TestFixture]
    public class MacroStabilityOutwardsSectionResultRowTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            // Call
            var row = new MacroStabilityOutwardsSectionResultRow(result);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<MacroStabilityOutwardsFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.SimpleAssessmentInput, row.SimpleAssessmentInput);
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);
            Assert.AreEqual(result.AssessmentLayerThree, row.AssessmentLayerThree);

            TestHelper.AssertTypeConverter<MacroStabilityOutwardsSectionResultRow,
                NoProbabilityValueDoubleConverter>(
                nameof(MacroStabilityOutwardsSectionResultRow.AssessmentLayerTwoA));
            TestHelper.AssertTypeConverter<MacroStabilityOutwardsSectionResultRow,
                NoValueRoundedDoubleConverter>(
                nameof(MacroStabilityOutwardsSectionResultRow.AssessmentLayerThree));
        }

        [Test]
        public void SimpleAssessmentInput_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<SimpleAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new MacroStabilityOutwardsSectionResultRow(result);

            // Call
            row.SimpleAssessmentInput = newValue;

            // Assert
            Assert.AreEqual(newValue, result.SimpleAssessmentInput);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(0.5)]
        [TestCase(1e-6)]
        [TestCase(double.NaN)]
        public void AssessmentLayerTwoA_ForValidValues_ResultPropertyChanged(double value)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityOutwardsSectionResultRow(result);

            // Call
            row.AssessmentLayerTwoA = value;

            // Assert
            Assert.AreEqual(value, row.AssessmentLayerTwoA);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void AssessmentLayerTwoA_ForInvalidValues_ThrowsArgumentException(double value)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityOutwardsSectionResultRow(result);

            // Call
            TestDelegate test = () => row.AssessmentLayerTwoA = value;

            // Assert
            string message = Assert.Throws<ArgumentException>(test).Message;
            const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void AssessmentLayerThree_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            double newValue = random.NextDouble();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityOutwardsSectionResultRow(result);

            // Call
            row.AssessmentLayerThree = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerThree, row.AssessmentLayerThree.GetAccuracy());
        }
    }
}