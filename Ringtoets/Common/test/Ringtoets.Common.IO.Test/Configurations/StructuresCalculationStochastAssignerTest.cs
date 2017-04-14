// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.Common.IO.Test.Configurations
{
    [TestFixture]
    public class StructuresCalculationStochastAssignerTest
    {
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
                    .SetName("AreStochastsValid_SetStormDurationStandardDeviation");
                yield return new TestCaseData(
                        new Action<StructuresCalculationConfiguration>(c => c.StormDuration = new StochastConfiguration
                        {
                            VariationCoefficient = 3.2
                        }),
                        "stormduur")
                    .SetName("AreStochastsValid_SetStormDurationVariationCoefficient");
                yield return new TestCaseData(
                        new Action<StructuresCalculationConfiguration>(c => c.ModelFactorSuperCriticalFlow = new StochastConfiguration
                        {
                            StandardDeviation = 3.2
                        }),
                        "modelfactoroverloopdebiet")
                    .SetName("AreStochastsValid_SetModelFactorSuperCriticalFlowStandardDeviation");
                yield return new TestCaseData(
                        new Action<StructuresCalculationConfiguration>(c => c.ModelFactorSuperCriticalFlow = new StochastConfiguration
                        {
                            VariationCoefficient = 3.2
                        }),
                        "modelfactoroverloopdebiet")
                    .SetName("AreStochastsValid_SetModelFactorSuperCriticalFlowStandardDeviation");
            }
        }

        [Test]
        public void Constructor_WithoutConfiguration_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new StructuresCalculation<SimpleStructuresInput>();

            // Call
            TestDelegate test = () => new SimpleStructuresCalculationStochastAssigner(
                null,
                calculation,
                StandardDeviationStochastSetter,
                VariationCoefficientStochastSetter);

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
                null,
                StandardDeviationStochastSetter,
                VariationCoefficientStochastSetter);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutSetStandardDeviationStochast_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var configuration = mocks.Stub<StructuresCalculationConfiguration>("name");
            mocks.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructuresInput>();

            // Call
            TestDelegate test = () => new SimpleStructuresCalculationStochastAssigner(
                configuration,
                calculation,
                null,
                VariationCoefficientStochastSetter);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("setStandardDeviationStochast", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutSetVariationCoefficientStochast_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var configuration = mocks.Stub<StructuresCalculationConfiguration>("name");
            mocks.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructuresInput>();

            // Call
            TestDelegate test = () => new SimpleStructuresCalculationStochastAssigner(
                configuration,
                calculation,
                StandardDeviationStochastSetter,
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("setVariationCoefficientStochast", exception.ParamName);
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
                calculation,
                StandardDeviationStochastSetter,
                VariationCoefficientStochastSetter);

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
                calculation,
                StandardDeviationStochastSetter,
                VariationCoefficientStochastSetter);

            // Call
            bool valid = assigner.PublicValidateSpecificStochasts();

            // Assert
            Assert.IsTrue(valid);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(SetInvalidBaseStochastParameters))]
        public void AreStochastsValid_SpreadDefinedForBaseStochast_LogsErrorAndReturnsFalse(Action<StructuresCalculationConfiguration> modify, string stochastName)
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
                calculation,
                StandardDeviationStochastSetter,
                VariationCoefficientStochastSetter);

            // Call
            var valid = true;
            Action validate = () => valid = assigner.AreStochastsValid();

            // Assert
            string expectedMessage = $"Er kan geen spreiding voor stochast '{stochastName}' opgegeven worden. Berekening '{calculationName}' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(validate, Tuple.Create(expectedMessage, LogLevelConstant.Error));
            Assert.IsFalse(valid);
            mocks.VerifyAll();
        }

        [Test]
        public void AreStochastsValid_SpecificInvalid_ReturnsFalse()
        {
            // Setup
            const string calculationName = "name";
            var mocks = new MockRepository();
            var configuration = mocks.Stub<StructuresCalculationConfiguration>(calculationName);
            mocks.ReplayAll();

            var calculation = new StructuresCalculation<SimpleStructuresInput>();

            var assigner = new SimpleStructuresCalculationStochastAssigner(
                configuration,
                calculation,
                StandardDeviationStochastSetter,
                VariationCoefficientStochastSetter)
            {
                SpecificIsValid = false
            };

            // Call
            bool valid = assigner.AreStochastsValid();

            // Assert
            Assert.IsFalse(valid);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(StructuresCalculationStochastAssignerTest), nameof(GetSetStochastParameterActions), new object[]
        {
            "AreStochastValid_WithoutStructures{0}_LogsErrorReturnFalse"
        })]
        public void AreStochastsValid_WithoutStructureStochastDefinedWithParamter_LogsErrorReturnsFalse(
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
                calculation,
                StandardDeviationStochastSetter,
                VariationCoefficientStochastSetter)
            {
                StandardDeviationStochasts = new[]
                {
                    StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>.StandardDeviationDefinition.Create(stochastName,
                                                                                                                                                                       standardDeviationStochastConfiguration, input => null, (input, distribution) => { })
                },
                VariationCoefficientStochasts = new[]
                {
                    StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>.VariationCoefficientDefinition.Create(stochastName,
                                                                                                                                                                          variationCoefficientStochastConfiguration, input => null, (input, distribution) => { })
                }
            };

            // Call
            var valid = true;
            Action validate = () => valid = assigner.AreStochastsValid();

            // Assert
            string expectedMessage = $"Er is geen kunstwerk opgegeven om de stochast '{stochastName}' aan toe te voegen. Berekening '{calculationName}' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(validate, Tuple.Create(expectedMessage, LogLevelConstant.Error));
            Assert.IsFalse(valid);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(StructuresCalculationStochastAssignerTest), nameof(GetSetStochastParameterActions), new object[]
        {
            "AreStochastValid_WithStructures{0}_ReturnsTrue"
        })]
        public void AreStochastsValid_WithStructure_ReturnsTrue(
            Action<StochastConfiguration> modifyStandardDeviationStochast,
            Action<StochastConfiguration> modifyVariationCoefficientStochast,
            string stochastName)
        {
            // Setup
            const string calculationName = "name";
            var mocks = new MockRepository();
            var configuration = mocks.Stub<StructuresCalculationConfiguration>(calculationName);
            mocks.ReplayAll();

            configuration.StructureName = "some name";

            var calculation = new StructuresCalculation<SimpleStructuresInput>();

            var standardDeviationStochastConfiguration = new StochastConfiguration();
            var variationCoefficientStochastConfiguration = new StochastConfiguration();

            modifyStandardDeviationStochast(standardDeviationStochastConfiguration);
            modifyVariationCoefficientStochast(variationCoefficientStochastConfiguration);

            var assigner = new SimpleStructuresCalculationStochastAssigner(
                configuration,
                calculation,
                StandardDeviationStochastSetter,
                VariationCoefficientStochastSetter)
            {
                StandardDeviationStochasts = new[]
                {
                    StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>.StandardDeviationDefinition.Create(stochastName,
                                                                                                                                                                       standardDeviationStochastConfiguration, input => null, (input, distribution) => { })
                },
                VariationCoefficientStochasts = new[]
                {
                    StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>.VariationCoefficientDefinition.Create(stochastName,
                                                                                                                                                                          variationCoefficientStochastConfiguration, input => null, (input, distribution) => { })
                }
            };

            // Call
            bool valid = assigner.AreStochastsValid();

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

            configuration.StructureName = "some name";

            var calculation = new StructuresCalculation<SimpleStructuresInput>();

            var standardDeviationStochastConfiguration = new StochastConfiguration();
            var variationCoefficientStochastConfiguration = new StochastConfiguration();

            StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>.StandardDeviationDefinition definitionA =
                StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>.StandardDeviationDefinition.Create("stochastA",
                                                                                                                                                                   standardDeviationStochastConfiguration, input => null, (input, distribution) => { });

            StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>.VariationCoefficientDefinition definitionB =
                StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>.VariationCoefficientDefinition.Create("stochastB",
                                                                                                                                                                      variationCoefficientStochastConfiguration, input => null, (input, distribution) => { });

            var standardDeviationSetterCalled = false;
            var variationCoefficientSetterCalled = false;

            var assigner = new SimpleStructuresCalculationStochastAssigner(
                configuration,
                calculation,
                definition =>
                {
                    standardDeviationSetterCalled = true;
                    Assert.AreSame(definitionA, definition);
                    return setStandardDeviationStochastSuccessful;
                },
                definition =>
                {
                    variationCoefficientSetterCalled = true;
                    Assert.AreSame(definitionB, definition);
                    return setVariationCoefficientStochastSuccessful;
                })
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
            bool valid = assigner.SetAllStochasts();

            // Assert
            Assert.AreEqual(setStandardDeviationStochastSuccessful && setVariationCoefficientStochastSuccessful, valid);
            Assert.IsTrue(standardDeviationSetterCalled);
            Assert.AreEqual(setStandardDeviationStochastSuccessful, variationCoefficientSetterCalled);
            mocks.VerifyAll();
        }

        #region StandardDeviationDefinition

        [Test]
        public void StandardDeviationDefinition_StochastNameMissing_ThrowsArgumentNullException()
        {
            // Setup
            var getter = new Func<SimpleStructuresInput, IDistribution>(i => null);
            var setter = new Action<SimpleStructuresInput, IDistribution>((i, d) => { });

            // Call
            TestDelegate test = () => StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>
                .StandardDeviationDefinition.Create(null,
                    null,
                    getter, setter);

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
            var setter = new Action<SimpleStructuresInput, IDistribution>((i, d) => { });

            // Call
            TestDelegate test = () => StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>
                .StandardDeviationDefinition.Create(stochastName,
                    configuration,
                    null, setter);

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
            TestDelegate test = () => StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>
                .StandardDeviationDefinition.Create(stochastName,
                    null,
                    getter, null);

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
            var setter = new Action<SimpleStructuresInput, IDistribution>((i, d) => { });

            // Call
            StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>.StandardDeviationDefinition definition =
                StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>
                    .StandardDeviationDefinition.Create(stochastName,
                        configuration,
                        getter, setter);

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
            var setter = new Action<SimpleStructuresInput, IVariationCoefficientDistribution>((i, d) => { });

            // Call
            TestDelegate test = () => StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>
                .VariationCoefficientDefinition.Create(null,
                    null,
                    getter, setter);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochastName", exception.ParamName);
        }

        [Test]
        public void VariationCoefficientDefinition_GetterMissing_ThrowsArgumentNullException()
        {
            // Setup
            const string stochastName = "";
            var setter = new Action<SimpleStructuresInput, IVariationCoefficientDistribution>((i, d) => { });

            // Call
            TestDelegate test = () =>
                StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>
                    .VariationCoefficientDefinition.Create(stochastName,
                        null,
                        null, setter);

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
            TestDelegate test = () =>
                StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>
                    .VariationCoefficientDefinition.Create(stochastName,
                        null,
                        getter, null);

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
            var setter = new Action<SimpleStructuresInput, IVariationCoefficientDistribution>((i, d) => { });

            // Call
            StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>.VariationCoefficientDefinition definition =
                StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>
                    .VariationCoefficientDefinition.Create(stochastName,
                                                           configuration,
                                                           getter, setter);

            // Assert
            Assert.NotNull(definition);
            Assert.AreEqual(stochastName, definition.StochastName);
            Assert.AreEqual(configuration, definition.Configuration);
            Assert.AreEqual(getter, definition.Getter);
            Assert.AreEqual(setter, definition.Setter);
        }

        #endregion

        private static IEnumerable<TestCaseData> GetSetStochastParameterActions(string testNameFormat)
        {
            yield return new TestCaseData(
                    new Action<StochastConfiguration>(c => c.Mean = 3.2),
                    new Action<StochastConfiguration>(c => { }),
                    "stochastA")
                .SetName(string.Format(testNameFormat, "WithStructureAndStandardDeviationStochastMeanSet"));
            yield return new TestCaseData(
                    new Action<StochastConfiguration>(c => c.StandardDeviation = 3.2),
                    new Action<StochastConfiguration>(c => { }),
                    "stochastB")
                .SetName(string.Format(testNameFormat, "WithStructureAndStandardDeviationStochastStandardDeviationSet"));
            yield return new TestCaseData(
                    new Action<StochastConfiguration>(c => c.VariationCoefficient = 3.2),
                    new Action<StochastConfiguration>(c => { }),
                    "stochastC")
                .SetName(string.Format(testNameFormat, "WithStructureAndStandardDeviationStochastVariationCoefficientSet"));

            yield return new TestCaseData(
                    new Action<StochastConfiguration>(c => { }),
                    new Action<StochastConfiguration>(c => c.Mean = 3.2),
                    "stochastD")
                .SetName(string.Format(testNameFormat, "WithStructureAndVariationCoefficientStochastMeanSet"));
            yield return new TestCaseData(
                    new Action<StochastConfiguration>(c => { }),
                    new Action<StochastConfiguration>(c => c.StandardDeviation = 3.2),
                    "stochastE")
                .SetName(string.Format(testNameFormat, "WithStructureAndVariationCoefficientStochastStandardDeviationSet"));
            yield return new TestCaseData(
                    new Action<StochastConfiguration>(c => { }),
                    new Action<StochastConfiguration>(c => c.VariationCoefficient = 3.2),
                    "stochastF")
                .SetName(string.Format(testNameFormat, "WithStructureAndVariationCoefficientStochastVariationCoefficientSet"));
        }

        private static bool StandardDeviationStochastSetter(
            StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>.StandardDeviationDefinition definition)
        {
            return true;
        }

        private static bool VariationCoefficientStochastSetter(
            StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>.VariationCoefficientDefinition definition)
        {
            return true;
        }

        #region Simple implementations

        private class SimpleStructuresCalculationStochastAssigner
            : StructuresCalculationStochastAssigner<StructuresCalculationConfiguration, SimpleStructuresInput, StructureBase>
        {
            public SimpleStructuresCalculationStochastAssigner(
                StructuresCalculationConfiguration configuration,
                StructuresCalculation<SimpleStructuresInput> calculation,
                TrySetStandardDeviationStochast setStandardDeviationStochast,
                TrySetVariationCoefficientStochast setVariationCoefficientStochast)
                : base(configuration, calculation, setStandardDeviationStochast, setVariationCoefficientStochast) {}

            public bool SpecificIsValid { get; set; } = true;
            public IEnumerable<StandardDeviationDefinition> StandardDeviationStochasts { get; set; } = Enumerable.Empty<StandardDeviationDefinition>();
            public IEnumerable<VariationCoefficientDefinition> VariationCoefficientStochasts { get; set; } = Enumerable.Empty<VariationCoefficientDefinition>();

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

            public bool PublicValidateSpecificStochasts()
            {
                return ValidateSpecificStochasts();
            }
        }

        private class SimpleStructuresInput : StructuresInputBase<StructureBase>
        {
            protected override void UpdateStructureParameters() {}
        }

        #endregion
    }
}