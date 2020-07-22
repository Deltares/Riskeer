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

using System;
using System.Collections.Generic;
using System.Linq;
using Components.Persistence.Stability.Data;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;
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
        public void Create_SoilProfileWithMultiplePreconsolidationStressesOnOneLayer_ReturnsPersistableStates()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(
                new TestHydraulicBoundaryLocation());
            calculation.InputParameters.StochasticSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D(new[]
            {
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress(new Point2D(2, 1)),
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress(new Point2D(2, 2))
            });

            IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile = calculation.InputParameters.SoilProfileUnderSurfaceLine;

            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();

            PersistableGeometryFactory.Create(soilProfile, idFactory, registry);

            // Call
            IEnumerable<PersistableState> states = PersistableStateFactory.Create(soilProfile, idFactory, registry);

            // Assert
            Assert.AreEqual(1, states.Count());

            PersistableState state = states.First();

            Assert.IsNotNull(state.Id);
            CollectionAssert.IsEmpty(state.StateLines);
            CollectionAssert.IsEmpty(state.StatePoints);
        }

        [Test]
        public void Create_SoilProfileWithPOPAndPreconsolidationStressOnOneLayer_ReturnsPersistableStates()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D(new[]
            {
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress(new Point2D(2, 1))
            });

            IMacroStabilityInwardsSoilLayer firstLayer = stochasticSoilProfile.SoilProfile.Layers.First();
            firstLayer.Data.UsePop = true;
            firstLayer.Data.Pop = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 1,
                CoefficientOfVariation = (RoundedDouble) 2
            };
            calculation.InputParameters.StochasticSoilProfile = stochasticSoilProfile;

            IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile = calculation.InputParameters.SoilProfileUnderSurfaceLine;

            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();

            PersistableGeometryFactory.Create(soilProfile, idFactory, registry);

            // Call
            IEnumerable<PersistableState> states = PersistableStateFactory.Create(soilProfile, idFactory, registry);

            // Assert
            Assert.AreEqual(1, states.Count());

            PersistableState state = states.First();

            Assert.IsNotNull(state.Id);
            CollectionAssert.IsEmpty(state.StateLines);
            CollectionAssert.IsEmpty(state.StatePoints);
        }

        [Test]
        public void Create_WithValidData_ReturnsPersistableStates()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D(new[]
            {
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress(new Point2D(2, 1))
            });

            IMacroStabilityInwardsSoilLayer lastLayer = stochasticSoilProfile.SoilProfile.Layers.Last();
            lastLayer.Data.UsePop = true;
            lastLayer.Data.Pop = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 1,
                CoefficientOfVariation = (RoundedDouble) 2
            };
            calculation.InputParameters.StochasticSoilProfile = stochasticSoilProfile;

            IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile = calculation.InputParameters.SoilProfileUnderSurfaceLine;

            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();
            PersistableGeometryFactory.Create(soilProfile, idFactory, registry);

            // Call
            IEnumerable<PersistableState> states = PersistableStateFactory.Create(soilProfile, idFactory, registry);

            // Assert
            PersistableDataModelTestHelper.AssertStates(soilProfile, states);
            PersistableState state = states.First();
            Assert.AreEqual(registry.States[MacroStabilityInwardsExportStageType.Daily], state.Id);
        }
    }
}