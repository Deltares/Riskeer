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

using Core.Common.Base.Geometry;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Plugin.ImportInfos;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.ImportInfos
{
    [TestFixture]
    public class PipingImportInfoFactoryTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateFailureMechanismSectionsImportInfo_WithData_ReturnsImportInfo(bool isEnabled)
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            if (isEnabled)
            {
                assessmentSection.ReferenceLine.SetGeometry(new[]
                {
                    new Point2D(0, 0)
                });
            }

            // Call
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = PipingImportInfoFactory.CreateFailureMechanismSectionsImportInfo();

            // Assert
            Assert.AreEqual("Vakindeling", importInfo.Name);
            Assert.AreEqual("Algemeen", importInfo.Category);

            FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;
            Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);

            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SectionsIcon, importInfo.Image);

            var context = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);
            Assert.AreEqual(isEnabled, importInfo.IsEnabled(context));
            Assert.IsInstanceOf<FailureMechanismSectionsImporter>(importInfo.CreateFileImporter(context, ""));
        }
    }
}