﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using System.Drawing;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Gui;
using Core.Gui.Commands;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;
using Riskeer.Integration.IO.Importers;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.ImportInfos
{
    [TestFixture]
    public class HydraulicBoundaryDataContextImportInfoTest
    {
        private MockRepository mocks;
        private ImportInfo importInfo;
        private RiskeerPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            IGui gui = StubFactory.CreateGuiStub(mocks);
            gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            mocks.ReplayAll();

            plugin = new RiskeerPlugin
            {
                Gui = gui
            };
            importInfo = plugin.GetImportInfos().First(i => i.DataType == typeof(HydraulicBoundaryDataContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Call
            string name = importInfo.Name;

            // Assert
            Assert.AreEqual("HLCD bestand", name);
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Call
            string category = importInfo.Category;

            // Assert
            Assert.AreEqual("Algemeen", category);
        }

        [Test]
        public void Image_Always_ReturnExpectedIcon()
        {
            // Call
            Image image = importInfo.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.DatabaseIcon, image);
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnExpectedFileFilter()
        {
            // Call
            FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;

            // Assert
            Assert.AreEqual("HLCD bestand (*.sqlite)|*.sqlite", fileFilterGenerator.Filter);
        }

        [Test]
        public void CreateFileImporter_Always_ReturnFileImporter()
        {
            // Setup
            mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            mocks.ReplayAll();

            var importTarget = new HydraulicBoundaryDataContext(new HydraulicBoundaryData(), new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            IFileImporter importer = importInfo.CreateFileImporter(importTarget, "");

            // Assert
            Assert.IsInstanceOf<HydraulicLocationConfigurationDatabaseImporter>(importer);
            mocks.VerifyAll();
        }
    }
}