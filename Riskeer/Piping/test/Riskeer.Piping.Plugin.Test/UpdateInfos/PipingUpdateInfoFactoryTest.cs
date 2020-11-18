// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Plugin.UpdateInfos;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.UpdateInfos
{
    [TestFixture]
    public class PipingUpdateInfoFactoryTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateFailureMechanismSectionsImportInfo_WithData_ReturnsImportInfo(bool isEnabled)
        {
            // Setup
            PipingFailureMechanism failureMechanism = isEnabled
                                                          ? TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels()
                                                          : new PipingFailureMechanism();

            var context = new PipingFailureMechanismSectionsContext(failureMechanism, new AssessmentSectionStub());

            // Call
            UpdateInfo<PipingFailureMechanismSectionsContext> updateInfo = PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo();

            // Assert
            Assert.AreEqual("Vakindeling", updateInfo.Name);
            Assert.AreEqual("Algemeen", updateInfo.Category);

            FileFilterGenerator fileFilterGenerator = updateInfo.FileFilterGenerator;
            Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);

            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SectionsIcon, updateInfo.Image);

            Assert.AreEqual(failureMechanism.FailureMechanismSectionSourcePath, updateInfo.CurrentPath(context));
            Assert.AreEqual(isEnabled, updateInfo.IsEnabled(context));
            Assert.IsInstanceOf<FailureMechanismSectionsImporter>(updateInfo.CreateFileImporter(context, ""));
        }
    }
}