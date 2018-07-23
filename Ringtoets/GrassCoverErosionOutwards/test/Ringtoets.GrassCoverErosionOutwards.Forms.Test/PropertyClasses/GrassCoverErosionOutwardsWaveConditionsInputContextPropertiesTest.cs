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

using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.Revetment.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsInputContextPropertiesTest
    {
        private MockRepository mockRepository;
        private IObservablePropertyChangeHandler handler;
        private IAssessmentSection assessmentSection;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
        }

        [TearDown]
        public void TearDown()
        {
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                new GrassCoverErosionOutwardsFailureMechanism());

            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsInputContextProperties(context,
                                                                                               AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                                               handler);

            // Assert
            Assert.IsInstanceOf<FailureMechanismCategoryWaveConditionsInputContextProperties<GrassCoverErosionOutwardsWaveConditionsInputContext>>(properties);
            Assert.AreSame(context, properties.Data);
            Assert.AreEqual("Gras", properties.RevetmentType);
        }

        [Test]
        public void Constructor_Always_OverriddenExpectedPropertyDescriptors()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                new GrassCoverErosionOutwardsFailureMechanism());

            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsInputContextProperties(context,
                                                                                               AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                                               handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(16, dynamicProperties.Count);

            const string hydraulicParametersCategory = "Hydraulische gegevens";

            PropertyDescriptor assessmentLevelProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(assessmentLevelProperty,
                                                                            hydraulicParametersCategory,
                                                                            "Waterstand [m+NAP]",
                                                                            "Berekende waterstand op de geselecteerde locatie.",
                                                                            true);

            PropertyDescriptor upperBoundaryDesignWaterLevelProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(upperBoundaryDesignWaterLevelProperty,
                                                                            hydraulicParametersCategory,
                                                                            "Bovengrens op basis van waterstand [m+NAP]",
                                                                            "Bovengrens bepaald aan de hand van de waarde van de waterstand op de geselecteerde hydraulische locatie.",
                                                                            true);
        }
    }
}