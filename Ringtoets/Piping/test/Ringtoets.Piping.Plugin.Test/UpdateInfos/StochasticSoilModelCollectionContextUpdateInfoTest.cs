// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.IO.Importers;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.UpdateInfos
{
    [TestFixture]
    public class StochasticSoilModelCollectionContextUpdateInfoTest
    {
        private UpdateInfo updateInfo;
        private PipingPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new PipingPlugin();
            updateInfo = plugin.GetUpdateInfos().First(i => i.DataType == typeof(StochasticSoilModelCollectionContext));
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
            string name = updateInfo.Name;

            // Assert
            Assert.AreEqual("Stochastische ondergrondmodellen", name);
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Call
            string category = updateInfo.Category;

            // Assert
            Assert.AreEqual("Algemeen", category);
        }

        [Test]
        public void Image_Always_ReturnExpectedIcon()
        {
            // Call
            Image image = updateInfo.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingSoilProfileIcon, image);
        }

        [Test]
        public void IsEnabled_ReferenceLineNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = null;
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            var context = new StochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = updateInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_ReferenceLineSet_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            var context = new StochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = updateInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void FileFilter_Always_ReturnExpectedFileFilter()
        {
            // Call
            FileFilterGenerator fileFilter = updateInfo.FileFilterGenerator;

            // Assert
            Assert.AreEqual("D-Soil Model bestand (*.soil)|*.soil", fileFilter.Filter);
        }

        [Test]
        public void VerifyUpdates_CalculationWithoutOutputs_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();

            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.ReplayAll();

            plugin.Gui = gui;

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new PipingCalculationScenario(new GeneralPipingInput()));

            var stochasticSoilModelCollection = new StochasticSoilModelCollection();
            var context = new StochasticSoilModelCollectionContext(stochasticSoilModelCollection, failureMechanism, assessmentSection);

            // Call
            bool requiresUpdateConfirmation = updateInfo.VerifyUpdates(context);

            // Assert
            Assert.IsTrue(requiresUpdateConfirmation);
            mocks.VerifyAll();
        }

        [Test]
        public void CurrentPath_StochasticSoilModelCollectionHasPathSet_ReturnsExpectedPath()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            const string expectedFilePath = "some/path";
            var stochasticSoilModelCollection = new StochasticSoilModelCollection();
            stochasticSoilModelCollection.AddRange(new[]
            {
                new TestStochasticSoilModel()
            }, expectedFilePath);

            var failureMechanism = new PipingFailureMechanism();

            var context = new StochasticSoilModelCollectionContext(stochasticSoilModelCollection, failureMechanism, assessmentSection);

            // Call
            string currentPath = updateInfo.CurrentPath(context);

            // Assert
            Assert.AreEqual(expectedFilePath, currentPath);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateFileImporter_Always_ReturnFileImporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            assessmentSection.ReferenceLine = new ReferenceLine();

            var failureMechanism = new PipingFailureMechanism();

            var importTarget = new StochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection);

            // Call
            IFileImporter importer = updateInfo.CreateFileImporter(importTarget, "");

            // Assert
            Assert.IsInstanceOf<StochasticSoilModelImporter>(importer);
            mocks.VerifyAll();
        }
    }
}