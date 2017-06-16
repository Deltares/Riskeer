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

using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Service.Test
{
    [TestFixture]
    public class MacroStabilityInwardsInputServiceTest
    {
        [Test]
        public void SetMatchingStochasticSoilModel_SurfaceLineOverlappingSingleSoilModel_SetsSoilModel()
        {
            // Setup
            var soilModel = new StochasticSoilModel(1, "A");
            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

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
            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());
            var soilModel1 = new StochasticSoilModel(1, "A");
            var soilModel2 = new StochasticSoilModel(2, "C");

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
            var nonOverlappingSoilModel = new StochasticSoilModel(1, "A");
            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                StochasticSoilModel = nonOverlappingSoilModel
            };

            var soilModel1 = new StochasticSoilModel(1, "A");
            var soilModel2 = new StochasticSoilModel(2, "C");

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
            var soilProfile = new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1);

            var soilModel = new StochasticSoilModel(1, "A");
            soilModel.StochasticSoilProfiles.Add(soilProfile);

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
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
            var soilModel = new StochasticSoilModel(1, "A");
            soilModel.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 1),
                new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 2)
            });
            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
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
            var soilProfile = new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1);

            var soilModel = new StochasticSoilModel(1, "A");
            soilModel.StochasticSoilProfiles.Add(soilProfile);

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
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