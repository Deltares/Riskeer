// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;

namespace Ringtoets.Integration.TestUtils.Test
{
    [TestFixture]
    public class DataUpdateHelperTest
    {
        private AssessmentSection dikeSection;

        [SetUp]
        public void SetUp()
        {
            dikeSection = new AssessmentSection(AssessmentSectionComposition.Dike);
        }

        [Test]
        public void ImportPipingStochasticSoilModels_ValidAssessmentSection_AddsThreeSoilModelsWithProfiles()
        {
            // Call
            DataUpdateHelper.UpdatePipingStochasticSoilModels(dikeSection);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "PK001_0001_Piping",
                "PK001_0002_Piping",
                "PK001_0003_Piping"
            }, dikeSection.PipingFailureMechanism.StochasticSoilModels.Select(sm => sm.Name));
            CollectionAssert.AreEqual(new[]
            {
                1,
                2,
                1
            }, dikeSection.PipingFailureMechanism.StochasticSoilModels.Select(sm => sm.StochasticSoilProfiles.Count));
            CollectionAssert.AreEqual(new[]
            {
                0.5,
                0.5,
                0.5,
                1.0
            }, dikeSection.PipingFailureMechanism.StochasticSoilModels.SelectMany(sm => sm.StochasticSoilProfiles.Select(sp => sp.Probability)));
            CollectionAssert.AreEqual(new[]
            {
                "W1-6_0_1D1",
                "W1-6_0_1D1",
                "W1-7_0_1D1",
                "W1-7_0_1D1"
            }, dikeSection.PipingFailureMechanism.StochasticSoilModels.SelectMany(sm => sm.StochasticSoilProfiles.Select(sp => sp.SoilProfile.Name)));
        }

        [Test]
        public void UpdateMacroStabilityInwardsStochasticSoilModels_ValidAssessmentSection_AddsThreeSoilModelsWithProfiles()
        {
            // Call
            DataUpdateHelper.UpdateMacroStabilityInwardsStochasticSoilModels(dikeSection);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "PK001_0001_Piping",
                "PK001_0002_Piping",
                "PK001_0003_Piping"
            }, dikeSection.MacroStabilityInwards.StochasticSoilModels.Select(sm => sm.Name));
            CollectionAssert.AreEqual(new[]
            {
                1,
                2,
                1
            }, dikeSection.MacroStabilityInwards.StochasticSoilModels.Select(sm => sm.StochasticSoilProfiles.Count));
            CollectionAssert.AreEqual(new[]
            {
                0.5,
                0.5,
                0.5,
                1.0
            }, dikeSection.MacroStabilityInwards.StochasticSoilModels.SelectMany(sm => sm.StochasticSoilProfiles.Select(sp => sp.Probability)));
            CollectionAssert.AreEqual(new[]
            {
                "W1-6_0_1D1",
                "W1-6_0_1D1",
                "W1-7_0_1D1",
                "W1-7_0_1D1"
            }, dikeSection.MacroStabilityInwards.StochasticSoilModels.SelectMany(sm => sm.StochasticSoilProfiles.Select(sp => sp.SoilProfile.Name)));
        }
    }
}