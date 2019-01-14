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
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.IO.Handlers;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.TestUtil;

namespace Ringtoets.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class HydraulicLocationConfigurationDatabaseImportHandlerTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin);
        private static readonly string testDataDirectory = Path.Combine(testDataPath, nameof(HydraulicLocationConfigurationDatabaseImportHandler));

        [Test]
        public void Constructor_ViewParentNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new HydraulicLocationConfigurationDatabaseImportHandler(null, updateHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("viewParent", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_UpdateHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IWin32Window>();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("updateHandler", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IWin32Window>();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            var importHandler = new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, updateHandler);

            // Assert
            Assert.IsInstanceOf<IHydraulicLocationConfigurationDatabaseImportHandler>(importHandler);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNewFilePathSet_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IWin32Window>();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            var importHandler = new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, updateHandler);

            // Call
            TestDelegate call = () => importHandler.OnNewFilePathSet(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNewFilePathSet_HlcdFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IWin32Window>();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            var importHandler = new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, updateHandler);

            // Call
            TestDelegate call = () => importHandler.OnNewFilePathSet(new HydraulicBoundaryDatabase(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hlcdFilePath", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNewFilePathSet_WithValidFilePath_SetsHydraulicLocationConfigurationDatabaseSettings()
        {
            // Setup
            string newHlcdFilePath = Path.Combine(testDataDirectory, "hlcdWithScenarioInformation.sqlite");

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection,
                                                             Path.Combine(testDataDirectory, "complete.sqlite"));

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IWin32Window>();
            var updateHandler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            updateHandler.Expect(uh => uh.InquireConfirmation()).Return(true);
            updateHandler.Expect(uh => uh.Update(Arg<HydraulicBoundaryDatabase>.Is.Same(hydraulicBoundaryDatabase),
                                                 Arg<ReadHydraulicLocationConfigurationDatabaseSettings>.Is.NotNull,
                                                 Arg<string>.Is.Equal(newHlcdFilePath)))
                         .WhenCalled(invoke =>
                         {
                             Assert.AreEqual(1, (IEnumerable<ReadHydraulicLocationConfigurationDatabaseSettings>) invoke.Arguments[1]);
                         });
            mocks.ReplayAll();

            var importHandler = new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, updateHandler);

            // Call
            importHandler.OnNewFilePathSet(hydraulicBoundaryDatabase, newHlcdFilePath);

            // Assert
            mocks.VerifyAll();
        }
    }
}