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

using System.Linq;
using Core.Common.Base.Data;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.HeightStructures.Forms.PresentationObjects;
using Riskeer.Integration.Data;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects;

namespace Riskeer.Integration.Plugin.Test.StateInfos
{
    [TestFixture]
    public class CalculationsStateInfoTest
    {
        private RiskeerPlugin plugin;
        private StateInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RiskeerPlugin();
            info = plugin.GetStateInfos().First(si => si.Name == "Berekeningen");
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual("\uE91D", info.Symbol);
        }

        [Test]
        public void GetRootData_RiskeerProject_ReturnsExpectedRootData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var project = new RiskeerProject
            {
                AssessmentSections =
                {
                    assessmentSection
                }
            };

            // Call
            object rootData = info.GetRootData(project);

            // Assert
            var rootDataCollection = rootData as object[];
            Assert.IsNotNull(rootDataCollection);
            Assert.AreEqual(10, rootDataCollection.Length);
            Assert.AreEqual(new PipingFailureMechanismContext(assessmentSection.Piping, assessmentSection), rootDataCollection[0]);
            Assert.AreEqual(new GrassCoverErosionInwardsFailureMechanismContext(assessmentSection.GrassCoverErosionInwards, assessmentSection), rootDataCollection[1]);
            Assert.AreEqual(new MacroStabilityInwardsFailureMechanismContext(assessmentSection.MacroStabilityInwards, assessmentSection), rootDataCollection[2]);
            Assert.AreEqual(new StabilityStoneCoverFailureMechanismContext(assessmentSection.StabilityStoneCover, assessmentSection), rootDataCollection[3]);
            Assert.AreEqual(new WaveImpactAsphaltCoverFailureMechanismContext(assessmentSection.WaveImpactAsphaltCover, assessmentSection), rootDataCollection[4]);
            Assert.AreEqual(new GrassCoverErosionOutwardsFailureMechanismContext(assessmentSection.GrassCoverErosionOutwards, assessmentSection), rootDataCollection[5]);
            Assert.AreEqual(new HeightStructuresFailureMechanismContext(assessmentSection.HeightStructures, assessmentSection), rootDataCollection[6]);
            Assert.AreEqual(new ClosingStructuresFailureMechanismContext(assessmentSection.ClosingStructures, assessmentSection), rootDataCollection[7]);
            Assert.AreEqual(new StabilityPointStructuresFailureMechanismContext(assessmentSection.StabilityPointStructures, assessmentSection), rootDataCollection[8]);
            Assert.AreEqual(new DuneErosionFailureMechanismContext(assessmentSection.DuneErosion, assessmentSection), rootDataCollection[9]);
        }

        [Test]
        public void GetRootData_ProjectMock_ReturnsNull()
        {
            // Setup
            var mockRepository = new MockRepository();
            var project = mockRepository.StrictMock<IProject>();

            mockRepository.ReplayAll();

            // Call
            object rootData = info.GetRootData(project);

            // Assert
            Assert.IsNull(rootData);

            mockRepository.VerifyAll();
        }
    }
}