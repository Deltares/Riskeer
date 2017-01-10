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

using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Service.Test
{
    [TestFixture]
    public class PipingInputServiceTest
    {
        [Test]
        public void SetMatchingStochasticSoilModel_SurfaceLineOverlappingSingleSoilModel_SetsSoilModel()
        {
            // Setup
            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel.StochasticSoilProfiles.Add(new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D, 1)
            });

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };

            // Call
            PipingInputService.SetMatchingStochasticSoilModel(pipingInput, new[]
            {
                soilModel
            });

            // Assert
            Assert.AreEqual(soilModel, pipingInput.StochasticSoilModel);
        }

        [Test]
        public void SetMatchingStochasticSoilModel_SurfaceLineOverlappingMultipleSoilModels_DoesNotSetModel()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };
            var soilModel1 = new StochasticSoilModel(1, "A", "B");
            soilModel1.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel1.StochasticSoilProfiles.Add(new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D, 1)
            });
            var soilModel2 = new StochasticSoilModel(2, "C", "D");
            soilModel2.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel2.StochasticSoilProfiles.Add(new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D, 1)
            });

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
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });
            var nonOverlappingSoilModel = new StochasticSoilModel(1, "A", "B");
            nonOverlappingSoilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                StochasticSoilModel = nonOverlappingSoilModel
            };

            var soilModel1 = new StochasticSoilModel(1, "A", "B");
            soilModel1.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel1.StochasticSoilProfiles.Add(new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D, 1)
            });
            var soilModel2 = new StochasticSoilModel(2, "C", "D");
            soilModel2.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            soilModel2.StochasticSoilProfiles.Add(new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D, 1)
            });

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
            var soilProfile = new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1);

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.StochasticSoilProfiles.Add(soilProfile);

            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                StochasticSoilModel = soilModel
            };

            // Call
            PipingInputService.SyncStochasticSoilProfileWithStochasticSoilModel(pipingInput);

            // Assert
            Assert.AreEqual(soilProfile, pipingInput.StochasticSoilProfile);
        }

        [Test]
        public void SyncStochasticSoilProfileWithStochasticSoilModel_MultipleStochasticSoilProfilesInStochasticSoilModel_DoesNotSetStochasticSoilProfile()
        {
            // Setup
            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 1),
                new StochasticSoilProfile(1.1, SoilProfileType.SoilProfile1D, 2)
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
            var soilProfile = new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1);

            var soilModel = new StochasticSoilModel(1, "A", "B");
            soilModel.StochasticSoilProfiles.Add(soilProfile);

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