// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;
using Riskeer.Piping.Forms.PropertyClasses.Probabilistic;

namespace Riskeer.Piping.Plugin.Test.PropertyInfos.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingSectionSpecificOutputPropertyInfoTest
    {
        private PipingPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new PipingPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(ProbabilisticPipingSectionSpecificOutputProperties));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(ProbabilisticPipingSectionSpecificOutputContext), info.DataType);
        }

        [Test]
        public void CreateInstance_WithContextWithPartialFaultTreeOutput_ExpectedProperties()
        {
            // Setup
            var calculation = new ProbabilisticPipingCalculationScenario
            {
                Output = new ProbabilisticPipingOutput(PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput(),
                                                       PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput())
            };
            var context = new ProbabilisticPipingSectionSpecificOutputContext(calculation);

            // Call
            IObjectProperties objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<ProbabilisticFaultTreePipingSectionSpecificOutputProperties>(objectProperties);
            Assert.AreSame(context.WrappedData.Output.SectionSpecificOutput, objectProperties.Data);
        }

        [Test]
        public void CreateInstance_WithContextWithPartialSubMechanismOutput_ExpectedProperties()
        {
            // Setup
            var calculation = new ProbabilisticPipingCalculationScenario
            {
                Output = new ProbabilisticPipingOutput(PipingTestDataGenerator.GetRandomPartialProbabilisticSubMechanismPipingOutput(),
                                                       PipingTestDataGenerator.GetRandomPartialProbabilisticSubMechanismPipingOutput())
            };
            var context = new ProbabilisticPipingSectionSpecificOutputContext(calculation);

            // Call
            IObjectProperties objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<ProbabilisticSubMechanismPipingSectionSpecificOutputProperties>(objectProperties);
            Assert.AreSame(context.WrappedData.Output.SectionSpecificOutput, objectProperties.Data);
        }

        [Test]
        public void CreateInstance_WithContextWithOtherPartialOutput_Null()
        {
            // Setup
            var calculation = new ProbabilisticPipingCalculationScenario
            {
                Output = new ProbabilisticPipingOutput(PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(),
                                                       PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput())
            };

            var context = new ProbabilisticPipingSectionSpecificOutputContext(calculation);

            // Call
            IObjectProperties objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsNull(objectProperties);
        }
    }
}