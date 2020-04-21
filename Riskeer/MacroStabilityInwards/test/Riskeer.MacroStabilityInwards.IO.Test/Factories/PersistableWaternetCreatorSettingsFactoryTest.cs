using System;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.IO.Factories;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableWaternetCreatorSettingsFactoryTest
    {
        [Test]
        public void Create_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableWaternetCreatorSettingsFactory.Create(null, new IdFactory(), new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void Create_IdFactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableWaternetCreatorSettingsFactory.Create(new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties()), null, new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idFactory", exception.ParamName);
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableWaternetCreatorSettingsFactory.Create(new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties()), new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }
    }
}