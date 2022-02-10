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
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
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
    public class NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRowTest
    {
        private static NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.ConstructionProperties ConstructionProperties =>
            new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.ConstructionProperties
            {
                InitialFailureMechanismResultTypeIndex = 2,
                InitialFailureMechanismResultProfileProbabilityIndex = 3,
                InitialFailureMechanismResultSectionProbabilityIndex = 4,
                FurtherAnalysisTypeIndex = 5,
                RefinedProfileProbabilityIndex = 6,
                RefinedSectionProbabilityIndex = 7,
                ProfileProbabilityIndex = 8,
                SectionProbabilityIndex = 9,
                SectionNIndex = 10,
                AssemblyGroupIndex = 11
            };

        [Test]
        public void Constructor_FailureMechanismSectionResultRowErrorProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = CreateFailureMechanismSectionAssemblyResult;

            // Call
            void Call() => new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, null, performAssemblyFunc, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionResultRowErrorProvider", exception.ParamName);
        }

        [Test]
        public void Constructor_PerformAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            void Call() => new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, null, ConstructionProperties);

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
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = CreateFailureMechanismSectionAssemblyResult;

            // Call
            void Call() => new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, null);

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
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = CreateFailureMechanismSectionAssemblyResult;

            // Call
            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.IsRelevant, row.IsRelevant);
            Assert.AreEqual(result.InitialFailureMechanismResultType, row.InitialFailureMechanismResultType);
            Assert.AreEqual(result.ManualInitialFailureMechanismResultProfileProbability, row.InitialFailureMechanismResultProfileProbability);
            Assert.AreEqual(result.ManualInitialFailureMechanismResultSectionProbability, row.InitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(result.FurtherAnalysisType, row.FurtherAnalysisType);
            Assert.AreEqual(result.RefinedProfileProbability, row.RefinedProfileProbability);
            Assert.AreEqual(result.RefinedSectionProbability, row.RefinedSectionProbability);

            TestHelper.AssertTypeConverter<AdoptableWithProfileProbabilityFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.InitialFailureMechanismResultProfileProbability));
            TestHelper.AssertTypeConverter<AdoptableWithProfileProbabilityFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.InitialFailureMechanismResultSectionProbability));
            TestHelper.AssertTypeConverter<AdoptableWithProfileProbabilityFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.RefinedProfileProbability));
            TestHelper.AssertTypeConverter<AdoptableWithProfileProbabilityFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.RefinedSectionProbability));
            TestHelper.AssertTypeConverter<AdoptableWithProfileProbabilityFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.ProfileProbability));
            TestHelper.AssertTypeConverter<AdoptableWithProfileProbabilityFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.SectionProbability));
            TestHelper.AssertTypeConverter<AdoptableWithProfileProbabilityFailureMechanismSectionResultRow, NoValueRoundedDoubleConverter>(
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.SectionN));

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(10, columnStateDefinitions.Count);

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultTypeIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultProfileProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.FurtherAnalysisTypeIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.RefinedProfileProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.RefinedSectionProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.ProfileProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SectionProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SectionNIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.AssemblyGroupIndex);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithIsRelevantTrueAndInitialFailureMechanismResultTypeManual_WhenErrorProviderReturnsError_ThenShowError()
        {
            // Given
            var random = new Random(21);
            double profileProbability = random.NextDouble();
            double sectionProbability = profileProbability + 0.1;

            const string profileErrorText = "profile error";
            const string sectionErrorText = "section error";
            var mocks = new MockRepository();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowErrorProvider>();
            errorProvider.Expect(ep => ep.GetManualProbabilityValidationError(sectionProbability))
                         .Return(profileErrorText);
            errorProvider.Expect(ep => ep.GetManualProbabilityValidationError(sectionProbability))
                         .Return(sectionErrorText);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                InitialFailureMechanismResultType = NonAdoptableInitialFailureMechanismResultType.Manual,
                ManualInitialFailureMechanismResultProfileProbability = sectionProbability,
                ManualInitialFailureMechanismResultSectionProbability = sectionProbability
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = CreateFailureMechanismSectionAssemblyResult;

            // When
            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(profileErrorText, columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(sectionErrorText, columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false, NonAdoptableInitialFailureMechanismResultType.Manual)]
        [TestCase(true, NonAdoptableInitialFailureMechanismResultType.NoFailureProbability)]
        public void GivenRowWithIsRelevantAndInitialFailureMechanismResultType_WhenErrorProviderReturnsError_ThenShowNoError(
            bool isRelevant, NonAdoptableInitialFailureMechanismResultType initialFailureMechanismResultType)
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowErrorProvider>();
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .IgnoreArguments()
                         .Return("error message");
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResultType = initialFailureMechanismResultType
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = CreateFailureMechanismSectionAssemblyResult;

            // When
            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.IsEmpty(columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultProfileProbabilityIndex].ErrorText);
            Assert.IsEmpty(columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithFurtherAnalysisTypeExecuted_WhenErrorProviderReturnsError_ThenShowsError()
        {
            // Given
            var random = new Random(21);
            double profileProbability = random.NextDouble();
            double sectionProbability = profileProbability + 0.1;

            const string profileErrorText = "profile error";
            const string sectionErrorText = "section error";

            var mocks = new MockRepository();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowErrorProvider>();
            errorProvider.Expect(ep => ep.GetManualProbabilityValidationError(profileProbability))
                         .Return(profileErrorText);
            errorProvider.Expect(ep => ep.GetManualProbabilityValidationError(sectionProbability))
                         .Return(sectionErrorText);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                InitialFailureMechanismResultType = NonAdoptableInitialFailureMechanismResultType.NoFailureProbability,
                FurtherAnalysisType = FailureMechanismSectionResultFurtherAnalysisType.Executed,
                RefinedProfileProbability = profileProbability,
                RefinedSectionProbability = sectionProbability
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = CreateFailureMechanismSectionAssemblyResult;

            // When
            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(profileErrorText, columnStateDefinitions[ConstructionProperties.RefinedProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(sectionErrorText, columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(FailureMechanismSectionResultFurtherAnalysisType.NotNecessary)]
        [TestCase(FailureMechanismSectionResultFurtherAnalysisType.Necessary)]
        public void GivenRowWithFurtherAnalysisNotExecuted_WhenErrorProviderReturnsError_ThenShowsNoError(FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType)
        {
            // Given
            var random = new Random(21);
            double profileProbability = random.NextDouble();
            double sectionProbability = profileProbability + 0.1;

            var mocks = new MockRepository();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowErrorProvider>();
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .IgnoreArguments()
                         .Return("error");
            var lengthEffectProvider = mocks.Stub<ILengthEffectProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                InitialFailureMechanismResultType = NonAdoptableInitialFailureMechanismResultType.NoFailureProbability,
                FurtherAnalysisType = furtherAnalysisType,
                RefinedProfileProbability = profileProbability,
                RefinedSectionProbability = sectionProbability
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = CreateFailureMechanismSectionAssemblyResult;

            // When
            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.IsEmpty(columnStateDefinitions[ConstructionProperties.RefinedProfileProbabilityIndex].ErrorText);
            Assert.IsEmpty(columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex].ErrorText);

            mocks.VerifyAll();
        }

        private static FailureMechanismSectionAssemblyResult CreateFailureMechanismSectionAssemblyResult()
        {
            var random = new Random(21);
            return new FailureMechanismSectionAssemblyResult(random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());
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
            const NonAdoptableInitialFailureMechanismResultType newValue = NonAdoptableInitialFailureMechanismResultType.NoFailureProbability;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.InitialFailureMechanismResultType = newValue,
                result => result.InitialFailureMechanismResultType,
                newValue);
        }

        [Test]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetValidProbabilities))]
        public void InitialFailureMechanismResultProfileProbability_SetNewValue_NotifyObserversAndPropertyChanged(double newValue)
        {
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.InitialFailureMechanismResultProfileProbability = newValue,
                result => result.ManualInitialFailureMechanismResultProfileProbability,
                newValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetInvalidProbabilities))]
        public void InitialFailureMechanismResultProfileProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(row => row.InitialFailureMechanismResultProfileProbability = value);
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
        public void RefinedProfileProbability_SetNewValue_NotifyObserversAndPropertyChanged(double newValue)
        {
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.RefinedProfileProbability = newValue,
                result => result.RefinedProfileProbability,
                newValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetInvalidProbabilities))]
        public void RefinedProfileProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(row => row.RefinedProfileProbability = value);
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
            Action<NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow> setPropertyAction,
            Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, T> assertPropertyFunc,
            T newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);
            result.Attach(observer);

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = CreateFailureMechanismSectionAssemblyResult;
            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Call
            setPropertyAction(row);

            // Assert
            Assert.AreEqual(newValue, assertPropertyFunc(result));

            mocks.VerifyAll();
        }

        private static void ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(
            Action<NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow> setPropertyAction)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = CreateFailureMechanismSectionAssemblyResult;
            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

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
        public void Constructor_PerformAssemblyFuncReturnsResult_ReturnsAssemblyResult()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            FailureMechanismSectionAssemblyResult assemblyResult = CreateFailureMechanismSectionAssemblyResult();
            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = () => assemblyResult;

            // Call
            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Assert
            FailureMechanismSectionAssemblyResult rowAssemblyResult = row.AssemblyResult;
            Assert.AreSame(assemblyResult, rowAssemblyResult);

            Assert.AreEqual(rowAssemblyResult.ProfileProbability, row.ProfileProbability);
            Assert.AreEqual(rowAssemblyResult.SectionProbability, row.SectionProbability);
            Assert.AreEqual(rowAssemblyResult.N, row.SectionN, row.SectionN.GetAccuracy());
            Assert.AreEqual(2, row.SectionN.NumberOfDecimalPlaces);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroupDisplayHelper.GetAssemblyGroupDisplayName(rowAssemblyResult.AssemblyGroup),
                            row.AssemblyGroup);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingAndPerformAssemblyThrowsException_ThenShowError()
        {
            // Given
            var random = new Random(39);

            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            int nrOfCalls = 0;
            FailureMechanismSectionAssemblyResult assemblyResult = CreateFailureMechanismSectionAssemblyResult();
            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = () =>
            {
                if (nrOfCalls == 1)
                {
                    throw new AssemblyException();
                }

                nrOfCalls++;
                return assemblyResult;
            };

            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Precondition
            Assert.AreSame(assemblyResult, row.AssemblyResult);

            // When
            row.InitialFailureMechanismResultType = NonAdoptableInitialFailureMechanismResultType.NoFailureProbability;

            // Then
            var expectedAssemblyResult = new DefaultFailureMechanismSectionAssemblyResult();
            FailureMechanismSectionAssemblyResult actualAssemblyResult = row.AssemblyResult;
            Assert.AreEqual(expectedAssemblyResult.N, actualAssemblyResult.N);
            Assert.AreEqual(expectedAssemblyResult.SectionProbability, actualAssemblyResult.SectionProbability);
            Assert.AreEqual(expectedAssemblyResult.ProfileProbability, actualAssemblyResult.ProfileProbability);
            Assert.AreEqual(expectedAssemblyResult.AssemblyGroup, actualAssemblyResult.AssemblyGroup);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithAssemblyErrors_WhenUpdatingAndPerformAssemblyDoesNotThrowException_ThenNoErrorShown()
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            const string errorText = "Message";
            int nrOfCalls = 0;
            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = () =>
            {
                if (nrOfCalls == 1)
                {
                    throw new AssemblyException(errorText);
                }

                nrOfCalls++;
                return CreateFailureMechanismSectionAssemblyResult();
            };

            var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
            FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Precondition
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.ProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SectionNIndex].ErrorText);
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

            // When
            calculator.ThrowExceptionOnCalculate = true;
            row.InitialFailureMechanismResultType = NonAdoptableInitialFailureMechanismResultType.NoFailureProbability;

            // Then

            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.ProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.SectionNIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithAssemblyErrors_WhenUpdatingAndAssemblyDoesNotThrowException_ThenNoErrorShown()
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            const string errorText = "Message";
            int nrOfCalls = 0;
            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = () =>
            {
                if (nrOfCalls == 0)
                {
                    nrOfCalls++;
                    throw new AssemblyException(errorText);
                }

                return CreateFailureMechanismSectionAssemblyResult();
            };

            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Precondition
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.ProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.SectionNIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

            // When
            row.InitialFailureMechanismResultType = NonAdoptableInitialFailureMechanismResultType.NoFailureProbability;

            // Then
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.ProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SectionNIndex].ErrorText);
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
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = CreateFailureMechanismSectionAssemblyResult;

            // Call
            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.ProfileProbabilityIndex], true, true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex], true, true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.SectionNIndex], true, true);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithIsRelevant_ExpectedColumnStates(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .IgnoreArguments()
                         .Return(string.Empty);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                IsRelevant = isRelevant,
                FurtherAnalysisType = FailureMechanismSectionResultFurtherAnalysisType.Executed,
                InitialFailureMechanismResultType = NonAdoptableInitialFailureMechanismResultType.Manual
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = CreateFailureMechanismSectionAssemblyResult;

            // Call
            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultTypeIndex], isRelevant);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultProfileProbabilityIndex], isRelevant);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex], isRelevant);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.FurtherAnalysisTypeIndex], isRelevant);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.RefinedProfileProbabilityIndex], isRelevant);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex], isRelevant);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(NonAdoptableInitialFailureMechanismResultType.Manual, true, false)]
        [TestCase(NonAdoptableInitialFailureMechanismResultType.NoFailureProbability, false, true)]
        public void Constructor_WithInitialFailureMechanismResultType_ExpectedColumnStates(NonAdoptableInitialFailureMechanismResultType initialFailureMechanismResultType,
                                                                                           bool isEnabled, bool isReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .IgnoreArguments()
                         .Return(string.Empty);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                InitialFailureMechanismResultType = initialFailureMechanismResultType
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = CreateFailureMechanismSectionAssemblyResult;

            // Call
            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultProfileProbabilityIndex], isEnabled, isReadOnly);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex], isEnabled, isReadOnly);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(FailureMechanismSectionResultFurtherAnalysisType.NotNecessary, false)]
        [TestCase(FailureMechanismSectionResultFurtherAnalysisType.Necessary, false)]
        [TestCase(FailureMechanismSectionResultFurtherAnalysisType.Executed, true)]
        public void Constructor_WithFurtherAnalysisType_ExpectedColumnStates(FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType,
                                                                             bool expectedDisabled)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .IgnoreArguments()
                         .Return(string.Empty);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                FurtherAnalysisType = furtherAnalysisType
            };

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = CreateFailureMechanismSectionAssemblyResult;

            // Call
            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.RefinedProfileProbabilityIndex], expectedDisabled);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex], expectedDisabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(AssemblyGroupColorTestHelper), nameof(AssemblyGroupColorTestHelper.FailureMechanismSectionAssemblyGroupColorCases))]
        public void Constructor_WithAssemblyGroupSet_ExpectedColumnStates(FailureMechanismSectionAssemblyGroup assemblyGroup,
                                                                          Color expectedBackgroundColor)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            Func<FailureMechanismSectionAssemblyResult> performAssemblyFunc = () => new FailureMechanismSectionAssemblyResult(double.NaN, double.NaN, double.NaN, assemblyGroup);

            // Call
            var row = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(
                columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex], expectedBackgroundColor);

            mocks.VerifyAll();
        }

        #endregion
    }
}