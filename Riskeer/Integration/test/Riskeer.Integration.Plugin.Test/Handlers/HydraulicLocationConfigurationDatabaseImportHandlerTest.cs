﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using Core.Gui.Forms;
using Core.Gui.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.Integration.TestUtil;

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
            void Call() => new HydraulicLocationConfigurationDatabaseImportHandler(null, updateHandler, new HydraulicBoundaryData());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("viewParent", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_UpdateHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IViewParent>();
            mocks.ReplayAll();

            // Call
            void Call() => new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, null, new HydraulicBoundaryData());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("updateHandler", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IViewParent>();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, updateHandler, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IViewParent>();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            var importHandler = new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, updateHandler, new HydraulicBoundaryData());

            // Assert
            Assert.IsInstanceOf<IHydraulicLocationConfigurationDatabaseImportHandler>(importHandler);
            mocks.VerifyAll();
        }

        [Test]
        public void ImportHydraulicLocationConfigurationSettings_HydraulicLocationConfigurationSettingsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IViewParent>();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            var importHandler = new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, updateHandler, new HydraulicBoundaryData());

            // Call
            void Call() => importHandler.ImportHydraulicLocationConfigurationSettings(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicLocationConfigurationSettings", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ImportHydraulicLocationConfigurationSettings_HlcdFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IViewParent>();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryData = new HydraulicBoundaryData();
            var importHandler = new HydraulicLocationConfigurationDatabaseImportHandler(viewParent, updateHandler, hydraulicBoundaryData);

            // Call
            void Call() => importHandler.ImportHydraulicLocationConfigurationSettings(hydraulicBoundaryData.HydraulicLocationConfigurationSettings, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hlcdFilePath", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ImportHydraulicLocationConfigurationSettings_WithValidFilePath_RunsActivity()
        {
            // Setup
            string newHlcdFilePath = Path.Combine(testDataDirectory, "hlcdWithScenarioInformation.sqlite");

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            DataImportHelper.ImportHydraulicBoundaryData(assessmentSection,
                                                             Path.Combine(testDataDirectory, "complete.sqlite"));

            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;

            var mocks = new MockRepository();
            var updateHandler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Activity closes itself
            };

            using (var viewParent = new TestViewParentForm())
            {
                var importHandler = new HydraulicLocationConfigurationDatabaseImportHandler(
                    viewParent, updateHandler, hydraulicBoundaryData);

                // Call
                importHandler.ImportHydraulicLocationConfigurationSettings(
                    hydraulicBoundaryData.HydraulicLocationConfigurationSettings,
                    newHlcdFilePath);
            }

            // Assert
            mocks.VerifyAll();
        }
    }
}