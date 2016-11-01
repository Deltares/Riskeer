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
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryDatabasePropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new HydraulicBoundaryDatabaseProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryDatabaseContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            const string filePath = @"C:\file.sqlite";
            HydraulicBoundaryDatabaseContext hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection)
            {
                WrappedData =
                {
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                    {
                        FilePath = filePath
                    }
                }
            };

            // Call
            HydraulicBoundaryDatabaseProperties properties = new HydraulicBoundaryDatabaseProperties
            {
                Data = hydraulicBoundaryDatabaseContext
            };

            // Assert
            Assert.AreEqual(filePath, properties.FilePath);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var hydraulicBoundaryLocationProperties = new HydraulicBoundaryDatabaseProperties();

            var dynamicPropertyBag = new DynamicPropertyBag(hydraulicBoundaryLocationProperties);
            const string expectedFilePathDisplayName = "Hydraulische randvoorwaardendatabase";
            const string expectedFilePathDescription = "Locatie van het hydraulische randvoorwaardendatabase bestand.";
            const string expectedFilePathCategory = "Algemeen";

            // Call
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(hydraulicBoundaryLocationProperties, true);

            // Assert
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            PropertyDescriptor filePathProperty = dynamicProperties.Find("FilePath", false);

            Assert.IsInstanceOf<TypeConverter>(classTypeConverter);

            Assert.IsNotNull(filePathProperty);
            Assert.IsTrue(filePathProperty.IsReadOnly);
            Assert.IsTrue(filePathProperty.IsBrowsable);
            Assert.AreEqual(expectedFilePathDisplayName, filePathProperty.DisplayName);
            Assert.AreEqual(expectedFilePathDescription, filePathProperty.Description);
            Assert.AreEqual(expectedFilePathCategory, filePathProperty.Category);
        }
    }
}