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
using System.IO;
using System.Windows.Forms;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.Plugin.Handlers;

namespace Riskeer.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class HydraulicLocationConfigurationDatabaseImportHandlerTest : NUnitFormTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Plugin);
        private static readonly string testDataDirectory = Path.Combine(testDataPath, nameof(HydraulicLocationConfigurationDatabaseImportHandler));

        [Test]
        public void Constructor_ViewParentNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new HydraulicLocationConfigurationDatabaseImportHandler(null, updateHandler, new HydraulicBoundaryDatabase());

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
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, null, new HydraulicBoundaryDatabase());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("updateHandler", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IWin32Window>();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, updateHandler, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
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
            var importHandler = new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, updateHandler, new HydraulicBoundaryDatabase());

            // Assert
            Assert.IsInstanceOf<IHydraulicLocationConfigurationDatabaseImportHandler>(importHandler);
            mocks.VerifyAll();
        }

        [Test]
        public void ImportHydraulicLocationConfigurationSettings_HydraulicLocationConfigurationSettingsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IWin32Window>();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            var importHandler = new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, updateHandler, new HydraulicBoundaryDatabase());

            // Call
            TestDelegate call = () => importHandler.ImportHydraulicLocationConfigurationSettings(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicLocationConfigurationSettings", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ImportHydraulicLocationConfigurationSettings_HlcdFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IWin32Window>();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var importHandler = new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, updateHandler, hydraulicBoundaryDatabase);

            // Call
            TestDelegate call = () => importHandler.ImportHydraulicLocationConfigurationSettings(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings,
                                                                                                 null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hlcdFilePath", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ImportHydraulicLocationConfigurationSettings_WithValidFilePath_RunsActivity()
        {
            // Setup
            string newHlcdFilePath = Path.Combine(testDataDirectory, "hlcdWithScenarioInformation.sqlite");

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection,
                                                             Path.Combine(testDataDirectory, "complete.sqlite"));

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IWin32Window>();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Activity closes itself
            };

            var importHandler = new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, updateHandler, hydraulicBoundaryDatabase);

            // Call
            importHandler.ImportHydraulicLocationConfigurationSettings(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings, newHlcdFilePath);

            // Assert
            mocks.VerifyAll();
        }
    }
}