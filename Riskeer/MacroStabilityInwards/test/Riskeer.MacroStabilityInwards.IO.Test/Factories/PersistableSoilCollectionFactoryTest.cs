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
using System.ComponentModel;
using System.Linq;
using Components.Persistence.Stability.Data;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.IO.TestUtil;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableSoilCollectionFactoryTest
    {
        [Test]
        public void Create_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableSoilCollectionFactory.Create(null, new IdFactory(), new MacroStabilityInwardsExportRegistry());

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
            void Call() => PersistableSoilCollectionFactory.Create(soilProfile, null, new MacroStabilityInwardsExportRegistry());

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
            void Call() => PersistableSoilCollectionFactory.Create(soilProfile, new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhi)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.SuCalculated)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated)]
        public void Create_WithSoilProfile_ReturnsPersistableSoilCollection(MacroStabilityInwardsShearStrengthModel shearStrengthModel)
        {
            // Setup
            var soilProfile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(
                new[]
                {
                    MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D(new[]
                    {
                        MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D()
                    })
                },
                Enumerable.Empty<IMacroStabilityInwardsPreconsolidationStress>());

            var registry = new MacroStabilityInwardsExportRegistry();

            IEnumerable<MacroStabilityInwardsSoilLayer2D> originalLayers = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(soilProfile.Layers);
            originalLayers.ForEachElementDo(layer => layer.Data.ShearStrengthModel = shearStrengthModel);

            // Call
            PersistableSoilCollection soilCollection = PersistableSoilCollectionFactory.Create(soilProfile, new IdFactory(), registry);

            // Assert
            IEnumerable<PersistableSoil> actualSoils = soilCollection.Soils;
            PersistableDataModelTestHelper.AssertPersistableSoils(originalLayers, actualSoils);

            Assert.AreEqual(actualSoils.Count(), registry.Soils.Count);
            for (var i = 0; i < originalLayers.Count(); i++)
            {
                KeyValuePair<MacroStabilityInwardsSoilLayer2D, string> registrySoil = registry.Soils.ElementAt(i);
                Assert.AreSame(originalLayers.ElementAt(i), registrySoil.Key);
                Assert.AreEqual(actualSoils.ElementAt(i).Id, registrySoil.Value);
            }
        }

        [Test]
        public void Create_InvalidShearStrengthType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var registry = new MacroStabilityInwardsExportRegistry();
            var soilProfile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(
                new[]
                {
                    MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D()
                },
                Enumerable.Empty<IMacroStabilityInwardsPreconsolidationStress>());
            const MacroStabilityInwardsShearStrengthModel shearStrengthModel = (MacroStabilityInwardsShearStrengthModel) 99;
            soilProfile.Layers.First().Data.ShearStrengthModel = shearStrengthModel;

            // Call
            void Call() => PersistableSoilCollectionFactory.Create(soilProfile, new IdFactory(), registry);

            // Assert
            string message = $"The value of argument 'shearStrengthModel' ({shearStrengthModel}) is invalid for Enum type '{nameof(MacroStabilityInwardsShearStrengthModel)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
            Assert.AreEqual("shearStrengthModel", exception.ParamName);
        }
    }
}