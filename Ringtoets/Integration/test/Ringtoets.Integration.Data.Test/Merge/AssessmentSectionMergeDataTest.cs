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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.Merge;

namespace Ringtoets.Integration.Data.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionMergeDataTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionMergeData(null, Enumerable.Empty<IFailureMechanism>(),
                                                                     new AssessmentSectionMergeData.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionMergeData(new AssessmentSection(AssessmentSectionComposition.Dike),
                                                                     null, new AssessmentSectionMergeData.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void Constructor_PropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionMergeData(new AssessmentSection(AssessmentSectionComposition.Dike),
                                                                     Enumerable.Empty<IFailureMechanism>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_EmptyProperties_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            IEnumerable<IFailureMechanism> failureMechanisms = Enumerable.Empty<IFailureMechanism>();

            // Call
            var mergeData = new AssessmentSectionMergeData(assessmentSection, failureMechanisms,
                                                           new AssessmentSectionMergeData.ConstructionProperties());

            // Assert
            Assert.AreSame(assessmentSection, mergeData.AssessmentSection);
            Assert.AreSame(failureMechanisms, mergeData.FailureMechanisms);
            Assert.IsFalse(mergeData.MergePiping);
            Assert.IsFalse(mergeData.MergeGrassCoverErosionInwards);
            Assert.IsFalse(mergeData.MergeMacroStabilityInwards);
            Assert.IsFalse(mergeData.MergeMacroStabilityOutwards);
            Assert.IsFalse(mergeData.MergeMicrostability);
            Assert.IsFalse(mergeData.MergeStabilityStoneCover);
            Assert.IsFalse(mergeData.MergeWaveImpactAsphaltCover);
            Assert.IsFalse(mergeData.MergeWaterPressureAsphaltCover);
            Assert.IsFalse(mergeData.MergeGrassCoverErosionOutwards);
            Assert.IsFalse(mergeData.MergeGrassCoverSlipOffOutwards);
            Assert.IsFalse(mergeData.MergeGrassCoverSlipOffInwards);
            Assert.IsFalse(mergeData.MergeHeightStructures);
            Assert.IsFalse(mergeData.MergeClosingStructures);
            Assert.IsFalse(mergeData.MergePipingStructure);
            Assert.IsFalse(mergeData.MergeStabilityPointStructures);
            Assert.IsFalse(mergeData.MergeStrengthStabilityLengthwiseConstruction);
            Assert.IsFalse(mergeData.MergeDuneErosion);
            Assert.IsFalse(mergeData.MergeTechnicalInnovation);
        }

        [Test]
        public void Constructor_PropertiesSet_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            IEnumerable<IFailureMechanism> failureMechanisms = Enumerable.Empty<IFailureMechanism>();

            var constructionProperties = new AssessmentSectionMergeData.ConstructionProperties
            {
                MergePiping = random.NextBoolean(),
                MergeGrassCoverErosionInwards = random.NextBoolean(),
                MergeMacroStabilityInwards = random.NextBoolean(),
                MergeMacroStabilityOutwards = random.NextBoolean(),
                MergeMicrostability = random.NextBoolean(),
                MergeStabilityStoneCover = random.NextBoolean(),
                MergeWaveImpactAsphaltCover = random.NextBoolean(),
                MergeWaterPressureAsphaltCover = random.NextBoolean(),
                MergeGrassCoverErosionOutwards = random.NextBoolean(),
                MergeGrassCoverSlipOffOutwards = random.NextBoolean(),
                MergeGrassCoverSlipOffInwards = random.NextBoolean(),
                MergeHeightStructures = random.NextBoolean(),
                MergeClosingStructures = random.NextBoolean(),
                MergePipingStructure = random.NextBoolean(),
                MergeStabilityPointStructures = random.NextBoolean(),
                MergeStrengthStabilityLengthwiseConstruction = random.NextBoolean(),
                MergeDuneErosion = random.NextBoolean(),
                MergeTechnicalInnovation = random.NextBoolean()
            };

            // Call            
            var mergeData = new AssessmentSectionMergeData(assessmentSection, failureMechanisms,
                                                           constructionProperties);

            // Assert
            Assert.AreSame(assessmentSection, mergeData.AssessmentSection);
            Assert.AreSame(failureMechanisms, mergeData.FailureMechanisms);
            Assert.AreEqual(constructionProperties.MergePiping, mergeData.MergePiping);
            Assert.AreEqual(constructionProperties.MergeGrassCoverErosionInwards, mergeData.MergeGrassCoverErosionInwards);
            Assert.AreEqual(constructionProperties.MergeMacroStabilityInwards, mergeData.MergeMacroStabilityInwards);
            Assert.AreEqual(constructionProperties.MergeMacroStabilityOutwards, mergeData.MergeMacroStabilityOutwards);
            Assert.AreEqual(constructionProperties.MergeMicrostability, mergeData.MergeMicrostability);
            Assert.AreEqual(constructionProperties.MergeStabilityStoneCover, mergeData.MergeStabilityStoneCover);
            Assert.AreEqual(constructionProperties.MergeWaveImpactAsphaltCover, mergeData.MergeWaveImpactAsphaltCover);
            Assert.AreEqual(constructionProperties.MergeWaterPressureAsphaltCover, mergeData.MergeWaterPressureAsphaltCover);
            Assert.AreEqual(constructionProperties.MergeGrassCoverErosionOutwards, mergeData.MergeGrassCoverErosionOutwards);
            Assert.AreEqual(constructionProperties.MergeGrassCoverSlipOffOutwards, mergeData.MergeGrassCoverSlipOffOutwards);
            Assert.AreEqual(constructionProperties.MergeGrassCoverSlipOffInwards, mergeData.MergeGrassCoverSlipOffInwards);
            Assert.AreEqual(constructionProperties.MergeHeightStructures, mergeData.MergeHeightStructures);
            Assert.AreEqual(constructionProperties.MergeClosingStructures, mergeData.MergeClosingStructures);
            Assert.AreEqual(constructionProperties.MergePipingStructure, mergeData.MergePipingStructure);
            Assert.AreEqual(constructionProperties.MergeStabilityPointStructures, mergeData.MergeStabilityPointStructures);
            Assert.AreEqual(constructionProperties.MergeStrengthStabilityLengthwiseConstruction, mergeData.MergeStrengthStabilityLengthwiseConstruction);
            Assert.AreEqual(constructionProperties.MergeDuneErosion, mergeData.MergeDuneErosion);
            Assert.AreEqual(constructionProperties.MergeTechnicalInnovation, mergeData.MergeTechnicalInnovation);
        }
    }
}