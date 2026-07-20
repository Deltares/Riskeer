// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Plugin;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ImportInfos;
using Riskeer.Common.IO.FileImporters;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Plugin.FileImporter;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.ImportInfos
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismSectionsContextImportInfoTest
    {
        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Setup
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                plugin.Gui = gui;

                ImportInfo importInfo = GetImportInfo();

                // Call
                string name = importInfo.Name;

                // Assert
                Assert.AreEqual("Vakindeling", name);
            }
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Setup
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                plugin.Gui = gui;

                ImportInfo importInfo = GetImportInfo();

                // Call
                string category = importInfo.Category;

                // Assert
                Assert.AreEqual("Algemeen", category);
            }
        }

        [Test]
        public void Image_Always_ReturnExpectedIcon()
        {
            // Setup
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                plugin.Gui = gui;

                ImportInfo importInfo = GetImportInfo();

                // Call
                Image image = importInfo.Image;

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SectionsIcon, image);
            }
        }

        [Test]
        public void IsEnabled_ReferenceLineWithoutGeometry_ReturnFalse()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(new ReferenceLine());
            var context = new MacroStabilityInwardsFailureMechanismSectionsContext(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            ImportInfo importInfo = GetImportInfo();

            // Call
            bool isEnabled = importInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
        }

        [Test]
        public void IsEnabled_ReferenceLineWithGeometry_ReturnTrue()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(ReferenceLineTestFactory.CreateReferenceLineWithGeometry());
            var context = new MacroStabilityInwardsFailureMechanismSectionsContext(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            ImportInfo importInfo = GetImportInfo();

            // Call
            bool isEnabled = importInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnExpectedFileFilter()
        {
            // Setup
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                plugin.Gui = gui;

                ImportInfo importInfo = GetImportInfo();

                // Call
                FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);
            }
        }

        [Test]
        public void CreateFileImporter_WithValidData_ReturnsFileImporter()
        {
            // Setup
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(new ReferenceLine());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var context = new MacroStabilityInwardsFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                plugin.Gui = gui;
                ImportInfo importInfo = GetImportInfo();

                // Call
                IFileImporter importer = importInfo.CreateFileImporter(context, string.Empty);

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionsImporter>(importer);
            }
        }

        private static ImportInfo GetImportInfo()
        {
            return RiskeerImportInfoFactory.CreateFailureMechanismSectionsImportInfo<MacroStabilityInwardsFailureMechanismSectionsContext>(c => new MacroStabilityInwardsFailureMechanismSectionReplaceStrategy((MacroStabilityInwardsFailureMechanism) c.WrappedData));
        }
    }
}