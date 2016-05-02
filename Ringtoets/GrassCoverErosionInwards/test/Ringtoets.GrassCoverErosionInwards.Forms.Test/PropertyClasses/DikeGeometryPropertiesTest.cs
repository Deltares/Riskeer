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

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DikeGeometryPropertiesTest
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
            var properties = new DikeGeometryProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionInwardsCalculationContext>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual(Resources.DikeGeometryProperties_DisplayName, properties.ToString());
        }

        [Test]
        public void Data_SetNewCalculationContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput());
            var properties = new DikeGeometryProperties();

            // Call
            properties.Data = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanismMock, assessmentSectionMock);

            // Assert
            var coordinates = GetCoordinates(calculation);
            Assert.AreEqual(coordinates, properties.Coordinates);

            var roughness = GetRoughness(calculation);
            Assert.AreEqual(roughness, properties.Roughness);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput());

            // Call
            var properties = new DikeGeometryProperties
            {
                Data = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanismMock, assessmentSectionMock)
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor coordinatesProperty = dynamicProperties[coordinatesPropertyIndex];
            Assert.IsNotNull(coordinatesProperty);
            Assert.IsTrue(coordinatesProperty.IsReadOnly);
            Assert.AreEqual("Coördinaten [m]", coordinatesProperty.DisplayName);
            Assert.AreEqual("Lijst met geometrie punten.", coordinatesProperty.Description);

            PropertyDescriptor numberOfCoordinatesDikeHeightProperty = dynamicProperties[numberOfCoordinatesDikeHeightPropertyIndex];
            Assert.IsNotNull(numberOfCoordinatesDikeHeightProperty);
            Assert.IsTrue(numberOfCoordinatesDikeHeightProperty.IsReadOnly);
            Assert.AreEqual("Ruwheden [-]", numberOfCoordinatesDikeHeightProperty.DisplayName);
            Assert.AreEqual("Lijst met ruwheden per sectie.", numberOfCoordinatesDikeHeightProperty.Description);
        }

        private const int coordinatesPropertyIndex = 0;
        private const int numberOfCoordinatesDikeHeightPropertyIndex = 1;

        private static IEnumerable<string> GetRoughness(GrassCoverErosionInwardsCalculation calculation)
        {
            var roughnesses = calculation.InputParameters.DikeGeometry.Select(d => d.Roughness);
            var i = roughnesses.Select(roughness => new RoundedDouble(2, roughness).Value.ToString(CultureInfo.InvariantCulture));
            return i;
        }

        private static IEnumerable<string> GetCoordinates(GrassCoverErosionInwardsCalculation calculation)
        {
            var startingPoint = calculation.InputParameters.DikeGeometry.FirstOrDefault();
            Assert.IsNotNull(startingPoint);
            var coordinates = new List<string>
            {
                new RoundedDouble(2, startingPoint.StartingPoint.X).Value.ToString(CultureInfo.InvariantCulture)
            };
            coordinates.AddRange(calculation.InputParameters.DikeGeometry.Select(d => new RoundedDouble(2, d.EndingPoint.X).Value.ToString(CultureInfo.InvariantCulture)));
            return coordinates.ToArray();
        }
    }
}