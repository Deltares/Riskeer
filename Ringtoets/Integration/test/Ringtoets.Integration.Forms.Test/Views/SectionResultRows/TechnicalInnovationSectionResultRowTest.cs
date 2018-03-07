﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultRows
{
    [TestFixture]
    public class TechnicalInnovationSectionResultRowTest
    {
        [Test]
        public void Constructor_WithProperties_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new TechnicalInnovationFailureMechanismSectionResult(section);

            // Call
            var row = new TechnicalInnovationSectionResultRow(result);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<TechnicalInnovationFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.SimpleAssessmentResult, row.SimpleAssessmentResult);
            Assert.AreEqual(result.AssessmentLayerThree, row.AssessmentLayerThree);

            TestHelper.AssertTypeConverter<TechnicalInnovationSectionResultRow, NoValueRoundedDoubleConverter>(
                nameof(TechnicalInnovationSectionResultRow.AssessmentLayerThree));
        }

        [Test]
        public void SimpleAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<SimpleAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new TechnicalInnovationFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new TechnicalInnovationSectionResultRow(result);

            // Call
            row.SimpleAssessmentResult = newValue;

            // Assert
            Assert.AreEqual(newValue, result.SimpleAssessmentResult);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerThree_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            double newValue = random.NextDouble();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new TechnicalInnovationFailureMechanismSectionResult(section);
            var row = new TechnicalInnovationSectionResultRow(result);

            // Call
            row.AssessmentLayerThree = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerThree, row.AssessmentLayerThree.GetAccuracy());
        }

        [Test]
        public void Update_Always_ThrowsNotImplementedException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new TechnicalInnovationFailureMechanismSectionResult(section);
            var row = new TechnicalInnovationSectionResultRow(result);

            // Call
            TestDelegate call = () => row.Update();

            // Assert
            Assert.Throws<NotImplementedException>(call);
        }
    }
}