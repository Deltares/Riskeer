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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.Forms;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.Views;

namespace Ringtoets.StabilityStoneCover.Forms.Test.Views
{
    [TestFixture]
    public class StabilityStoneCoverSectionResultRowTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);

            // Call
            var row = new StabilityStoneCoverSectionResultRow(result);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<StabilityStoneCoverFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.SimpleAssessmentResult, row.SimpleAssessmentResult);
            Assert.AreEqual(result.DetailedAssessmentResultForFactorizedSignalingNorm, row.DetailedAssessmentResultForFactorizedSignalingNorm);
            Assert.AreEqual(result.DetailedAssessmentResultForSignalingNorm, row.DetailedAssessmentResultForSignalingNorm);
            Assert.AreEqual(result.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm, row.DetailedAssessmentResultForSignalingNorm);
            Assert.AreEqual(result.DetailedAssessmentResultForLowerLimitNorm, row.DetailedAssessmentResultForLowerLimitNorm);
            Assert.AreEqual(result.DetailedAssessmentResultForFactorizedLowerLimitNorm, row.DetailedAssessmentResultForFactorizedLowerLimitNorm);
            Assert.AreEqual(SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertTo(result.TailorMadeAssessmentResult),
                            row.TailorMadeAssessmentResult);
            Assert.AreEqual(result.AssessmentLayerThree, row.AssessmentLayerThree);

            TestHelper.AssertTypeConverter<StabilityStoneCoverSectionResultRow,
                NoValueRoundedDoubleConverter>(nameof(StabilityStoneCoverSectionResultRow.AssessmentLayerThree));
        }

        #region Registration

        [Test]
        public void SimpleAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<SimpleAssessmentValidityOnlyResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new StabilityStoneCoverSectionResultRow(result);

            // Call
            row.SimpleAssessmentResult = newValue;

            // Assert
            Assert.AreEqual(newValue, result.SimpleAssessmentResult);
            mocks.VerifyAll();
        }

        [Test]
        public void DetailedAssessmentResultForFactorizedSignalingNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new StabilityStoneCoverSectionResultRow(result);

            // Call
            row.DetailedAssessmentResultForFactorizedSignalingNorm = newValue;

            // Assert
            Assert.AreEqual(newValue, result.DetailedAssessmentResultForFactorizedSignalingNorm);
            mocks.VerifyAll();
        }

        [Test]
        public void DetailedAssessmentResultForSignalingNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new StabilityStoneCoverSectionResultRow(result);

            // Call
            row.DetailedAssessmentResultForSignalingNorm = newValue;

            // Assert
            Assert.AreEqual(newValue, result.DetailedAssessmentResultForSignalingNorm);
            mocks.VerifyAll();
        }

        [Test]
        public void DetailedAssessmentResultForMechanismSpecificLowerLimitNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new StabilityStoneCoverSectionResultRow(result);

            // Call
            row.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = newValue;

            // Assert
            Assert.AreEqual(newValue, result.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm);
            mocks.VerifyAll();
        }

        [Test]
        public void DetailedAssessmentResultForLowerLimitNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new StabilityStoneCoverSectionResultRow(result);

            // Call
            row.DetailedAssessmentResultForLowerLimitNorm = newValue;

            // Assert
            Assert.AreEqual(newValue, result.DetailedAssessmentResultForLowerLimitNorm);
            mocks.VerifyAll();
        }

        [Test]
        public void DetailedAssessmentResultForFactorizedLowerLimitNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new StabilityStoneCoverSectionResultRow(result);

            // Call
            row.DetailedAssessmentResultForFactorizedLowerLimitNorm = newValue;

            // Assert
            Assert.AreEqual(newValue, result.DetailedAssessmentResultForFactorizedLowerLimitNorm);
            mocks.VerifyAll();
        }

        [Test]
        public void TailorMadeAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<SelectableFailureMechanismSectionAssemblyCategoryGroup>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new StabilityStoneCoverSectionResultRow(result);

            // Call
            row.TailorMadeAssessmentResult = newValue;

            // Assert
            FailureMechanismSectionAssemblyCategoryGroup expectedCategoryGroup = SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertFrom(newValue);
            Assert.AreEqual(expectedCategoryGroup, result.TailorMadeAssessmentResult);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerThree_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            double newValue = random.NextDouble();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            var row = new StabilityStoneCoverSectionResultRow(result);

            // Call
            row.AssessmentLayerThree = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerThree, row.AssessmentLayerThree.GetAccuracy());
        }

        #endregion
    }
}