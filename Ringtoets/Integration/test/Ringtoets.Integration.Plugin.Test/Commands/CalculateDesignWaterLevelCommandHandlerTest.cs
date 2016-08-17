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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.Commands;
using Ringtoets.Integration.Plugin.Commands;

namespace Ringtoets.Integration.Plugin.Test.Commands
{
    [TestFixture]
    public class CalculateDesignWaterLevelCommandHandlerTest : NUnitFormTest
    {
        private MockRepository mockRepository;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_NullMainWindow_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new CalculateDesignWaterLevelCommandHandler(null, assessmentSectionMock);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            const string expectedParamName = "viewParent";
            Assert.AreEqual(expectedParamName, paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_NullIAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup & Call
            TestDelegate test = () => new CalculateDesignWaterLevelCommandHandler(new Form(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            const string expectedParamName = "assessmentSection";
            Assert.AreEqual(expectedParamName, paramName);
        }

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            var commandHandler = new CalculateDesignWaterLevelCommandHandler(new Form(), assessmentSectionMock);

            // Assert
            Assert.IsInstanceOf<ICalculateDesignWaterLevelCommandHandler>(commandHandler);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_HydraulicDatabaseDoesNotExist_LogsErrorAndDoesNotNotifyObservers()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = "Does not exist"
            };
            assessmentSectionMock.Expect(asm => asm.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase).Repeat.Any();

            var observerMock = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();

            var commandHandler = new CalculateDesignWaterLevelCommandHandler(new Form(), assessmentSectionMock);
            var locations = Enumerable.Empty<HydraulicBoundaryLocation>();

            hydraulicBoundaryDatabase.Attach(observerMock);

            // Call
            Action call = () => commandHandler.CalculateDesignWaterLevels(locations);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith("Berekeningen konden niet worden gestart. ", msgs.First());
            });
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_ValidPathEmptyList_NotifyObserversButNoLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Expect(asm => asm.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase).Repeat.Any();

            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var commandHandler = new CalculateDesignWaterLevelCommandHandler(new Form(), assessmentSectionMock);
            var locations = Enumerable.Empty<HydraulicBoundaryLocation>();

            hydraulicBoundaryDatabase.Attach(observerMock);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            // Call
            Action call = () => commandHandler.CalculateDesignWaterLevels(locations);

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevels_ValidPathOneLocationInTheList_NotifyObserversAndLogsMessages()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };

            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1.0, 1);

            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Expect(asm => asm.Id).Return(null);
            assessmentSectionMock.Expect(asm => asm.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase).Repeat.AtLeastOnce();
            assessmentSectionMock.Expect(asm => asm.FailureMechanismContribution).Return(failureMechanismContribution);

            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            const string hydraulicLocationName = "name";
            var locations = new List<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, hydraulicLocationName, 2, 3)
            };

            hydraulicBoundaryDatabase.Attach(observerMock);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            var commandHandler = new CalculateDesignWaterLevelCommandHandler(new Form(), assessmentSectionMock);

            // Call
            Action call = () => commandHandler.CalculateDesignWaterLevels(locations);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                string expectedName = string.Format("Toetspeil voor locatie {0}", hydraulicLocationName);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", expectedName), msgs.First());
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", expectedName), msgs.Skip(1).First());
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", expectedName), msgs.Skip(2).First());
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", expectedName), msgs.Skip(3).First());
                StringAssert.AreNotEqualIgnoringCase(string.Format("Uitvoeren van '{0}' is mislukt.", expectedName), msgs.Last());
            });
            mockRepository.VerifyAll();
        }
    }
}