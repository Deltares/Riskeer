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

using System;
using System.ComponentModel;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsInputContextPropertiesTest
    {
        [Test]
        public void Constructor_WithoutContext_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsInputContextProperties(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(
                new WaveConditionsInput(), 
                new TestCalculation(), 
                new GrassCoverErosionOutwardsFailureMechanism());

            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsInputContextProperties(context);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContextProperties<GrassCoverErosionOutwardsWaveConditionsInputContext>>(properties);
            Assert.AreSame(context, properties.Data);
            Assert.AreEqual("Gras", properties.RevetmentType);
        }

        [Test]
        public void Constructor_Always_OverriddenExpectedPropertyDescriptors()
        {
            // Setup
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(
                new WaveConditionsInput(),
                new TestCalculation(),
                new GrassCoverErosionOutwardsFailureMechanism());

            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsInputContextProperties(context);

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                BrowsableAttribute.Yes
            });
            Assert.AreEqual(15, dynamicProperties.Count);

            var hydraulicParametersCategory = "Hydraulische gegevens";

            PropertyDescriptor assessmentLevelProperty = dynamicProperties[1];
            Assert.IsNotNull(assessmentLevelProperty);
            Assert.IsTrue(assessmentLevelProperty.IsReadOnly);
            Assert.AreEqual(hydraulicParametersCategory, assessmentLevelProperty.Category);
            Assert.AreEqual("Waterstand bij doorsnede-eis [m+NAP]", assessmentLevelProperty.DisplayName);
            Assert.AreEqual("Berekende waterstand bij doorsnede-eis op de geselecteerde locatie.", assessmentLevelProperty.Description);

            PropertyDescriptor upperBoundaryDesignWaterLevelProperty = dynamicProperties[2];
            Assert.IsNotNull(upperBoundaryDesignWaterLevelProperty);
            Assert.IsTrue(upperBoundaryDesignWaterLevelProperty.IsReadOnly);
            Assert.AreEqual(hydraulicParametersCategory, upperBoundaryDesignWaterLevelProperty.Category);
            Assert.AreEqual("Bovengrens op basis van waterstand bij doorsnede-eis [m+NAP]", upperBoundaryDesignWaterLevelProperty.DisplayName);
            Assert.AreEqual("Bovengrens bepaald aan de hand van de waarde van de waterstand bij doorsnede-eis op de geselecteerde hydraulische locatie.", upperBoundaryDesignWaterLevelProperty.Description);
        }
    }
}