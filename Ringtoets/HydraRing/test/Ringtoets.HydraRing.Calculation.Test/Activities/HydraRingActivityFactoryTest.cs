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
using Core.Common.Base.Service;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;

namespace Ringtoets.HydraRing.Calculation.Test.Activities
{
    [TestFixture]
    public class HydraRingActivityFactoryTest
    {
        #region ExceedanceProbabilityCalculationInput

        [Test]
        public void CreateExceedanceProbabilityCalculationInput_InputEmptyName_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var exceedanceProbabilityCalculationInput = mocks.StrictMock<ExceedanceProbabilityCalculationInput>(1);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HydraRingActivityFactory.Create("", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, exceedanceProbabilityCalculationInput, output => { });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test, "Name should be set.");
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void CreateExceedanceProbabilityCalculationInput_InputEmptyHlcdDirectory_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var exceedanceProbabilityCalculationInput = mocks.StrictMock<ExceedanceProbabilityCalculationInput>(1);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HydraRingActivityFactory.Create("name", "", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, exceedanceProbabilityCalculationInput, output => { });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test, "HLCD directory should be set.");
            Assert.AreEqual("hlcdDirectory", exception.ParamName);
        }

        [Test]
        public void CreateExceedanceProbabilityCalculationInput_InputEmptyRingId_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var exceedanceProbabilityCalculationInput = mocks.StrictMock<ExceedanceProbabilityCalculationInput>(1);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HydraRingActivityFactory.Create("name", "hlcdDirectory", "", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, exceedanceProbabilityCalculationInput, output => { });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test, "Ring id should be set.");
            Assert.AreEqual("ringId", exception.ParamName);
        }

        [Test]
        public void CreateExceedanceProbabilityCalculationInput_InputHandleCalculationInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HydraRingActivityFactory.Create("name", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, (ExceedanceProbabilityCalculationInput) null, output => { });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test, "Calculation input should be set.");
            Assert.AreEqual("hydraRingCalculationInput", exception.ParamName);
        }

        [Test]
        public void CreateExceedanceProbabilityCalculationInput_InputHandleCalculationOutputActionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var exceedanceProbabilityCalculationInput = mocks.StrictMock<ExceedanceProbabilityCalculationInput>(1);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HydraRingActivityFactory.Create("name", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, exceedanceProbabilityCalculationInput, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test, "Handle calculation output action should be set.");
            Assert.AreEqual("action", exception.ParamName);
        }

        [Test]
        public void CreateExceedanceProbabilityCalculationInput_CalculationInputValidParameters_ReturnsExpectedExceedanceProbabilityCalculationActivity()
        {
            // Setup
            var mocks = new MockRepository();
            var exceedanceProbabilityCalculationInput = mocks.StrictMock<ExceedanceProbabilityCalculationInput>(1);

            mocks.ReplayAll();

            // Call
            var activity = HydraRingActivityFactory.Create("name", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, exceedanceProbabilityCalculationInput, output => { });

            // Assert
            Assert.IsInstanceOf<ExceedanceProbabilityCalculationActivity>(activity);
            Assert.AreEqual("name", activity.Name);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        #endregion

        #region TargetProbabilityCalculationInput

        [Test]
        public void CreateTargetProbabilityCalculationInput_InputEmptyName_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var targetProbabilityCalculationInput = mocks.StrictMock<TargetProbabilityCalculationInput>(1, 2.2);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HydraRingActivityFactory.Create("", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, targetProbabilityCalculationInput, output => { });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test, "Name should be set.");
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void CreateTargetProbabilityCalculationInput_InputEmptyHlcdDirectory_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var targetProbabilityCalculationInput = mocks.StrictMock<TargetProbabilityCalculationInput>(1, 2.2);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HydraRingActivityFactory.Create("name", "", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, targetProbabilityCalculationInput, output => { });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test, "HLCD directory should be set.");
            Assert.AreEqual("hlcdDirectory", exception.ParamName);
        }

        [Test]
        public void CreateTargetProbabilityCalculationInput_InputEmptyRingId_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var targetProbabilityCalculationInput = mocks.StrictMock<TargetProbabilityCalculationInput>(1, 2.2);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HydraRingActivityFactory.Create("name", "hlcdDirectory", "", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, targetProbabilityCalculationInput, output => { });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test, "Ring id should be set.");
            Assert.AreEqual("ringId", exception.ParamName);
        }

        [Test]
        public void CreateTargetProbabilityCalculationInput_InputHandleCalculationInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HydraRingActivityFactory.Create("name", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, (TargetProbabilityCalculationInput) null, output => { });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test, "Calculation input should be set.");
            Assert.AreEqual("hydraRingCalculationInput", exception.ParamName);
        }

        [Test]
        public void CreateTargetProbabilityCalculationInput_InputHandleCalculationOutputActionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var targetProbabilityCalculationInput = mocks.StrictMock<TargetProbabilityCalculationInput>(1, 2.2);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HydraRingActivityFactory.Create("name", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, targetProbabilityCalculationInput, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test, "Handle calculation output action should be set.");
            Assert.AreEqual("action", exception.ParamName);
        }

        [Test]
        public void CreateTargetProbabilityCalculationInput_CalculationInputValidParameters_ReturnsExpectedTargetProbabilityCalculationActivity()
        {
            // Setup
            var mocks = new MockRepository();
            var targetProbabilityCalculationInput = mocks.StrictMock<TargetProbabilityCalculationInput>(1, 2.2);

            mocks.ReplayAll();

            // Call
            var activity = HydraRingActivityFactory.Create("name", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, targetProbabilityCalculationInput, output => { });

            // Assert
            Assert.IsInstanceOf<TargetProbabilityCalculationActivity>(activity);
            Assert.AreEqual("name", activity.Name);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        #endregion
    }
}