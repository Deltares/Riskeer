using System;
using System.Collections.Generic;
using Components.Persistence.Stability.Data;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.IO.TestUtil;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableCalculationSettingsFactoryTest
    {
        [Test]
        public void Create_SlidingCurveNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableCalculationSettingsFactory.Create(null, new IdFactory(), new MacroStabilityInwardsPersistenceRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("slidingCurve", exception.ParamName);
        }

        [Test]
        public void Create_IdFactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableCalculationSettingsFactory.Create(MacroStabilityInwardsOutputTestFactory.CreateOutput().SlidingCurve,
                                                                        null, new MacroStabilityInwardsPersistenceRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idFactory", exception.ParamName);
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableCalculationSettingsFactory.Create(MacroStabilityInwardsOutputTestFactory.CreateOutput().SlidingCurve,
                                                                        new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_WithValidData_ReturnsPersistableCalculationSettingsCollection()
        {
            // Setup
            MacroStabilityInwardsSlidingCurve slidingCurve = MacroStabilityInwardsOutputTestFactory.CreateOutput().SlidingCurve;
            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsPersistenceRegistry();

            // Call
            IEnumerable<PersistableCalculationSettings> settingsCollection = PersistableCalculationSettingsFactory.Create(slidingCurve, idFactory, registry);

            // Assert
            PersistableDataModelTestHelper.AssertCalculationSettings(slidingCurve, settingsCollection);
            CollectionAssert.AreEqual(settingsCollection, registry.Settings.Keys);
        }
    }
}