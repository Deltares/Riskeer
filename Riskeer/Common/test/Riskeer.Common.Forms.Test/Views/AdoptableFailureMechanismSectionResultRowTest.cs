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

using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Data.TestUtil;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.Probability;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Providers;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Primitives;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class AdoptableFailureMechanismSectionResultRowTest
    {
        private static AdoptableFailureMechanismSectionResultRow.ConstructionProperties ConstructionProperties =>
            new AdoptableFailureMechanismSectionResultRow.ConstructionProperties
            {
                InitialFailureMechanismResultTypeIndex = 2,
                InitialFailureMechanismResultSectionProbabilityIndex = 3,
                FurtherAnalysisTypeIndex = 4,
                RefinedSectionProbabilityIndex = 5,
                SectionProbabilityIndex = 6,
                AssemblyGroupIndex = 7
            };

        [Test]
        public void Constructor_CalculateInitialFailureMechanismResultProbabilityFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            // Call
            void Call() => new AdoptableFailureMechanismSectionResultRow(result, null, errorProvider, performAssemblyFunc,
                                                                         ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculateInitialFailureMechanismResultProbabilityFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismSectionResultErrorProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            // Call
            void Call() => new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, null, performAssemblyFunc,
                                                                         ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionResultRowErrorProvider", exception.ParamName);
        }

        [Test]
        public void Constructor_PerformAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            // Call
            void Call() => new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                         null, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("performAssemblyFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            // Call
            void Call() => new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                         performAssemblyFunc, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            double initialFailureMechanismResultProbability = new Random(21).NextDouble();

            // Call
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => initialFailureMechanismResultProbability, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<AdoptableFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.IsRelevant, row.IsRelevant);
            Assert.AreEqual(result.InitialFailureMechanismResultType, row.InitialFailureMechanismResultType);
            Assert.AreEqual(initialFailureMechanismResultProbability, row.InitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(result.FurtherAnalysisType, row.FurtherAnalysisType);
            Assert.AreEqual(result.RefinedSectionProbability, row.RefinedSectionProbability);

            TestHelper.AssertTypeConverter<AdoptableFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(AdoptableFailureMechanismSectionResultRow.InitialFailureMechanismResultSectionProbability));
            TestHelper.AssertTypeConverter<AdoptableFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(AdoptableFailureMechanismSectionResultRow.RefinedSectionProbability));
            TestHelper.AssertTypeConverter<AdoptableFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(AdoptableFailureMechanismSectionResultRow.SectionProbability));

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(6, columnStateDefinitions.Count);

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultTypeIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.FurtherAnalysisTypeIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.RefinedSectionProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SectionProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.AssemblyGroupIndex);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AdoptableInitialFailureMechanismResultType.Manual)]
        [TestCase(AdoptableInitialFailureMechanismResultType.NoFailureProbability)]
        public void GivenRowWithInitialFailureMechanismResultTypeAdopt_WhenValueChanged_ThenInitialProbabilitiesChanged(AdoptableInitialFailureMechanismResultType newValue)
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            double sectionProbability = new Random(21).NextDouble();
            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => sectionProbability, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Precondition
            Assert.AreEqual(sectionProbability, row.InitialFailureMechanismResultSectionProbability);

            // When
            row.InitialFailureMechanismResultType = newValue;

            // Then
            Assert.AreEqual(result.ManualInitialFailureMechanismResultSectionProbability, row.InitialFailureMechanismResultSectionProbability);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithIsRelevantTrueAndInitialFailureMechanismResultTypeAdopt_WhenErrorProviderReturnsError_ThenShowError()
        {
            // Given
            const string errorText = "error";
            var mocks = new MockRepository();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Expect(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(errorText);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            // When
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithIsRelevantTrueAndInitialFailureMechanismResultTypeManual_WhenErrorProviderReturnsError_ThenShowError()
        {
            // Given
            var random = new Random(21);
            double sectionProbability = random.NextDouble();

            const string errorText = "error";
            var mocks = new MockRepository();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Expect(ep => ep.GetManualProbabilityValidationError(sectionProbability))
                         .Return(errorText);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section)
            {
                InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual,
                ManualInitialFailureMechanismResultSectionProbability = sectionProbability
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            // When
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false, AdoptableInitialFailureMechanismResultType.Adopt)]
        [TestCase(false, AdoptableInitialFailureMechanismResultType.Manual)]
        [TestCase(true, AdoptableInitialFailureMechanismResultType.NoFailureProbability)]
        public void GivenRowWithIsRelevantAndInitialFailureMechanismResultType_WhenErrorProviderReturnsError_ThenShowNoError(
            bool isRelevant, AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType)
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return("error message");
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section)
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResultType = initialFailureMechanismResultType
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            // When
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithFurtherAnalysisTypeExecuted_WhenErrorProviderReturnsError_ThenShowsError()
        {
            // Given
            var random = new Random(21);
            double sectionProbability = random.NextDouble();

            const string errorText = "error";
            var mocks = new MockRepository();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(string.Empty);
            errorProvider.Expect(ep => ep.GetManualProbabilityValidationError(sectionProbability))
                         .Return(errorText);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section)
            {
                FurtherAnalysisType = FailureMechanismSectionResultFurtherAnalysisType.Executed,
                RefinedSectionProbability = sectionProbability
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            // When
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(FailureMechanismSectionResultFurtherAnalysisType.NotNecessary)]
        [TestCase(FailureMechanismSectionResultFurtherAnalysisType.Necessary)]
        public void GivenRowWithFurtherAnalysisTypeNotExecuted_WhenErrorProviderReturnsError_ThenShowsNoError(FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType)
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(string.Empty);
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .IgnoreArguments()
                         .Return("error message");
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section)
            {
                FurtherAnalysisType = furtherAnalysisType
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            // When
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.IsEmpty(columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex].ErrorText);

            mocks.VerifyAll();
        }

        #region Registration

        [Test]
        public void IsRelevant_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            const bool newValue = false;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.IsRelevant = newValue,
                result => result.IsRelevant,
                newValue);
        }

        [Test]
        public void InitialFailureMechanismResultType_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            const AdoptableInitialFailureMechanismResultType newValue = AdoptableInitialFailureMechanismResultType.NoFailureProbability;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.InitialFailureMechanismResultType = newValue,
                result => result.InitialFailureMechanismResultType,
                newValue);
        }

        [Test]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetValidProbabilities))]
        public void InitialFailureMechanismResultSectionProbability_SetNewValue_NotifyObserversAndPropertyChanged(double newValue)
        {
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.InitialFailureMechanismResultSectionProbability = newValue,
                result => result.ManualInitialFailureMechanismResultSectionProbability,
                newValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetInvalidProbabilities))]
        public void InitialFailureMechanismResultSectionProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(row => row.InitialFailureMechanismResultSectionProbability = value);
        }

        [Test]
        public void FurtherAnalysisType_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            const FailureMechanismSectionResultFurtherAnalysisType newValue = FailureMechanismSectionResultFurtherAnalysisType.Necessary;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.FurtherAnalysisType = newValue,
                result => result.FurtherAnalysisType,
                newValue);
        }

        [Test]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetValidProbabilities))]
        public void RefinedSectionProbability_SetNewValue_NotifyObserversAndPropertyChanged(double newValue)
        {
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.RefinedSectionProbability = newValue,
                result => result.RefinedSectionProbability,
                newValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetInvalidProbabilities))]
        public void RefinedSectionProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(row => row.RefinedSectionProbability = value);
        }

        private static void Property_SetNewValue_NotifyObserversAndPropertyChanged<T>(
            Action<AdoptableFailureMechanismSectionResultRow> setPropertyAction,
            Func<AdoptableFailureMechanismSectionResult, T> assertPropertyFunc,
            T newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);
            result.Attach(observer);

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Call
            setPropertyAction(row);

            // Assert
            Assert.AreEqual(newValue, assertPropertyFunc(result));

            mocks.VerifyAll();
        }

        private static void ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(
            Action<AdoptableFailureMechanismSectionResultRow> setPropertyAction)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Call
            void Call() => setPropertyAction(row);

            // Assert
            const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);

            mocks.VerifyAll();
        }

        #endregion

        #region Assembly

        [Test]
        public void Constructor_AssemblyRan_ReturnsAssemblyResult()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            var random = new Random(39);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            FailureMechanismSectionAssemblyResult assemblyResult = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult();
            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = () => assemblyResult;

            // Call
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Assert
            FailureMechanismSectionAssemblyResult rowAssemblyResult = row.AssemblyResult;
            Assert.AreSame(assemblyResult, rowAssemblyResult);

            Assert.AreEqual(rowAssemblyResult.SectionProbability, row.SectionProbability);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(rowAssemblyResult.FailureMechanismSectionAssemblyGroup),
                            row.AssemblyGroup);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingAndAssemblyThrowsException_ThenAssemblyPropertiesSetToDefault()
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            int nrOfCalls = 0;
            FailureMechanismSectionAssemblyResult assemblyResult = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult();
            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = () =>
            {
                if (nrOfCalls == 1)
                {
                    throw new AssemblyException();
                }

                nrOfCalls++;
                return assemblyResult;
            };

            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Precondition
            Assert.AreSame(assemblyResult, row.AssemblyResult);

            // When
            row.InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual;

            // Then
            var expectedAssemblyResult = new DefaultFailureMechanismSectionAssemblyResult();
            FailureMechanismSectionAssemblyResult actualAssemblyResult = row.AssemblyResult;
            Assert.AreEqual(expectedAssemblyResult.N, actualAssemblyResult.N);
            Assert.AreEqual(expectedAssemblyResult.SectionProbability, actualAssemblyResult.SectionProbability);
            Assert.AreEqual(expectedAssemblyResult.ProfileProbability, actualAssemblyResult.ProfileProbability);
            Assert.AreEqual(expectedAssemblyResult.FailureMechanismSectionAssemblyGroup, actualAssemblyResult.FailureMechanismSectionAssemblyGroup);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingAndAssemblyThrowsException_ThenShowError()
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            const string errorText = "Message";
            int nrOfCalls = 0;
            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = () =>
            {
                if (nrOfCalls == 1)
                {
                    throw new AssemblyException(errorText);
                }

                nrOfCalls++;
                return FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult();
            };

            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Precondition
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

            // When
            row.InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual;

            // Then

            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithAssemblyErrors_WhenUpdatingAndAssemblyDoesNotThrowException_ThenNoErrorShown()
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            const string errorText = "Message";
            int nrOfCalls = 0;
            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = () =>
            {
                if (nrOfCalls == 0)
                {
                    nrOfCalls++;
                    throw new AssemblyException(errorText);
                }

                return FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult();
            };

            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Precondition
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            const string expectedErrorText = "Message";

            Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
            Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

            // When
            row.InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual;

            // Then
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

            mocks.VerifyAll();
        }

        #endregion

        #region Column States

        [Test]
        public void Constructor_Always_ExpectedColumnStates()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            // Call
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex], true, true);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithIsRelevant_ExpectedColumnStates(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .Return(string.Empty);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section)
            {
                IsRelevant = isRelevant,
                FurtherAnalysisType = FailureMechanismSectionResultFurtherAnalysisType.Executed,
                InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            // Call
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultTypeIndex], isRelevant);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex], isRelevant);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.FurtherAnalysisTypeIndex], isRelevant);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex], isRelevant);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AdoptableInitialFailureMechanismResultType.Adopt, true, true)]
        [TestCase(AdoptableInitialFailureMechanismResultType.Manual, true, false)]
        [TestCase(AdoptableInitialFailureMechanismResultType.NoFailureProbability, false, true)]
        public void Constructor_WithInitialFailureMechanismResultType_ExpectedColumnStates(AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType,
                                                                                           bool isEnabled, bool isReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(string.Empty);
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .Return(string.Empty);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section)
            {
                InitialFailureMechanismResultType = initialFailureMechanismResultType
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            // Call
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex], isEnabled, isReadOnly);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(FailureMechanismSectionResultFurtherAnalysisType.NotNecessary, false)]
        [TestCase(FailureMechanismSectionResultFurtherAnalysisType.Necessary, false)]
        [TestCase(FailureMechanismSectionResultFurtherAnalysisType.Executed, true)]
        public void Constructor_WithFurtherAnalysisType_ExpectedColumnStates(FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType,
                                                                             bool expectedEnabled)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(string.Empty);
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .IgnoreArguments()
                         .Return(string.Empty);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section)
            {
                FurtherAnalysisType = furtherAnalysisType
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult;

            // Call
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex], expectedEnabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(AssemblyGroupColorTestHelper), nameof(AssemblyGroupColorTestHelper.FailureMechanismSectionAssemblyGroupColorCases))]
        public void Constructor_WithAssemblyGroupSet_ExpectedColumnStates(FailureMechanismSectionAssemblyGroup assemblyGroup,
                                                                          Color expectedBackgroundColor)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = () => FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult(assemblyGroup);

            // Call
            var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                    performAssemblyFunc, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(
                columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex], expectedBackgroundColor);

            mocks.VerifyAll();
        }

        #endregion
    }
}