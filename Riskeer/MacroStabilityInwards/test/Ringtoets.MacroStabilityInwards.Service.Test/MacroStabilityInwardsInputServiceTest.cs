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

using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.Service.Test
{
    [TestFixture]
    public class MacroStabilityInwardsInputServiceTest
    {
        [Test]
        public void SetMatchingStochasticSoilModel_SurfaceLineOverlappingSingleSoilModel_SetsSoilModel()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilModel soilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel();
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            MacroStabilityInwardsInputService.SetMatchingStochasticSoilModel(input, new[]
            {
                soilModel
            });

            // Assert
            Assert.AreEqual(soilModel, input.StochasticSoilModel);
        }

        [Test]
        public void SetMatchingStochasticSoilModel_SurfaceLineOverlappingMultipleSoilModels_DoesNotSetModel()
        {
            // Setup
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());
            MacroStabilityInwardsStochasticSoilModel soilModel1 =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("A");
            MacroStabilityInwardsStochasticSoilModel soilModel2 =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("C");

            // Call
            MacroStabilityInwardsInputService.SetMatchingStochasticSoilModel(input, new[]
            {
                soilModel1,
                soilModel2
            });

            // Assert
            Assert.IsNull(input.StochasticSoilModel);
        }

        [Test]
        public void SetMatchingStochasticSoilModel_CurrentSoilModelNotInOverlappingMultipleSoilModels_ClearsModel()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilModel nonOverlappingSoilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("A");
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                StochasticSoilModel = nonOverlappingSoilModel
            };

            MacroStabilityInwardsStochasticSoilModel soilModel1 =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("A");
            MacroStabilityInwardsStochasticSoilModel soilModel2 =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("C");

            // Call
            MacroStabilityInwardsInputService.SetMatchingStochasticSoilModel(input, new[]
            {
                soilModel1,
                soilModel2
            });

            // Assert
            Assert.IsNull(input.StochasticSoilModel);
        }

        [Test]
        public void SyncStochasticSoilProfileWithStochasticSoilModel_SingleStochasticSoilProfileInStochasticSoilModel_SetsStochasticSoilProfile()
        {
            // Setup
            var soilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.3, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D());

            MacroStabilityInwardsStochasticSoilModel soilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("A",
                                                                                                   new[]
                                                                                                   {
                                                                                                       soilProfile
                                                                                                   });

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                StochasticSoilModel = soilModel
            };

            // Call
            MacroStabilityInwardsInputService.SyncStochasticSoilProfileWithStochasticSoilModel(input);

            // Assert
            Assert.AreEqual(soilProfile, input.StochasticSoilProfile);
        }

        [Test]
        public void SyncStochasticSoilProfileWithStochasticSoilModel_MultipleStochasticSoilProfilesInStochasticSoilModel_DoesNotSetStochasticSoilProfile()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilModel soilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("A", new[]
                {
                    new MacroStabilityInwardsStochasticSoilProfile(0.0, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D()),
                    new MacroStabilityInwardsStochasticSoilProfile(1.0, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D())
                });

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                StochasticSoilModel = soilModel
            };

            // Call
            MacroStabilityInwardsInputService.SyncStochasticSoilProfileWithStochasticSoilModel(input);

            // Assert
            Assert.IsNull(input.StochasticSoilProfile);
        }

        [Test]
        public void SyncStochasticSoilProfileWithStochasticSoilModel_SingleStochasticSoilProfileInSoilModelAlreadySet_StochasticSoilProfileDoesNotChange()
        {
            // Setup
            var soilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.3, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D());

            MacroStabilityInwardsStochasticSoilModel soilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("A", new[]
                {
                    soilProfile
                });

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                StochasticSoilModel = soilModel,
                StochasticSoilProfile = soilProfile
            };

            // Call
            MacroStabilityInwardsInputService.SyncStochasticSoilProfileWithStochasticSoilModel(input);

            // Assert
            Assert.AreEqual(soilProfile, input.StochasticSoilProfile);
        }
    }
}