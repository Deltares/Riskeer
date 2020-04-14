using System;
using Components.Persistence.Stability.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
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
            void Call() => PersistableSoilCollectionFactory.Create(soilProfile,
                                                                   null, new MacroStabilityInwardsExportRegistry());

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
            void Call() => PersistableSoilCollectionFactory.Create(soilProfile,
                                                                   new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_WithValidData_ReturnsPersistableSoilCollection()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer>>();
            mocks.ReplayAll();

            // Call
            PersistableSoilCollection soilCollection = PersistableSoilCollectionFactory.Create(soilProfile, new IdFactory(), new MacroStabilityInwardsExportRegistry());

            // Assert
            Assert.IsNotNull(soilCollection);
            Assert.IsNull(soilCollection.Soils);
            mocks.VerifyAll();
        }
    }
}