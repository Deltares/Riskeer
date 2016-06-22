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

using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputContextForeshorePropertiesTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup & Call
            var properties = new GrassCoverErosionInwardsInputContextForeshoreProperties();

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
            var properties = new GrassCoverErosionInwardsInputContextForeshoreProperties();

            // Call
            properties.Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionMock);

            // Assert
            Assert.IsFalse(properties.UseForeshore);
            CollectionAssert.IsEmpty(properties.Coordinates);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Data_SetInputContextInstanceWithData_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    UseForeshore = true,
                    DikeProfile = new DikeProfile(new Point2D(0, 0))
                    {
                        ForeshoreGeometry =
                        {
                            new Point2D(1.1, 2.2),
                            new Point2D(3.3, 4.4)
                        }
                    }
                }
            };
            var properties = new GrassCoverErosionInwardsInputContextForeshoreProperties();

            // Call
            properties.Data = new GrassCoverErosionInwardsInputContext(calculation.InputParameters, calculation, failureMechanism, assessmentSectionMock);

            // Assert
            var expectedCoordinates = new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            };
            Assert.IsTrue(properties.UseForeshore);
            CollectionAssert.AreEqual(expectedCoordinates, properties.Coordinates);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberProperties = 1;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var input = new GrassCoverErosionInwardsInput();
            var properties = new GrassCoverErosionInwardsInputContextForeshoreProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionMock)
            };

            input.Attach(observerMock);

            // Call
            properties.UseForeshore = false;

            // Assert
            Assert.IsFalse(input.UseForeshore);
            mockRepository.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void PropertyAttributes_ReturnExpectedValues(bool withDikeProfile)
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var input = new GrassCoverErosionInwardsInput();

            if (withDikeProfile)
            {
                input.DikeProfile = new DikeProfile(new Point2D(0, 0));
            }

            // Call
            var properties = new GrassCoverErosionInwardsInputContextForeshoreProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionMock)
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor useForeshoreProperty = dynamicProperties[useForeshorePropertyIndex];
            Assert.IsNotNull(useForeshoreProperty);
            Assert.AreEqual(!withDikeProfile, useForeshoreProperty.IsReadOnly);
            Assert.AreEqual("Gebruik", useForeshoreProperty.DisplayName);
            Assert.AreEqual("Moet het voorland worden gebruikt tijdens de berekening?", useForeshoreProperty.Description);

            PropertyDescriptor coordinatesProperty = dynamicProperties[coordinatesPropertyIndex];
            Assert.IsNotNull(coordinatesProperty);
            Assert.IsTrue(coordinatesProperty.IsReadOnly);
            Assert.AreEqual("Coördinaten [m]", coordinatesProperty.DisplayName);
            Assert.AreEqual("Lijst met punten in lokale coördinaten.", coordinatesProperty.Description);

            mockRepository.VerifyAll();
        }

        private const int useForeshorePropertyIndex = 0;
        private const int coordinatesPropertyIndex = 1;
    }
}