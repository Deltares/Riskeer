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
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.DuneErosion.Data;

namespace Ringtoets.DuneErosion.Service.Test
{
    [TestFixture]
    public class DuneLocationCalculationActivityFactoryTest
    {
        [Test]
        public void CreateCalculationActivities_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => DuneLocationCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                                         assessmentSection,
                                                                                                         double.NaN,
                                                                                                         "A");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculations", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => DuneLocationCalculationActivityFactory.CreateCalculationActivities(Enumerable.Empty<DuneLocationCalculation>(),
                                                                                                         null,
                                                                                                         double.NaN,
                                                                                                         "A");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }
    }
}