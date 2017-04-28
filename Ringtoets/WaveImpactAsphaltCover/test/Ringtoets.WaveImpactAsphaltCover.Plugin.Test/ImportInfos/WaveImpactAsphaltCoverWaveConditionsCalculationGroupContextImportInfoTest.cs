﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Drawing;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Revetment.IO.Importers;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;

namespace Ringtoets.WaveImpactAsphaltCover.Plugin.Test.ImportInfos
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationGroupContextImportInfoTest
    {
        private ImportInfo importInfo;
        private WaveImpactAsphaltCoverPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new WaveImpactAsphaltCoverPlugin();
            importInfo = plugin.GetImportInfos().First(i => i.DataType == typeof(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext));
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
            Assert.AreEqual("Ringtoets berekeningenconfiguratie", name);
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
            TestHelper.AssertImagesAreEqual(Resources.GeneralFolderIcon, image);
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnExpectedFileFilter()
        {
            // Call
            FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;

            // Assert
            Assert.AreEqual("Ringtoets berekeningenconfiguratie (*.xml)|*.xml", fileFilterGenerator.Filter);
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
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(new CalculationGroup(),
                                                                                          failureMechanism,
                                                                                          assessmentSection);

            // Call
            IFileImporter importer = importInfo.CreateFileImporter(context, "");

            // Assert
            Assert.IsInstanceOf<WaveConditionsCalculationConfigurationImporter<WaveImpactAsphaltCoverWaveConditionsCalculation>>(importer);
            mocks.VerifyAll();
        }
    }
}