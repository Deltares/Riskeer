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
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ReadOnlyNormalDistributionPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new ReadOnlyNormalDistributionProperties();

            // Assert
            Assert.IsInstanceOf<DistributionProperties>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Normale verdeling", properties.DistributionType);
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var distributionMock = mockRepository.StrictMock<IDistribution>();
            mockRepository.ReplayAll();

            // Call
            var properties = new ReadOnlyNormalDistributionProperties
            {
                Data = distributionMock
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor meanProperty = dynamicProperties.Find("Mean", false);
            Assert.IsNotNull(meanProperty);
            Assert.IsTrue(meanProperty.IsReadOnly);

            PropertyDescriptor standardDeviationProperty = dynamicProperties.Find("StandardDeviation", false);
            Assert.IsNotNull(standardDeviationProperty);
            Assert.IsTrue(standardDeviationProperty.IsReadOnly);
            mockRepository.VerifyAll();
        }
    }
}