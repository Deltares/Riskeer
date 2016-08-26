﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputContextDikeGeometryPropertiesTest
    {
        private const int coordinatesPropertyIndex = 0;
        private const int roughnessesPropertyIndex = 1;
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new GrassCoverErosionInwardsInputContextDikeGeometryProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionInwardsInputContext>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var input = new GrassCoverErosionInwardsInput();
            var properties = new GrassCoverErosionInwardsInputContextDikeGeometryProperties();

            // Call
            properties.Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionMock);

            // Assert
            CollectionAssert.IsEmpty(properties.Coordinates);
            CollectionAssert.IsEmpty(properties.Roughnesses);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Data_SetInputContextInstanceWithData_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var dikeProfile = new DikeProfile(new Point2D(0, 0),
                                              new[]
                                              {
                                                  new RoughnessPoint(new Point2D(1.1, 2.2), 0.6),
                                                  new RoughnessPoint(new Point2D(3.3, 4.4), 0.7)
                                              }, new Point2D[0], null, new DikeProfile.ConstructionProperties());

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };

            var properties = new GrassCoverErosionInwardsInputContextDikeGeometryProperties();

            // Call
            properties.Data = new GrassCoverErosionInwardsInputContext(calculation.InputParameters, calculation, failureMechanism, assessmentSectionMock);

            // Assert
            var expectedCoordinates = new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            };
            CollectionAssert.AreEqual(expectedCoordinates, properties.Coordinates);

            var expectedRoughness = new[]
            {
                new RoundedDouble(2, 0.6)
            };
            CollectionAssert.AreEqual(expectedRoughness, properties.Roughnesses);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var input = new GrassCoverErosionInwardsInput();

            // Call
            var properties = new GrassCoverErosionInwardsInputContextDikeGeometryProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionMock)
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor coordinatesProperty = dynamicProperties[coordinatesPropertyIndex];
            Assert.IsNotNull(coordinatesProperty);
            Assert.IsTrue(coordinatesProperty.IsReadOnly);
            Assert.AreEqual("Coördinaten [m]", coordinatesProperty.DisplayName);
            Assert.AreEqual("Lijst met punten in lokale coördinaten.", coordinatesProperty.Description);

            PropertyDescriptor roughnessesProperty = dynamicProperties[roughnessesPropertyIndex];
            Assert.IsNotNull(roughnessesProperty);
            Assert.IsTrue(roughnessesProperty.IsReadOnly);
            Assert.AreEqual("Ruwheden [-]", roughnessesProperty.DisplayName);
            Assert.AreEqual("Lijst met de ruwheid van elk onderdeel.", roughnessesProperty.Description);

            mockRepository.VerifyAll();
        }
    }
}