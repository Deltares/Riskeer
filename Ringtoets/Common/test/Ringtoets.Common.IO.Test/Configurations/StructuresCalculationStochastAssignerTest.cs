// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.Common.IO.Test.Configurations
{
    [TestFixture]
    public class StructuresCalculationStochastAssignerTest
    {
        [Test]
        public void Constructor_WithoutConfiguration_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new StructuresCalculation<SimpleStructuresInput>();

            // Call
            TestDelegate test = () => new SimpleStructuresCalculationStochastAssigner(
                null,
                calculation);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("configuration", exception.ParamName);
        }

        [Test]
        public void Constructor_WithoutCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var configuration = mocks.Stub<StructuresCalculationConfiguration>("name");
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new SimpleStructuresCalculationStochastAssigner(
                configuration,
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithParameters_ReturnsNewInstance()
        {
            // Setup
            var mocks = new MockRepository();
            var configuration = mocks.Stub<StructuresCalculationConfiguration>("name");
            mocks.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructuresInput>();

            // Call
            var assigner = new SimpleStructuresCalculationStochastAssigner(
                configuration,
                calculation);

            // Assert
            Assert.NotNull(assigner);
            mocks.VerifyAll();
        }

        [Test]
        public void ValidateSpecificStochasts_Always_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var configuration = mocks.Stub<StructuresCalculationConfiguration>("name");
            mocks.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructuresInput>();

            var assigner = new SimpleStructuresCalculationStochastAssigner(
                configuration,
                calculation);

            // Call
            bool valid = assigner.PublicValidateSpecificStochasts();

            // Assert
            Assert.IsTrue(valid);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(SetInvalidBaseStochastParameters))]
        public void Assign_SpreadDefinedForBaseStochast_LogsErrorAndReturnsFalse(Action<StructuresCalculationConfiguration> modify, string stochastName)
        {
            // Setup
            const string calculationName = "name";
            var mocks = new MockRepository();
            var configuration = mocks.Stub<StructuresCalculationConfiguration>(calculationName);
            mocks.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructuresInput>();

            modify(configuration);

            var assigner = new SimpleStructuresCalculationStochastAssigner(
                configuration,
                calculation);

            // Call
            var valid = true;
            Action validate = () => valid = assigner.Assign();

            // Assert
            string expectedMessage = $"Er kan geen spreiding voor stochast '{stochastName}' opgegeven worden. Berekening '{calculationName}' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(validate, Tuple.Create(expectedMessage, LogLevelConstant.Error));
            Assert.IsFalse(valid);
            mocks.VerifyAll();
        }

        [Test]
        public void Assign_SpecificInvalid_ReturnsFalse()
        {
            // Setup
            const string calculationName = "name";
            var mocks = new MockRepository();
            var configuration = mocks.Stub<StructuresCalculationConfiguration>(calculationName);
            mocks.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructuresInput>();

            var assigner = new SimpleStructuresCalculationStochastAssigner(
                configuration,
                calculation)
            {
                SpecificIsValid = false
            };

            // Call
            bool valid = assigner.Assign();

            // Assert
            Assert.IsFalse(valid);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(StructuresCalculationStochastAssignerTest), nameof(GetSetStochastParameterActions), new object[]
        {
            "Assign_WithoutStructures{0}_LogsErrorReturnFalse"
        })]
        public void Assign_WithoutStructureStochastDefinedWithParamter_LogsErrorReturnsFalse(
            Action<StochastConfiguration> modifyStandardDeviationStochast,
            Action<StochastConfiguration> modifyVariationCoefficientStochast,
            string stochastName)
        {
            // Setup
            const string calculationName = "name";
            var mocks = new MockRepository();
            var configuration = mocks.Stub<StructuresCalculationConfiguration>(calculationName);
            mocks.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructuresInput>();
            var standardDeviationStochastConfiguration = new StochastConfiguration();
            var variationCoefficientStochastConfiguration = new StochastConfiguration();

            modifyStandardDeviationStochast(standardDeviationStochastConfiguration);
            modifyVariationCoefficientStochast(variationCoefficientStochastConfiguration);

            var assigner = new SimpleStructuresCalculationStochastAssigner(
                configuration,
                calculation)
            {
                StandardDeviationStochasts = new[]
                {
                    new StructuresCalculationStochastAssigner<
                        StructuresCalculationConfiguration,
                        SimpleStructuresInput,
                        StructureBase>.StandardDeviationDefinition(stochastName,
                                                                   standardDeviationStochastConfiguration,
                                                                   input => null,
                                                                   (input, distribution) => {})
                },
                VariationCoefficientStochasts = new[]
                {
                    new StructuresCalculationStochastAssigner<
                        StructuresCalculationConfiguration, 
                        SimpleStructuresInput, 
                        StructureBase>.VariationCoefficientDefinition(stochastName,
                                                                      variationCoefficientStochastConfiguration,
                                                                      input => null,
                                                                      (input, distribution) => {})
                }
            };

            // Call
            var valid = true;
            Action validate = () => valid = assigner.Assign();

            // Assert
            string expectedMessage = $"Er is geen kunstwerk opgegeven om de stochast '{stochastName}' aan toe te voegen. Berekening '{calculationName}' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(validate, Tuple.Create(expectedMessage, LogLevelConstant.Error));
            Assert.IsFalse(valid);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(StructuresCalculationStochastAssignerTest), nameof(GetSetStochastParameterActions), new object[]
        {
            "Assign_WithStructures{0}_ReturnsTrue"
        })]
        public void Assign_WithStructure_ReturnsTrue(
            Action<StochastConfiguration> modifyStandardDeviationStochast,
            Action<StochastConfiguration> modifyVariationCoefficientStochast,
            string stochastName)
        {
            // Setup
            const string calculationName = "name";
            var mocks = new MockRepository();
            var configuration = mocks.Stub<StructuresCalculationConfiguration>(calculationName);
            mocks.ReplayAll();

            configuration.StructureId = "some ID";

            var calculation = new StructuresCalculation<SimpleStructuresInput>();

            var standardDeviationStochastConfiguration = new StochastConfiguration();
            var variationCoefficientStochastConfiguration = new StochastConfiguration();

            modifyStandardDeviationStochast(standardDeviationStochastConfiguration);
            modifyVariationCoefficientStochast(variationCoefficientStochastConfiguration);

            var assigner = new SimpleStructuresCalculationStochastAssigner(
                configuration,
                calculation)
            {
                StandardDeviationStochasts = new[]
                {
                    new StructuresCalculationStochastAssigner<
                        StructuresCalculationConfiguration, 
                        SimpleStructuresInput, 
                        StructureBase>.StandardDeviationDefinition(stochastName,
                                                                   standardDeviationStochastConfiguration,
                                                                   input => new LogNormalDistribution(),
                                                                   (input, distribution) => {})
                },
                VariationCoefficientStochasts = new[]
                {
                    new StructuresCalculationStochastAssigner<
                        StructuresCalculationConfiguration, 
                        SimpleStructuresInput, 
                        StructureBase>.VariationCoefficientDefinition(stochastName,
                                                                      variationCoefficientStochastConfiguration,
                                                                      input => new VariationCoefficientLogNormalDistribution(),
                                                                      (input, distribution) => {})
                }
            };

            // Call
            bool valid = assigner.Assign();

            // Assert
            Assert.IsTrue(valid);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void SetAllStochasts_Always_CallsGettersAndSetters(
            bool setStandardDeviationStochastSuccessful,
            bool setVariationCoefficientStochastSuccessful)
        {
            // Setup
            const string calculationName = "name";
            var mocks = new MockRepository();
            var configuration = mocks.Stub<StructuresCalculationConfiguration>(calculationName);
            mocks.ReplayAll();

            configuration.StructureId = "some ID";

            var random = new Random(21);
            double allowedLevelIncreaseStorageMean = setStandardDeviationStochastSuccessful ? random.NextDouble() : -1;
            double allowedLevelIncreaseStorageStandardDeviation = random.NextDouble();
            double criticalOvertoppingDischargeMean = setVariationCoefficientStochastSuccessful ? random.NextDouble() : -1;
            double criticalOvertoppingDischargeCoefficientOfVariation = random.NextDouble();

            var calculation = new StructuresCalculation<SimpleStructuresInput>();

            var standardDeviationStochastConfiguration = new StochastConfiguration
            {
                Mean = allowedLevelIncreaseStorageMean,
                StandardDeviation = allowedLevelIncreaseStorageStandardDeviation
            };
            var variationCoefficientStochastConfiguration = new StochastConfiguration
            {
                Mean = criticalOvertoppingDischargeMean,
                VariationCoefficient = criticalOvertoppingDischargeCoefficientOfVariation
            };

            var definitionA =
                new StructuresCalculationStochastAssigner<
                    StructuresCalculationConfiguration, 
                    SimpleStructuresInput, 
                    StructureBase>.StandardDeviationDefinition("stochastA",
                                                               standardDeviationStochastConfiguration,
                                                               input => input.AllowedLevelIncreaseStorage,
                                                               (input, distribution) =>
                                                               {
                                                                   input.AllowedLevelIncreaseStorage = (LogNormalDistribution) distribution;
                                                               });

            var definitionB =
                new StructuresCalculationStochastAssigner<
                    StructuresCalculationConfiguration, 
                    SimpleStructuresInput,
                    StructureBase>.VariationCoefficientDefinition("stochastB",
                                                                  variationCoefficientStochastConfiguration,
                                                                  input => input.CriticalOvertoppingDischarge,
                                                                  (input, distribution) =>
                                                                  {
                                                                      input.CriticalOvertoppingDischarge = (VariationCoefficientLogNormalDistribution) distribution;
                                                                  });

            var assigner = new SimpleStructuresCalculationStochastAssigner(
                configuration,
                calculation)
            {
                StandardDeviationStochasts = new[]
                {
                    definitionA
                },
                VariationCoefficientStochasts = new[]
                {
                    definitionB
                }
            };

            // Call
            bool valid = assigner.Assign();

            // Assert
            Assert.AreEqual(setStandardDeviationStochastSuccessful && setVariationCoefficientStochastSuccessful, valid);
            if (valid)
            {
                Assert.AreEqual(allowedLevelIncreaseStorageMean,
                                calculation.InputParameters.AllowedLevelIncreaseStorage.Mean,
                                calculation.InputParameters.AllowedLevelIncreaseStorage.Mean.GetAccuracy());
                Assert.AreEqual(allowedLevelIncreaseStorageStandardDeviation,
                                calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation,
                                calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation.GetAccuracy());
                Assert.AreEqual(criticalOvertoppingDischargeMean,
                                calculation.InputParameters.CriticalOvertoppingDischarge.Mean,
                                calculation.InputParameters.CriticalOvertoppingDischarge.Mean.GetAccuracy());
                Assert.AreEqual(criticalOvertoppingDischargeCoefficientOfVariation,
                                calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation,
                                calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation.GetAccuracy());
            }
            mocks.VerifyAll();
        }

        #region StandardDeviationDefinition

        [Test]
        public void StandardDeviationDefinition_StochastNameMissing_ThrowsArgumentNullException()
        {
            // Setup
            var getter = new Func<SimpleStructuresInput, IDistribution>(i => null);
            var setter = new Action<SimpleStructuresInput, IDistribution>((i, d) => {});

            // Call
            TestDelegate test = () => new StructuresCalculationStochastAssigner<
                                          StructuresCalculationConfiguration, 
                                          SimpleStructuresInput, 
                                          StructureBase>.StandardDeviationDefinition(null,
                                                                                     null,
                                                                                     getter,
                                                                                     setter);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochastName", exception.ParamName);
        }

        [Test]
        public void StandardDeviationDefinition_GetterMissing_ThrowsArgumentNullException()
        {
            // Setup
            const string stochastName = "";
            var configuration = new StochastConfiguration();
            var setter = new Action<SimpleStructuresInput, IDistribution>((i, d) => {});

            // Call
            TestDelegate test = () => new StructuresCalculationStochastAssigner<
                                          StructuresCalculationConfiguration, 
                                          SimpleStructuresInput, 
                                          StructureBase>.StandardDeviationDefinition(stochastName,
                                                                                     configuration,
                                                                                     null,
                                                                                     setter);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("getter", exception.ParamName);
        }

        [Test]
        public void StandardDeviationDefinition_SetterMissing_ThrowsArgumentNullException()
        {
            // Setup
            const string stochastName = "";
            var getter = new Func<SimpleStructuresInput, IDistribution>(i => null);

            // Call
            TestDelegate test = () => new StructuresCalculationStochastAssigner<
                                          StructuresCalculationConfiguration, 
                                          SimpleStructuresInput, 
                                          StructureBase>.StandardDeviationDefinition(stochastName,
                                                                                     null,
                                                                                     getter,
                                                                                     null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("setter", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void StandardDeviationDefinition_WithOrWithoutConfiguration_ReturnsNewInstance(bool withConfiguration)
        {
            // Setup
            const string stochastName = "";
            StochastConfiguration configuration = null;
            if (withConfiguration)
            {
                configuration = new StochastConfiguration();
            }
            var getter = new Func<SimpleStructuresInput, IDistribution>(i => null);
            var setter = new Action<SimpleStructuresInput, IDistribution>((i, d) => {});

            // Call
            var definition = new StructuresCalculationStochastAssigner<
                                 StructuresCalculationConfiguration, 
                                 SimpleStructuresInput, 
                                 StructureBase>.StandardDeviationDefinition(stochastName,
                                                                            configuration,
                                                                            getter,
                                                                            setter);

            // Assert
            Assert.NotNull(definition);
            Assert.AreEqual(stochastName, definition.StochastName);
            Assert.AreEqual(configuration, definition.Configuration);
            Assert.AreEqual(getter, definition.Getter);
            Assert.AreEqual(setter, definition.Setter);
        }

        #endregion

        #region VariationCoefficientDefinition

        [Test]
        public void VariationCoefficientDefinition_StochastNameMissing_ThrowsArgumentNullException()
        {
            // Setup
            var getter = new Func<SimpleStructuresInput, IVariationCoefficientDistribution>(i => null);
            var setter = new Action<SimpleStructuresInput, IVariationCoefficientDistribution>((i, d) => {});

            // Call
            TestDelegate test = () => new StructuresCalculationStochastAssigner<
                                          StructuresCalculationConfiguration, 
                                          SimpleStructuresInput, 
                                          StructureBase>.VariationCoefficientDefinition(null,
                                                                                        null,
                                                                                        getter,
                                                                                        setter);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochastName", exception.ParamName);
        }

        [Test]
        public void VariationCoefficientDefinition_GetterMissing_ThrowsArgumentNullException()
        {
            // Setup
            const string stochastName = "";
            var setter = new Action<SimpleStructuresInput, IVariationCoefficientDistribution>((i, d) => {});

            // Call
            TestDelegate test = () => new StructuresCalculationStochastAssigner<
                                          StructuresCalculationConfiguration,
                                          SimpleStructuresInput,
                                          StructureBase>.VariationCoefficientDefinition(stochastName,
                                                                                        null,
                                                                                        null,
                                                                                        setter);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("getter", exception.ParamName);
        }

        [Test]
        public void VariationCoefficientDefinition_SetterMissing_ThrowsArgumentNullException()
        {
            // Setup
            const string stochastName = "";
            var getter = new Func<SimpleStructuresInput, IVariationCoefficientDistribution>(i => null);

            // Call
            TestDelegate test = () => new StructuresCalculationStochastAssigner<
                                          StructuresCalculationConfiguration, 
                                          SimpleStructuresInput, 
                                          StructureBase>.VariationCoefficientDefinition(stochastName,
                                                                                        null,
                                                                                        getter,
                                                                                        null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("setter", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void VariationCoefficientDefinition_WithOrWithoutConfiguration_ReturnsNewInstance(bool withConfiguration)
        {
            // Setup
            const string stochastName = "";
            StochastConfiguration configuration = null;
            if (withConfiguration)
            {
                configuration = new StochastConfiguration();
            }
            var getter = new Func<SimpleStructuresInput, IVariationCoefficientDistribution>(i => null);
            var setter = new Action<SimpleStructuresInput, IVariationCoefficientDistribution>((i, d) => {});

            // Call
            var definition = new StructuresCalculationStochastAssigner<
                                 StructuresCalculationConfiguration, 
                                 SimpleStructuresInput, 
                                 StructureBase>.VariationCoefficientDefinition(stochastName,
                                                                               configuration,
                                                                               getter,
                                                                               setter);

            // Assert
            Assert.NotNull(definition);
            Assert.AreEqual(stochastName, definition.StochastName);
            Assert.AreEqual(configuration, definition.Configuration);
            Assert.AreEqual(getter, definition.Getter);
            Assert.AreEqual(setter, definition.Setter);
        }

        #endregion

        #region Test data

        private static IEnumerable<TestCaseData> SetInvalidBaseStochastParameters
        {
            get
            {
                yield return new TestCaseData(
                        new Action<StructuresCalculationConfiguration>(c => c.StormDuration = new StochastConfiguration
                        {
                            StandardDeviation = 3.2
                        }),
                        "stormduur")
                    .SetName("Assign_SetStormDurationStandardDeviation");
                yield return new TestCaseData(
                        new Action<StructuresCalculationConfiguration>(c => c.StormDuration = new StochastConfiguration
                        {
                            VariationCoefficient = 3.2
                        }),
                        "stormduur")
                    .SetName("Assign_SetStormDurationVariationCoefficient");
            }
        }

        private static IEnumerable<TestCaseData> GetSetStochastParameterActions(string testNameFormat)
        {
            yield return new TestCaseData(
                    new Action<StochastConfiguration>(c => c.Mean = 3.2),
                    new Action<StochastConfiguration>(c => {}),
                    "stochastA")
                .SetName(string.Format(testNameFormat, "WithStructureAndStandardDeviationStochastMeanSet"));
            yield return new TestCaseData(
                    new Action<StochastConfiguration>(c => c.StandardDeviation = 3.2),
                    new Action<StochastConfiguration>(c => {}),
                    "stochastB")
                .SetName(string.Format(testNameFormat, "WithStructureAndStandardDeviationStochastStandardDeviationSet"));

            yield return new TestCaseData(
                    new Action<StochastConfiguration>(c => {}),
                    new Action<StochastConfiguration>(c => c.Mean = 3.2),
                    "stochastD")
                .SetName(string.Format(testNameFormat, "WithStructureAndVariationCoefficientStochastMeanSet"));
            yield return new TestCaseData(
                    new Action<StochastConfiguration>(c => {}),
                    new Action<StochastConfiguration>(c => c.VariationCoefficient = 3.2),
                    "stochastF")
                .SetName(string.Format(testNameFormat, "WithStructureAndVariationCoefficientStochastVariationCoefficientSet"));
        }

        #endregion

        #region Simple implementations

        private class SimpleStructuresCalculationStochastAssigner
            : StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>
        {
            public SimpleStructuresCalculationStochastAssigner(
                StructuresCalculationConfiguration configuration,
                StructuresCalculation<SimpleStructuresInput> calculation)
                : base(configuration, calculation) {}

            public bool SpecificIsValid { get; set; } = true;
            public IEnumerable<StandardDeviationDefinition> StandardDeviationStochasts { get; set; } = Enumerable.Empty<StandardDeviationDefinition>();
            public IEnumerable<VariationCoefficientDefinition> VariationCoefficientStochasts { get; set; } = Enumerable.Empty<VariationCoefficientDefinition>();

            public bool PublicValidateSpecificStochasts()
            {
                return ValidateSpecificStochasts();
            }

            protected override bool ValidateSpecificStochasts()
            {
                return SpecificIsValid;
            }

            protected override IEnumerable<StandardDeviationDefinition> GetStandardDeviationStochasts(bool structureDependent = false)
            {
                return StandardDeviationStochasts;
            }

            protected override IEnumerable<VariationCoefficientDefinition> GetVariationCoefficientStochasts(bool structureDependent = false)
            {
                return VariationCoefficientStochasts;
            }
        }

        private class SimpleStructuresInput : StructuresInputBase<StructureBase>
        {
            public override bool IsStructureInputSynchronized
            {
                get
                {
                    return false;
                }
            }

            public override void SynchronizeStructureInput() {}
        }

        #endregion
    }
}