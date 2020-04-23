﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using Components.Persistence.Stability.Data;
using Core.Common.Base.Geometry;
using Core.Common.Geometry;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.IO.TestUtil;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableStateFactoryTest
    {
        [Test]
        public void Create_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableStateFactory.Create(null, new IdFactory(), new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Create_IdFactoryNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfileUnderSurfaceLine>();
            mocks.ReplayAll();

            // Call
            void Call() => PersistableStateFactory.Create(soilProfile, null, new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idFactory", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfileUnderSurfaceLine>();
            mocks.ReplayAll();

            // Call
            void Call() => PersistableStateFactory.Create(soilProfile, new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_WithValidData_ReturnsPersistableStates()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(
                new TestHydraulicBoundaryLocation());
            IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile = calculation.InputParameters.SoilProfileUnderSurfaceLine;
            soilProfile.Layers.ForEachElementDo(l => l.Data.UsePop = true);

            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();

            PersistableGeometryFactory.Create(soilProfile, idFactory, registry);

            // Call
            IEnumerable<PersistableState> states = PersistableStateFactory.Create(soilProfile, idFactory, registry);

            // Assert
            Assert.AreEqual(2, states.Count());

            IEnumerable<MacroStabilityInwardsSoilLayer2D> layersWithPop = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(soilProfile.Layers)
                                                                                                                        .Where(l => l.Data.UsePop);

            for (var i = 0; i < states.Count(); i++)
            {
                PersistableState state = states.ElementAt(i);

                Assert.IsNotNull(state.Id);
                CollectionAssert.IsEmpty(state.StateLines);

                Assert.AreEqual(layersWithPop.Count(), state.StatePoints.Count());

                for (var j = 0; j < layersWithPop.Count(); j++)
                {
                    MacroStabilityInwardsSoilLayer2D layerWithPop = layersWithPop.ElementAt(j);
                    PersistableStatePoint statePoint = state.StatePoints.ElementAt(j);

                    Assert.IsNotNull(statePoint.Id);
                    Assert.IsEmpty(statePoint.Label);
                    Assert.IsNotNull(statePoint.LayerId);
                    Assert.IsTrue(statePoint.IsProbabilistic);

                    Point2D interiorPoint = AdvancedMath2D.GetPolygonInteriorPoint(layerWithPop.OuterRing.Points, layerWithPop.NestedLayers.Select(layers => layers.OuterRing.Points));
                    Assert.AreEqual(interiorPoint.X, statePoint.Point.X);
                    Assert.AreEqual(interiorPoint.Y, statePoint.Point.Z);

                    Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(layerWithPop.Data).GetDesignValue(), statePoint.Stress.Pop);
                    PersistableDataModelTestHelper.AssertStochasticParameter(layerWithPop.Data.Pop, statePoint.Stress.PopStochasticParameter);
                    Assert.AreEqual(PersistableStateType.Pop, statePoint.Stress.StateType);
                }
            }

            AssertRegistry(registry, new []
            {
                MacroStabilityInwardsExportStageType.Daily,
                MacroStabilityInwardsExportStageType.Extreme
            }, states);
        }

        private static void AssertRegistry(MacroStabilityInwardsExportRegistry registry, MacroStabilityInwardsExportStageType[] stages, IEnumerable<PersistableState> states)
        {
            Assert.AreEqual(stages.Length, registry.States.Count);

            for (var i = 0; i < stages.Length; i++)
            {
                PersistableState state = states.ElementAt(i);
                Assert.AreEqual(registry.States[stages[i]], state.Id);
            }
        }
    }
}