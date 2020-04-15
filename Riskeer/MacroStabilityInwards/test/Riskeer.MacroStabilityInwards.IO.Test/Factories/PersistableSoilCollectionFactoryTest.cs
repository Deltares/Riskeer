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
        public void Create_WithISoilProfile_ThrowsNotSupportedException()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer>>();
            mocks.ReplayAll();

            // Call
            void Call() => PersistableSoilCollectionFactory.Create(soilProfile, new IdFactory(), new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<NotSupportedException>(Call);
            string expectedMessage = $"{soilProfile.GetType().Name} is not supported. " +
                                     $"Supported types are: {nameof(MacroStabilityInwardsSoilProfile1D)} " +
                                     $"and {nameof(MacroStabilityInwardsSoilProfile2D)}.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhi)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.SuCalculated)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated)]
        public void Create_WithSoilProfile1D_ReturnsPersistableSoilCollection(MacroStabilityInwardsShearStrengthModel shearStrengthModel)
        {
            // Setup
            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            var registry = new MacroStabilityInwardsExportRegistry();

            IEnumerable<MacroStabilityInwardsSoilLayer1D> originalLayers = soilProfile.Layers;
            originalLayers.ForEachElementDo(layer => layer.Data.ShearStrengthModel = shearStrengthModel);

            // Call
            PersistableSoilCollection soilCollection = PersistableSoilCollectionFactory.Create(soilProfile, new IdFactory(), registry);

            // Assert
            IEnumerable<PersistableSoil> actualSoils = soilCollection.Soils;
            PersistableDataModelTestHelper.AssertPersistableSoils(originalLayers, actualSoils);

            Assert.AreEqual(actualSoils.Count(), registry.Soils.Count);
            for (var i = 0; i < originalLayers.Count(); i++)
            {
                KeyValuePair<IMacroStabilityInwardsSoilLayer, string> registrySoil = registry.Soils.ElementAt(i);
                Assert.AreSame(originalLayers.ElementAt(i), registrySoil.Key);
                Assert.AreEqual(actualSoils.ElementAt(i).Id, registrySoil.Value);
            }
        }

        [Test]
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhi)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.SuCalculated)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated)]
        public void Create_WithSoilProfile2D_ReturnsPersistableSoilCollection(MacroStabilityInwardsShearStrengthModel shearStrengthModel)
        {
            // Setup
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
            var soilProfile = (MacroStabilityInwardsSoilProfile2D) stochasticSoilProfile.SoilProfile;
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
                KeyValuePair<IMacroStabilityInwardsSoilLayer, string> registrySoil = registry.Soils.ElementAt(i);
                Assert.AreSame(originalLayers.ElementAt(i), registrySoil.Key);
                Assert.AreEqual(actualSoils.ElementAt(i).Id, registrySoil.Value);
            }
        }

        [Test]
        public void Create_InvalidShearStrengthType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            MacroStabilityInwardsSoilProfile2D soilProfile = MacroStabilityInwardsSoilProfile2DTestFactory.CreateMacroStabilityInwardsSoilProfile2D();
            var registry = new MacroStabilityInwardsExportRegistry();

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