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
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhi, PersistableShearStrengthModelType.CPhi, PersistableShearStrengthModelType.CPhi)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.SuCalculated, PersistableShearStrengthModelType.Su, PersistableShearStrengthModelType.Su)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated, PersistableShearStrengthModelType.CPhi, PersistableShearStrengthModelType.Su)]
        public void Create_WithValidData_ReturnsPersistableSoilCollection(MacroStabilityInwardsShearStrengthModel shearStrengthModel,
                                                                              PersistableShearStrengthModelType expectedShearStrengthModelTypeAbovePhreaticLevel,
                                                                              PersistableShearStrengthModelType expectedShearStrengthModelTypeBelowPhreaticLevel)
        {
            // Setup
            MacroStabilityInwardsSoilProfile2D soilProfile = MacroStabilityInwardsSoilProfile2DTestFactory.CreateMacroStabilityInwardsSoilProfile2D();
            var registry = new MacroStabilityInwardsExportRegistry();

            soilProfile.Layers.ForEachElementDo(layer => layer.Data.ShearStrengthModel = shearStrengthModel);

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
                PersistableDataModelTestHelper.AssertStochasticParameter(layer.Data.Cohesion, soil.CohesionStochasticParameter);
                
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(layer.Data).GetDesignValue(), soil.FrictionAngle);
                PersistableDataModelTestHelper.AssertStochasticParameter(layer.Data.FrictionAngle, soil.FrictionAngleStochasticParameter);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(layer.Data).GetDesignValue(), soil.ShearStrengthRatio);
                PersistableDataModelTestHelper.AssertStochasticParameter(layer.Data.ShearStrengthRatio, soil.ShearStrengthRatioStochasticParameter);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(layer.Data).GetDesignValue(), soil.StrengthIncreaseExponent);
                PersistableDataModelTestHelper.AssertStochasticParameter(layer.Data.StrengthIncreaseExponent, soil.StrengthIncreaseExponentStochasticParameter);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(layer.Data).GetDesignValue(), soil.VolumetricWeightAbovePhreaticLevel);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(layer.Data).GetDesignValue(), soil.VolumetricWeightBelowPhreaticLevel);

                Assert.IsFalse(soil.CohesionAndFrictionAngleCorrelated);
                Assert.IsFalse(soil.ShearStrengthRatioAndShearStrengthExponentCorrelated);

                Assert.AreEqual(expectedShearStrengthModelTypeAbovePhreaticLevel, soil.ShearStrengthModelTypeAbovePhreaticLevel);
                Assert.AreEqual(expectedShearStrengthModelTypeBelowPhreaticLevel, soil.ShearStrengthModelTypeBelowPhreaticLevel);

                KeyValuePair<IMacroStabilityInwardsSoilLayer, string> registrySoil = registry.Soils.ElementAt(i);
                Assert.AreSame(layer, registrySoil.Key);
                Assert.AreEqual(soil.Id, registrySoil.Value);
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