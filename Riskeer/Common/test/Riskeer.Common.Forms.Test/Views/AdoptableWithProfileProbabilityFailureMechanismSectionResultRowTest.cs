// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
    public class AdoptableWithProfileProbabilityFailureMechanismSectionResultRowTest
    {
        private static AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.ConstructionProperties ConstructionProperties =>
            new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.ConstructionProperties
            {
                InitialFailureMechanismResultTypeIndex = 2,
                InitialFailureMechanismResultProfileProbabilityIndex = 3,
                InitialFailureMechanismResultSectionProbabilityIndex = 4,
                FurtherAnalysisTypeIndex = 5,
                ProbabilityRefinementTypeIndex = 6,
                RefinedProfileProbabilityIndex = 7,
                RefinedSectionProbabilityIndex = 8,
                ProfileProbabilityIndex = 9,
                SectionProbabilityIndex = 10,
                SectionNIndex = 11,
                AssemblyGroupIndex = 12
            };

        [Test]
        public void Constructor_CalculateProbabilityStrategyNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            void Call() => new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, null, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculateProbabilityStrategy", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismSectionResultRowErrorProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            void Call() => new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, null, performAssemblyFunc, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionResultRowErrorProvider", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_PerformAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            void Call() => new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, null, ConstructionProperties);

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
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            void Call() => new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const double profileProbability = 0.1;
            const double sectionProbability = 0.2;

            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            calculateStrategy.Stub(c => c.CalculateProfileProbability()).Return(profileProbability);
            calculateStrategy.Stub(c => c.CalculateSectionProbability()).Return(sectionProbability);
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                ProbabilityRefinementType = ProbabilityRefinementType.Both
            };

            // Call
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<AdoptableWithProfileProbabilityFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.IsRelevant, row.IsRelevant);
            Assert.AreEqual(result.InitialFailureMechanismResultType, row.InitialFailureMechanismResultType);
            Assert.AreEqual(profileProbability, row.InitialFailureMechanismResultProfileProbability);
            Assert.AreEqual(sectionProbability, row.InitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(result.FurtherAnalysisType, row.FurtherAnalysisType);
            Assert.AreEqual(result.ProbabilityRefinementType, row.ProbabilityRefinementType);
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
            Assert.AreEqual(11, columnStateDefinitions.Count);

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultTypeIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultProfileProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.FurtherAnalysisTypeIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.ProbabilityRefinementTypeIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.RefinedProfileProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.RefinedSectionProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.ProfileProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SectionProbabilityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SectionNIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.AssemblyGroupIndex);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AdoptableInitialFailureMechanismResultType.Manual)]
        [TestCase(AdoptableInitialFailureMechanismResultType.NoFailureProbability)]
        public void GivenRowWithInitialFailureMechanismResultTypeAdopt_WhenValueChanged_ThenInitialProbabilitiesChanged(AdoptableInitialFailureMechanismResultType newValue)
        {
            // Given
            const double profileProbability = 0.1;
            const double sectionProbability = 0.2;

            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            calculateStrategy.Stub(c => c.CalculateProfileProbability()).Return(profileProbability);
            calculateStrategy.Stub(c => c.CalculateSectionProbability()).Return(sectionProbability);
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Precondition
            Assert.AreEqual(profileProbability, row.InitialFailureMechanismResultProfileProbability);
            Assert.AreEqual(sectionProbability, row.InitialFailureMechanismResultSectionProbability);

            // When
            row.InitialFailureMechanismResultType = newValue;

            // Then
            Assert.AreEqual(result.ManualInitialFailureMechanismResultProfileProbability, row.InitialFailureMechanismResultProfileProbability);
            Assert.AreEqual(result.ManualInitialFailureMechanismResultSectionProbability, row.InitialFailureMechanismResultSectionProbability);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(ProbabilityRefinementType.Profile, double.NaN, "<afgeleid>")]
        [TestCase(ProbabilityRefinementType.Section, "<afgeleid>", double.NaN)]
        public void GivenRowWithProbabilityRefinementType_WhenValueChanged_ThenInitialProbabilitiesChanged(ProbabilityRefinementType newValue, object newProfileValue, object newSectionValue)
        {
            // Given
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                ProbabilityRefinementType = ProbabilityRefinementType.Both
            };

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Precondition
            Assert.AreEqual(result.RefinedProfileProbability, row.RefinedProfileProbability);
            Assert.AreEqual(result.RefinedSectionProbability, row.RefinedSectionProbability);

            // When
            row.ProbabilityRefinementType = newValue;

            // Then
            Assert.AreEqual(newProfileValue, row.RefinedProfileProbability);
            Assert.AreEqual(newSectionValue, row.RefinedSectionProbability);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithIsRelevantTrueAndInitialFailureMechanismResultTypeAdopt_WhenErrorProviderReturnsError_ThenShowError()
        {
            // Given
            const string errorText = "error";
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Expect(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(errorText)
                         .Repeat.Twice();
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // When
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex].ErrorText);

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
            const string sectionErrorText = "error";
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Expect(ep => ep.GetManualProbabilityValidationError(profileProbability))
                         .Return(profileErrorText);
            errorProvider.Expect(ep => ep.GetManualProbabilityValidationError(sectionProbability))
                         .Return(sectionErrorText);
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual,
                ManualInitialFailureMechanismResultProfileProbability = profileProbability,
                ManualInitialFailureMechanismResultSectionProbability = sectionProbability
            };

            // When
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(profileErrorText, columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(sectionErrorText, columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex].ErrorText);

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
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return("error message");
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResultType = initialFailureMechanismResultType
            };

            // When
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithFurtherAnalysisTypeExecutedAndProbabilityRefinementTypeProfile_WhenErrorProviderReturnsError_ThenShowsError()
        {
            // Given
            var random = new Random(21);
            double refinedProfileProbability = random.NextDouble();

            const string profileErrorText = "profile error";

            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(string.Empty);
            errorProvider.Expect(ep => ep.GetManualProbabilityValidationError(refinedProfileProbability))
                         .Return(profileErrorText);

            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                IsRelevant = true,
                FurtherAnalysisType = FailureMechanismSectionResultFurtherAnalysisType.Executed,
                ProbabilityRefinementType = ProbabilityRefinementType.Profile,
                RefinedProfileProbability = refinedProfileProbability
            };

            // When
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(profileErrorText, columnStateDefinitions[ConstructionProperties.RefinedProfileProbabilityIndex].ErrorText);
            Assert.IsEmpty(columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithFurtherAnalysisTypeExecutedAndProbabilityRefinementTypeSection_WhenErrorProviderReturnsError_ThenShowsError()
        {
            // Given
            var random = new Random(21);
            double refinedSectionProbability = random.NextDouble();

            const string sectionErrorText = "section error";

            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(string.Empty);
            errorProvider.Expect(ep => ep.GetManualProbabilityValidationError(refinedSectionProbability))
                         .Return(sectionErrorText);
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                IsRelevant = true,
                FurtherAnalysisType = FailureMechanismSectionResultFurtherAnalysisType.Executed,
                ProbabilityRefinementType = ProbabilityRefinementType.Section,
                RefinedSectionProbability = refinedSectionProbability
            };

            // When
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.IsEmpty(columnStateDefinitions[ConstructionProperties.RefinedProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(sectionErrorText, columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithFurtherAnalysisTypeExecutedAndProbabilityRefinementTypeBoth_WhenErrorProviderReturnsError_ThenShowsError()
        {
            // Given
            var random = new Random(21);
            double refinedProfileProbability = random.NextDouble();
            double refinedSectionProbability = refinedProfileProbability + 0.1;

            const string profileErrorText = "profile error";
            const string sectionErrorText = "section error";

            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(string.Empty);
            errorProvider.Expect(ep => ep.GetManualProbabilityValidationError(refinedProfileProbability))
                         .Return(profileErrorText);
            errorProvider.Expect(ep => ep.GetManualProbabilityValidationError(refinedSectionProbability))
                         .Return(sectionErrorText);
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                IsRelevant = true,
                FurtherAnalysisType = FailureMechanismSectionResultFurtherAnalysisType.Executed,
                ProbabilityRefinementType = ProbabilityRefinementType.Both,
                RefinedProfileProbability = refinedProfileProbability,
                RefinedSectionProbability = refinedSectionProbability
            };

            // When
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(profileErrorText, columnStateDefinitions[ConstructionProperties.RefinedProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(sectionErrorText, columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void GivenRowWithFurtherAnalysisTypeNotExecutedAndProbabilityRefinementType_WhenErrorProviderReturnsError_ThenShowsNoError(
            [Values(FailureMechanismSectionResultFurtherAnalysisType.Necessary, FailureMechanismSectionResultFurtherAnalysisType.NotNecessary)]
            FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType,
            [Values(ProbabilityRefinementType.Profile, ProbabilityRefinementType.Section, ProbabilityRefinementType.Both)]
            ProbabilityRefinementType refinementType)
        {
            // Given
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.StrictMock<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return("calculated probability error message");
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .IgnoreArguments()
                         .Return("error message");
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                IsRelevant = true,
                FurtherAnalysisType = furtherAnalysisType,
                ProbabilityRefinementType = refinementType
            };

            // When
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Then
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.IsEmpty(columnStateDefinitions[ConstructionProperties.RefinedProfileProbabilityIndex].ErrorText);
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
        public void ProbabilityRefinementType_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            const ProbabilityRefinementType newValue = ProbabilityRefinementType.Both;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.ProbabilityRefinementType = newValue,
                result => result.ProbabilityRefinementType,
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
            Action<AdoptableWithProfileProbabilityFailureMechanismSectionResultRow> setPropertyAction,
            Func<AdoptableWithProfileProbabilityFailureMechanismSectionResult, T> assertPropertyFunc,
            T newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Call
            setPropertyAction(row);

            // Assert
            Assert.AreEqual(newValue, assertPropertyFunc(result));

            mocks.VerifyAll();
        }

        private static void ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(
            Action<AdoptableWithProfileProbabilityFailureMechanismSectionResultRow> setPropertyAction)
        {
            // Setup
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

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
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSectionAssemblyResult assemblyResult = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult();
            var assemblyResultWrapper = new FailureMechanismSectionAssemblyResultWrapper(assemblyResult, AssemblyMethod.BOI0A1, AssemblyMethod.BOI0B1);
            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = () => assemblyResultWrapper;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Assert
            Assert.AreEqual(assemblyResult.ProfileProbability, row.ProfileProbability);
            Assert.AreEqual(assemblyResult.SectionProbability, row.SectionProbability);
            Assert.AreEqual(assemblyResult.N, row.SectionN, row.SectionN.GetAccuracy());
            Assert.AreEqual(2, row.SectionN.NumberOfDecimalPlaces);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(assemblyResult.FailureMechanismSectionAssemblyGroup),
                            row.AssemblyGroup);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingAndPerformAssemblyThrowsException_ThenAssemblyPropertiesSetToDefault()
        {
            // Given
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            int nrOfCalls = 0;
            FailureMechanismSectionAssemblyResult assemblyResult = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult();
            var assemblyResultWrapper = new FailureMechanismSectionAssemblyResultWrapper(assemblyResult, AssemblyMethod.BOI0A1, AssemblyMethod.BOI0B1);
            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = () =>
            {
                if (nrOfCalls == 1)
                {
                    throw new AssemblyException();
                }

                nrOfCalls++;
                return assemblyResultWrapper;
            };

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Precondition
            Assert.AreEqual(assemblyResult.ProfileProbability, row.ProfileProbability);
            Assert.AreEqual(assemblyResult.SectionProbability, row.SectionProbability);
            Assert.AreEqual(assemblyResult.N, row.SectionN, row.SectionN.GetAccuracy());
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(assemblyResult.FailureMechanismSectionAssemblyGroup),
                            row.AssemblyGroup);

            // When
            row.InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual;

            // Then
            var expectedAssemblyResult = new DefaultFailureMechanismSectionAssemblyResult();
            Assert.AreEqual(expectedAssemblyResult.ProfileProbability, row.ProfileProbability);
            Assert.AreEqual(expectedAssemblyResult.SectionProbability, row.SectionProbability);
            Assert.AreEqual(expectedAssemblyResult.N, row.SectionN, row.SectionN.GetAccuracy());
            Assert.AreEqual(2, row.SectionN.NumberOfDecimalPlaces);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(expectedAssemblyResult.FailureMechanismSectionAssemblyGroup),
                            row.AssemblyGroup);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingAndPerformAssemblyThrowsException_ThenShowError()
        {
            // Given
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            const string errorText = "Message";
            int nrOfCalls = 0;
            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = () =>
            {
                if (nrOfCalls == 1)
                {
                    throw new AssemblyException(errorText);
                }

                nrOfCalls++;
                return FailureMechanismSectionAssemblyResultWrapperTestFactory.Create();
            };

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Precondition
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.ProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SectionNIndex].ErrorText);
            Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

            // When
            row.InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual;

            // Then
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.ProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.SectionNIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithAssemblyErrors_WhenUpdatingAndPerformAssemblyDoesNotThrowException_ThenNoErrorShown()
        {
            // Given
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            const string errorText = "Message";
            int nrOfCalls = 0;
            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = () =>
            {
                if (nrOfCalls == 0)
                {
                    nrOfCalls++;
                    throw new AssemblyException(errorText);
                }

                return FailureMechanismSectionAssemblyResultWrapperTestFactory.Create();
            };

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Precondition
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.ProfileProbabilityIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.SectionNIndex].ErrorText);
            Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

            // When
            row.InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual;

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
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

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
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .IgnoreArguments()
                         .Return(string.Empty);
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                IsRelevant = isRelevant,
                FurtherAnalysisType = FailureMechanismSectionResultFurtherAnalysisType.Executed,
                InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual,
                ProbabilityRefinementType = ProbabilityRefinementType.Both
            };

            // Call
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

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
                columnStateDefinitions[ConstructionProperties.ProbabilityRefinementTypeIndex], isRelevant);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.RefinedProfileProbabilityIndex], isRelevant);
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
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(string.Empty);
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .IgnoreArguments()
                         .Return(string.Empty);
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                InitialFailureMechanismResultType = initialFailureMechanismResultType
            };

            // Call
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

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
        public void Constructor_WithFurtherAnalysisType_ExpectedColumnStates(FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType, bool expectedDisabled)
        {
            // Setup
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(string.Empty);
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .IgnoreArguments()
                         .Return(string.Empty);
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                FurtherAnalysisType = furtherAnalysisType,
                ProbabilityRefinementType = ProbabilityRefinementType.Both
            };

            // Call
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.ProbabilityRefinementTypeIndex], expectedDisabled);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.RefinedProfileProbabilityIndex], expectedDisabled);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex], expectedDisabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(ProbabilityRefinementType.Both, false, false)]
        [TestCase(ProbabilityRefinementType.Profile, false, true)]
        [TestCase(ProbabilityRefinementType.Section, true, false)]
        public void Constructor_WithProbabilityRefinementType_ExpectedColumnStates(ProbabilityRefinementType probabilityRefinementType,
                                                                                   bool profileProbabilityIsReadOnly,
                                                                                   bool sectionProbabilityIsReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            errorProvider.Stub(ep => ep.GetCalculatedProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(string.Empty);
            errorProvider.Stub(ep => ep.GetManualProbabilityValidationError(double.NaN))
                         .IgnoreArguments()
                         .Return(string.Empty);
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                FurtherAnalysisType = FailureMechanismSectionResultFurtherAnalysisType.Executed,
                ProbabilityRefinementType = probabilityRefinementType
            };

            // Call
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.RefinedProfileProbabilityIndex], true, profileProbabilityIsReadOnly);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex], true, sectionProbabilityIsReadOnly);

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(FailureMechanismSectionAssemblyGroupColorTestHelper), nameof(FailureMechanismSectionAssemblyGroupColorTestHelper.FailureMechanismSectionAssemblyGroupColorCases))]
        public void Constructor_WithAssemblyGroupSet_ExpectedColumnStates(FailureMechanismSectionAssemblyGroup assemblyGroup,
                                                                          Color expectedBackgroundColor)
        {
            // Setup
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc = () => new FailureMechanismSectionAssemblyResultWrapper(
                FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult(assemblyGroup),
                AssemblyMethod.BOI0A1, AssemblyMethod.BOI0B1);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            var row = new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(result, calculateStrategy, errorProvider, performAssemblyFunc, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(
                columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex], expectedBackgroundColor);

            mocks.VerifyAll();
        }

        #endregion
    }
}