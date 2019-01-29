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

using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.IO.Configurations;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.HeightStructures.Plugin.Test.ImportInfos
{
    [TestFixture]
    public class HeightStructuresCalculationGroupContextImportInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new HeightStructuresPlugin())
            {
                // Call
                ImportInfo info = GetImportInfo(plugin);

                // Assert
                Assert.IsNotNull(info.CreateFileImporter);
                Assert.IsNotNull(info.IsEnabled);
                Assert.AreEqual("Ringtoets berekeningenconfiguratie", info.Name);
                Assert.AreEqual("Algemeen", info.Category);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, info.Image);
                Assert.IsNotNull(info.FileFilterGenerator);
            }
        }

        [Test]
        public void CreateFileImporter_Always_ReturnFileImporter()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            var context = new HeightStructuresCalculationGroupContext(new CalculationGroup(),
                                                                      null,
                                                                      new HeightStructuresFailureMechanism(),
                                                                      assessmentSection);

            using (var plugin = new HeightStructuresPlugin())
            {
                ImportInfo info = GetImportInfo(plugin);

                // Call
                IFileImporter fileImporter = info.CreateFileImporter(context, "test");

                // Assert
                Assert.IsInstanceOf<HeightStructuresCalculationConfigurationImporter>(fileImporter);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnFileFilter()
        {
            // Setup
            using (var plugin = new HeightStructuresPlugin())
            {
                ImportInfo info = GetImportInfo(plugin);

                // Call
                FileFilterGenerator fileFilterGenerator = info.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Ringtoets berekeningenconfiguratie (*.xml)|*.xml", fileFilterGenerator.Filter);
            }
        }

        [Test]
        public void IsEnabled_Always_ReturnsTrue()
        {
            // Setup
            using (var plugin = new HeightStructuresPlugin())
            {
                ImportInfo info = GetImportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(null);

                // Assert
                Assert.IsTrue(isEnabled);
            }
        }

        private static ImportInfo GetImportInfo(HeightStructuresPlugin plugin)
        {
            return plugin.GetImportInfos().First(ei => ei.DataType == typeof(HeightStructuresCalculationGroupContext));
        }
    }
}