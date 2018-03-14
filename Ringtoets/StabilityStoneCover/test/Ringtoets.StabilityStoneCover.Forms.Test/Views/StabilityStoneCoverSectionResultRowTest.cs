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
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.Forms;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.Views;

namespace Ringtoets.StabilityStoneCover.Forms.Test.Views
{
    [TestFixture]
    public class StabilityStoneCoverSectionResultRowTest
    {
        private static StabilityStoneCoverSectionResultRow.ConstructionProperties ConstructionProperties
        {
            get
            {
                return new StabilityStoneCoverSectionResultRow.ConstructionProperties
                {
                    SimpleAssessmentResultIndex = 1,
                    DetailedAssessmentResultForFactorizedSignalingNormIndex = 2,
                    DetailedAssessmentResultForSignalingNormIndex = 3,
                    DetailedAssessmentResultForMechanismSpecificLowerLimitNormIndex = 4,
                    DetailedAssessmentResultForLowerLimitNormIndex = 5,
                    DetailedAssessmentResultForFactorizedLowerLimitNormIndex = 6,
                    TailorMadeAssessmentResultIndex = 7,
                    SimpleAssemblyCategoryGroupIndex = 8,
                    DetailedAssemblyCategoryGroupIndex = 9,
                    TailorMadeAssemblyCategoryGroupIndex = 10,
                    CombinedAssemblyCategoryGroupIndex = 11,
                    ManualAssemblyCategoryGroupIndex = 13
                };
            }
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => new StabilityStoneCoverSectionResultRow(result, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new StabilityStoneCoverSectionResultRow(result, ConstructionProperties);

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionResultRow<StabilityStoneCoverFailureMechanismSectionResult>>(row);
                Assert.AreEqual(result.SimpleAssessmentResult, row.SimpleAssessmentResult);
                Assert.AreEqual(result.DetailedAssessmentResultForFactorizedSignalingNorm, row.DetailedAssessmentResultForFactorizedSignalingNorm);
                Assert.AreEqual(result.DetailedAssessmentResultForSignalingNorm, row.DetailedAssessmentResultForSignalingNorm);
                Assert.AreEqual(result.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm, row.DetailedAssessmentResultForSignalingNorm);
                Assert.AreEqual(result.DetailedAssessmentResultForLowerLimitNorm, row.DetailedAssessmentResultForLowerLimitNorm);
                Assert.AreEqual(result.DetailedAssessmentResultForFactorizedLowerLimitNorm, row.DetailedAssessmentResultForFactorizedLowerLimitNorm);
                Assert.AreEqual(SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertTo(result.TailorMadeAssessmentResult),
                                row.TailorMadeAssessmentResult);
                Assert.AreEqual(result.UseManualAssemblyCategoryGroup, row.UseManualAssemblyCategoryGroup);
                Assert.AreEqual(SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertTo(result.ManualAssemblyCategoryGroup),
                                row.ManualAssemblyCategoryGroup);

                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                Assert.AreEqual(12, columnStateDefinitions.Count);

                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SimpleAssessmentResultIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssessmentResultForFactorizedSignalingNormIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssessmentResultForSignalingNormIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssessmentResultForMechanismSpecificLowerLimitNormIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssessmentResultForLowerLimitNormIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssessmentResultForFactorizedLowerLimitNormIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.TailorMadeAssessmentResultIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SimpleAssemblyCategoryGroupIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssemblyCategoryGroupIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.CombinedAssemblyCategoryGroupIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.ManualAssemblyCategoryGroupIndex);
            }
        }

        [Test]
        public void Constructor_AssemblyRan_ReturnCategoryGroups()
        {
            // Setup
            var random = new Random(39);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.DetailedAssessmentAssemblyGroupOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                calculator.TailorMadeAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                calculator.CombinedAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

                // Call
                var row = new StabilityStoneCoverSectionResultRow(result, ConstructionProperties);
                // Assert
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.SimpleAssessmentAssemblyOutput.Group),
                                row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.DetailedAssessmentAssemblyGroupOutput.Value),
                                row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.TailorMadeAssemblyCategoryOutput.Value),
                                row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.CombinedAssemblyCategoryOutput.Value),
                                row.CombinedAssemblyCategoryGroup);
            }
        }

        [Test]
        public void UseManualAssemblyCategoryGroup_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            bool newValue = !result.UseManualAssemblyCategoryGroup;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityStoneCoverSectionResultRow(result, ConstructionProperties);

                // Precondition
                Assert.IsFalse(result.UseManualAssemblyCategoryGroup);

                // Call
                row.UseManualAssemblyCategoryGroup = newValue;

                // Assert
                Assert.AreEqual(newValue, result.UseManualAssemblyCategoryGroup);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ManualAssemblyCategoryGroup_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<SelectableFailureMechanismSectionAssemblyCategoryGroup>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityStoneCoverSectionResultRow(result, ConstructionProperties);

                // Call
                row.ManualAssemblyCategoryGroup = newValue;

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedCategoryGroup = SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertFrom(newValue);
                Assert.AreEqual(expectedCategoryGroup, result.ManualAssemblyCategoryGroup);
                mocks.VerifyAll();
            }
        }

        #region Registration

        [Test]
        public void SimpleAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<SimpleAssessmentValidityOnlyResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityStoneCoverSectionResultRow(result, ConstructionProperties);

                // Call
                row.SimpleAssessmentResult = newValue;

                // Assert
                Assert.AreEqual(newValue, result.SimpleAssessmentResult);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentResultForFactorizedSignalingNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityStoneCoverSectionResultRow(result, ConstructionProperties);

                // Call
                row.DetailedAssessmentResultForFactorizedSignalingNorm = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResultForFactorizedSignalingNorm);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentResultForSignalingNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityStoneCoverSectionResultRow(result, ConstructionProperties);

                // Call
                row.DetailedAssessmentResultForSignalingNorm = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResultForSignalingNorm);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentResultForMechanismSpecificLowerLimitNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityStoneCoverSectionResultRow(result, ConstructionProperties);

                // Call
                row.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentResultForLowerLimitNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityStoneCoverSectionResultRow(result, ConstructionProperties);

                // Call
                row.DetailedAssessmentResultForLowerLimitNorm = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResultForLowerLimitNorm);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentResultForFactorizedLowerLimitNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityStoneCoverSectionResultRow(result, ConstructionProperties);

                // Call
                row.DetailedAssessmentResultForFactorizedLowerLimitNorm = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResultForFactorizedLowerLimitNorm);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void TailorMadeAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<SelectableFailureMechanismSectionAssemblyCategoryGroup>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityStoneCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityStoneCoverSectionResultRow(result, ConstructionProperties);

                // Call
                row.TailorMadeAssessmentResult = newValue;

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedCategoryGroup = SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertFrom(newValue);
                Assert.AreEqual(expectedCategoryGroup, result.TailorMadeAssessmentResult);
                mocks.VerifyAll();
            }
        }

        #endregion
    }
}