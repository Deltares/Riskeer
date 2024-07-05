// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Primitives;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.FailureMechanismSectionResults;

namespace Riskeer.Storage.Core.Test.Read.FailureMechanismSectionResults
{
    [TestFixture]
    public class FailureMechanismSectionResultEntityReadExtensionsTest
    {
        #region AdoptableFailureMechanismSectionResultEntity

        [Test]
        public void ReadAdoptableFailureMechanismSectionResultEntity_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => ((AdoptableFailureMechanismSectionResultEntity) null).Read(sectionResult);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ReadAdoptableFailureMechanismSectionResultEntity_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new AdoptableFailureMechanismSectionResultEntity();

            // Call
            void Call() => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void ReadAdoptableFailureMechanismSectionResultEntity_ParameterValues_SectionResultWithParameterValues()
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            var initialFailureMechanismResultType = random.NextEnumValue<AdoptableInitialFailureMechanismResultType>();
            double manualSectionProbability = random.NextDouble();
            var furtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>();
            double refinedSectionProbability = random.NextDouble();

            var entity = new AdoptableFailureMechanismSectionResultEntity
            {
                IsRelevant = Convert.ToByte(isRelevant),
                InitialFailureMechanismResultType = Convert.ToByte(initialFailureMechanismResultType),
                ManualInitialFailureMechanismResultSectionProbability = manualSectionProbability,
                FurtherAnalysisType = Convert.ToByte(furtherAnalysisType),
                RefinedSectionProbability = refinedSectionProbability
            };
            var sectionResult = new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.AreEqual(isRelevant, sectionResult.IsRelevant);
            Assert.AreEqual(initialFailureMechanismResultType, sectionResult.InitialFailureMechanismResultType);
            Assert.AreEqual(manualSectionProbability, sectionResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(furtherAnalysisType, sectionResult.FurtherAnalysisType);
            Assert.AreEqual(refinedSectionProbability, sectionResult.RefinedSectionProbability);
        }

        [Test]
        public void ReadAdoptableFailureMechanismSectionResultEntity_EntityWithNullValues_SectionResultWithNaNValues()
        {
            // Setup
            var entity = new AdoptableFailureMechanismSectionResultEntity();
            var sectionResult = new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.IsNaN(sectionResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.IsNaN(sectionResult.RefinedSectionProbability);
        }

        #endregion

        #region NonAdoptableFailureMechanismSectionResultEntity

        [Test]
        public void ReadNonAdoptableFailureMechanismSectionResultEntity_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new NonAdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => ((NonAdoptableFailureMechanismSectionResultEntity) null).Read(sectionResult);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ReadNonAdoptableFailureMechanismSectionResultEntity_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new NonAdoptableFailureMechanismSectionResultEntity();

            // Call
            void Call() => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void ReadNonAdoptableFailureMechanismSectionResultEntity_ParameterValues_SectionResultWithParameterValues()
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            var initialFailureMechanismResultType = random.NextEnumValue<NonAdoptableInitialFailureMechanismResultType>();
            double manualSectionProbability = random.NextDouble();
            var furtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>();
            double refinedSectionProbability = random.NextDouble();

            var entity = new NonAdoptableFailureMechanismSectionResultEntity
            {
                IsRelevant = Convert.ToByte(isRelevant),
                InitialFailureMechanismResultType = Convert.ToByte(initialFailureMechanismResultType),
                ManualInitialFailureMechanismResultSectionProbability = manualSectionProbability,
                FurtherAnalysisType = Convert.ToByte(furtherAnalysisType),
                RefinedSectionProbability = refinedSectionProbability
            };
            var sectionResult = new NonAdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.AreEqual(isRelevant, sectionResult.IsRelevant);
            Assert.AreEqual(initialFailureMechanismResultType, sectionResult.InitialFailureMechanismResultType);
            Assert.AreEqual(manualSectionProbability, sectionResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(furtherAnalysisType, sectionResult.FurtherAnalysisType);
            Assert.AreEqual(refinedSectionProbability, sectionResult.RefinedSectionProbability);
        }

        [Test]
        public void ReadNonAdoptableFailureMechanismSectionResultEntity_EntityWithNullValues_SectionResultWithNaNValues()
        {
            // Setup
            var entity = new NonAdoptableFailureMechanismSectionResultEntity();
            var sectionResult = new NonAdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.IsNaN(sectionResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.IsNaN(sectionResult.RefinedSectionProbability);
        }

        #endregion
    }
}