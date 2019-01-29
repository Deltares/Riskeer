// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.Views;

namespace Riskeer.HeightStructures.Forms.Test.Views
{
    [TestFixture]
    public class HeightStructuresScenarioRowTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var section = new FailureMechanismSection("testName", new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            });
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(section);

            // Call
            var row = new HeightStructuresScenarioRow(sectionResult);

            // Assert
            Assert.AreSame(sectionResult.Section.Name, row.Name);
            Assert.AreSame(sectionResult.Calculation, row.Calculation);
            Assert.IsInstanceOf<IScenarioRow<StructuresCalculation<HeightStructuresInput>>>(row);
        }

        [Test]
        public void ParameteredConstructor_SectionResultIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HeightStructuresScenarioRow(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreSame("sectionResult", paramName);
        }

        [Test]
        public void Calculation_SetNewValue_UpdatesSectionResultCalculation()
        {
            // Setup
            var section = new FailureMechanismSection("haha", new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            });
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(section);

            var row = new HeightStructuresScenarioRow(sectionResult);

            var calculation = new StructuresCalculation<HeightStructuresInput>();

            // Call
            row.Calculation = calculation;

            // Assert
            Assert.AreSame(calculation, row.Calculation);
            Assert.AreSame(calculation, sectionResult.Calculation);
        }

        [Test]
        public void Calculation_SetNewValue_NotifyObserversOnSectionResult()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var section = new FailureMechanismSection("testSection", new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            });
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(section);
            sectionResult.Attach(observer);

            var row = new HeightStructuresScenarioRow(sectionResult);

            var calculation = new StructuresCalculation<HeightStructuresInput>();

            // Call
            row.Calculation = calculation;

            // Assert
            mocks.VerifyAll(); // Assert observer is notified
        }
    }
}