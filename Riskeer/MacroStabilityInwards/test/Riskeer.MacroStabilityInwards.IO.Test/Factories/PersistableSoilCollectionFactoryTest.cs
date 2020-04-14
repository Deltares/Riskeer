using System;
using System.Collections.Generic;
using System.Linq;
using Components.Persistence.Stability.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Riskeer.MacroStabilityInwards.IO.Factories;
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
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer>>();
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
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer>>();
            mocks.ReplayAll();

            // Call
            void Call() => PersistableSoilCollectionFactory.Create(soilProfile, new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_WithValidData_ReturnsPersistableSoilCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile2D soilProfile = MacroStabilityInwardsSoilProfile2DTestFactory.CreateMacroStabilityInwardsSoilProfile2D();
            var registry = new MacroStabilityInwardsExportRegistry();

            // Call
            PersistableSoilCollection soilCollection = PersistableSoilCollectionFactory.Create(soilProfile, new IdFactory(), registry);

            // Assert
            IEnumerable<MacroStabilityInwardsSoilLayer2D> originalLayers = soilProfile.Layers;
            IEnumerable<PersistableSoil> actualSoils = soilCollection.Soils;

            Assert.AreEqual(originalLayers.Count(), actualSoils.Count());
            Assert.AreEqual(originalLayers.Count(), registry.Soils.Count);

            for (var i = 0; i < originalLayers.Count(); i++)
            {
                MacroStabilityInwardsSoilLayer2D layer = originalLayers.ElementAt(i);
                PersistableSoil soil = actualSoils.ElementAt(i);

                Assert.IsNotNull(soil.Id);
                Assert.AreEqual(layer.Data.MaterialName, soil.Name);
                Assert.AreEqual($"{layer.Data.MaterialName}-{soil.Id}", soil.Code);
                Assert.IsTrue(soil.IsProbabilistic);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(layer.Data).GetDesignValue(), soil.Cohesion);

                Assert.IsFalse(soil.CohesionAndFrictionAngleCorrelated);
                Assert.IsFalse(soil.ShearStrengthRatioAndShearStrengthExponentCorrelated);

                KeyValuePair<IMacroStabilityInwardsSoilLayer, string> registrySoil = registry.Soils.ElementAt(i);
                Assert.AreSame(layer, registrySoil.Key);
                Assert.AreEqual(soil.Id, registrySoil.Value);
            }
        }
    }
}