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
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.Views;

namespace Ringtoets.HeightStructures.Forms.Test.Views
{
    [TestFixture]
    public class HeightStructuresFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithoutSectionResult_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HeightStructuresFailureMechanismSectionResultRow(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Constructor_WithSectionResult_PropertiesFromSectionAndResult()
        {
            // Setup
            var section = CreateSection();
            var result = new HeightStructuresFailureMechanismSectionResult(section);

            // Call
            var row = new HeightStructuresFailureMechanismSectionResultRow(result);

            // Assert
            Assert.AreEqual(section.Name, row.Name);
            Assert.AreEqual(result.AssessmentLayerOne, row.AssessmentLayerOne);
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);
            Assert.AreEqual(result.AssessmentLayerThree, row.AssessmentLayerThree);

            Assert.IsTrue(TypeUtils.HasTypeConverter<HeightStructuresFailureMechanismSectionResultRow,
                              FailureMechanismSectionResultNoProbabilityValueDoubleConverter>(
                                  r => r.AssessmentLayerTwoA));
            Assert.IsTrue(TypeUtils.HasTypeConverter<HeightStructuresFailureMechanismSectionResultRow,
                              NoValueRoundedDoubleConverter>(
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
            var result = new HeightStructuresFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new HeightStructuresFailureMechanismSectionResultRow(result);

            // Call
            row.AssessmentLayerOne = newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerOne);

            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerThree_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            var newValue = random.NextDouble();
            var section = CreateSection();
            var result = new HeightStructuresFailureMechanismSectionResult(section);
            var row = new HeightStructuresFailureMechanismSectionResultRow(result);

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