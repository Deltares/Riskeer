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
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Primitives.TestUtil;

namespace Riskeer.Piping.Service.Test
{
    [TestFixture]
    public class PipingInputServiceTest
    {
        [Test]
        public void SetMatchingStochasticSoilModel_SurfaceLineOverlappingSingleSoilModel_SetsSoilModel()
        {
            // Setup
            PipingStochasticSoilModel soilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel();
            var pipingInput = new PipingInput(new GeneralPipingInput());

            // Call
            PipingInputService.SetMatchingStochasticSoilModel(pipingInput, new[]
            {
                soilModel
            });

            // Assert
            Assert.AreSame(soilModel, pipingInput.StochasticSoilModel);
        }

        [Test]
        public void SetMatchingStochasticSoilModel_SurfaceLineOverlappingMultipleSoilModels_DoesNotSetModel()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());
            PipingStochasticSoilModel soilModel1 = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("A");
            PipingStochasticSoilModel soilModel2 = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("C");

            // Call
            PipingInputService.SetMatchingStochasticSoilModel(pipingInput, new[]
            {
                soilModel1,
                soilModel2
            });

            // Assert
            Assert.IsNull(pipingInput.StochasticSoilModel);
        }

        [Test]
        public void SetMatchingStochasticSoilModel_CurrentSoilModelNotInOverlappingMultipleSoilModels_ClearsModel()
        {
            // Setup
            PipingStochasticSoilModel nonOverlappingSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel();
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                StochasticSoilModel = nonOverlappingSoilModel
            };

            PipingStochasticSoilModel soilModel1 = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("A");
            PipingStochasticSoilModel soilModel2 = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("C");

            // Call
            PipingInputService.SetMatchingStochasticSoilModel(pipingInput, new[]
            {
                soilModel1,
                soilModel2
            });

            // Assert
            Assert.IsNull(pipingInput.StochasticSoilModel);
        }

        [Test]
        public void SyncStochasticSoilProfileWithStochasticSoilModel_SingleStochasticSoilProfileInStochasticSoilModel_SetsStochasticSoilProfile()
        {
            // Setup
            var soilProfile = new PipingStochasticSoilProfile(1, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

            PipingStochasticSoilModel soilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("A", new[]
            {
                soilProfile
            });

            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                StochasticSoilModel = soilModel
            };

            // Call
            PipingInputService.SyncStochasticSoilProfileWithStochasticSoilModel(pipingInput);

            // Assert
            Assert.AreSame(soilProfile, pipingInput.StochasticSoilProfile);
        }

        [Test]
        public void SyncStochasticSoilProfileWithStochasticSoilModel_MultipleStochasticSoilProfilesInStochasticSoilModel_DoesNotSetStochasticSoilProfile()
        {
            // Setup
            PipingStochasticSoilModel soilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(new[]
            {
                new PipingStochasticSoilProfile(0.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile()),
                new PipingStochasticSoilProfile(1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
            });

            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                StochasticSoilModel = soilModel
            };

            // Call
            PipingInputService.SyncStochasticSoilProfileWithStochasticSoilModel(pipingInput);

            // Assert
            Assert.IsNull(pipingInput.StochasticSoilProfile);
        }

        [Test]
        public void SyncStochasticSoilProfileWithStochasticSoilModel_SingleStochasticSoilProfileInSoilModelAlreadySet_StochasticSoilProfileDoesNotChange()
        {
            // Setup
            var soilProfile = new PipingStochasticSoilProfile(0.3, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            PipingStochasticSoilModel soilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(new[]
            {
                soilProfile
            });

            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                StochasticSoilModel = soilModel,
                StochasticSoilProfile = soilProfile
            };

            // Call
            PipingInputService.SyncStochasticSoilProfileWithStochasticSoilModel(pipingInput);

            // Assert
            Assert.AreEqual(soilProfile, pipingInput.StochasticSoilProfile);
        }
    }
}