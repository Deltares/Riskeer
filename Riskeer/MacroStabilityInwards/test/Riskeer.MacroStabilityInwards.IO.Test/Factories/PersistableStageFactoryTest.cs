using System;
using System.Collections.Generic;
using Components.Persistence.Stability.Data;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.IO.TestUtil;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableStageFactoryTest
    {
        [Test]
        public void Create_IdFactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableStageFactory.Create(null, new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idFactory", exception.ParamName);
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableStageFactory.Create(new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_WithValidData_ReturnsStages()
        {
            // Setup
            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();
            for (var i = 0; i < 2; i++)
            {
                var settings = new PersistableCalculationSettings
                {
                    Id = idFactory.Create()
                };
                registry.Add(settings, settings.Id);
            }

            // Call
            IEnumerable<PersistableStage> stages = PersistableStageFactory.Create(idFactory, registry);

            // Assert
            PersistableDataModelTestHelper.AssertStages(stages, registry.Settings.Keys);
        }
    }
}