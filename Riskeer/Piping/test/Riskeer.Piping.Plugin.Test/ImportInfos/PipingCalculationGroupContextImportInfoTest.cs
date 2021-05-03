﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.IO.Configurations;
using Riskeer.Piping.Primitives;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.ImportInfos
{
    [TestFixture]
    public class PipingCalculationGroupContextImportInfoTest
    {
        private ImportInfo importInfo;
        private PipingPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new PipingPlugin();
            importInfo = plugin.GetImportInfos().First(i => i.DataType == typeof(PipingCalculationGroupContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Call
            string name = importInfo.Name;

            // Assert
            Assert.AreEqual("Riskeer berekeningenconfiguratie", name);
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
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, image);
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnExpectedFileFilter()
        {
            // Call
            FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;

            // Assert
            Assert.AreEqual("Riskeer berekeningenconfiguratie (*.xml)|*.xml", fileFilterGenerator.Filter);
        }

        [Test]
        public void IsEnabled_Always_ReturnTrue()
        {
            // Call
            bool isEnabled = importInfo.IsEnabled(null);

            // Assert
            Assert.IsTrue(isEnabled);
        }

        [Test]
        public void CreateFileImporter_Always_ReturnFileImporter()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var context = new PipingCalculationGroupContext(new CalculationGroup(),
                                                            null,
                                                            Enumerable.Empty<PipingSurfaceLine>(),
                                                            Enumerable.Empty<PipingStochasticSoilModel>(),
                                                            failureMechanism,
                                                            assessmentSection);

            // Call
            IFileImporter importer = importInfo.CreateFileImporter(context, "");

            // Assert
            Assert.IsInstanceOf<PipingCalculationConfigurationImporter>(importer);
            mocks.VerifyAll();
        }
    }
}