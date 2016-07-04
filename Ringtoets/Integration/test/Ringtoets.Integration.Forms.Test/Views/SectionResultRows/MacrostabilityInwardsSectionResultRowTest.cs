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

using Rhino.Mocks;

using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultRows
{
    [TestFixture]
    public class MacrostabilityInwardsSectionResultRowTest
    {
        [Test]
        public void Constructor_WithoutSectionResult_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacrostabilityInwardsSectionResultRow(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Constructor_WithSectionResult_PropertiesFromSectionAndResult()
        {
            // Setup
            var section = CreateSection();
            var result = new MacrostabilityInwardsFailureMechanismSectionResult(section);

            // Call
            var row = new MacrostabilityInwardsSectionResultRow(result);

            // Assert
            Assert.AreEqual(section.Name, row.Name);
            Assert.AreEqual(result.AssessmentLayerOne, row.AssessmentLayerOne);
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);
            Assert.AreEqual(result.AssessmentLayerThree, row.AssessmentLayerThree);

            Assert.IsTrue(TypeUtils.HasTypeConverter<MacrostabilityInwardsSectionResultRow,
                  FailureMechanismSectionResultNoProbabilityValueDoubleConverter>(
                      r => r.AssessmentLayerTwoA));
            Assert.IsTrue(TypeUtils.HasTypeConverter<MacrostabilityInwardsSectionResultRow,
                  FailureMechanismSectionResultNoValueRoundedDoubleConverter>(
                      r => r.AssessmentLayerThree));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssessmentLayerOne_AlwaysOnChange_NotifyObserversOfResultAndResultPropertyChanged(bool newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var section = CreateSection();
            var result = new MacrostabilityInwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new MacrostabilityInwardsSectionResultRow(result);

            // Call
            row.AssessmentLayerOne = newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerOne);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(0.5)]
        [TestCase(1e-6)]
        public void AssessmentLayerTwoA_ForValidValues_ResultPropertyChanged(double value)
        {
            // Setup
            var section = CreateSection();
            var result = new MacrostabilityInwardsFailureMechanismSectionResult(section);
            var row = new MacrostabilityInwardsSectionResultRow(result);

            // Call
            row.AssessmentLayerTwoA = value;

            // Assert
            Assert.AreEqual(value, row.AssessmentLayerTwoA);
        }

        [Test]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void AssessmentLayerTwoA_ForInvalidValues_ThrowsArgumentException(double value)
        {
            // Setup
            var section = CreateSection();
            var result = new MacrostabilityInwardsFailureMechanismSectionResult(section);
            var row = new MacrostabilityInwardsSectionResultRow(result);

            // Call
            TestDelegate test = () => row.AssessmentLayerTwoA = value;

            // Assert
            var message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual(RingtoetsCommonDataResources.ArbitraryProbabilityFailureMechanismSectionResult_AssessmentLayerTwoA_Value_needs_to_be_between_0_and_1,
                            message);
        }

        [Test]
        public void AssessmentLayerThree_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            var newValue = random.NextDouble();
            var section = CreateSection();
            var result = new MacrostabilityInwardsFailureMechanismSectionResult(section);
            var row = new MacrostabilityInwardsSectionResultRow(result);

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