﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryDatabasePropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int usePreprocessorPropertyIndex = 1;
        private const int preprocessorDirectoryPropertyIndex = 2;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var context = new HydraulicBoundaryDatabaseContext(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(context);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryDatabaseContext>>(properties);
            Assert.AreSame(context, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const string filePath = @"C:\file.sqlite";
            const bool usePreprocessor = true;
            const string preprocessorDirectory = @"C:\preprocessor";

            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = filePath,
                    CanUsePreprocessor = true,
                    UsePreprocessor = usePreprocessor,
                    PreprocessorDirectory = preprocessorDirectory
                }
            });

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabaseContext);

            // Assert
            Assert.AreEqual(filePath, properties.FilePath);
            Assert.AreEqual(usePreprocessor, properties.UsePreprocessor);
            Assert.AreEqual(preprocessorDirectory, properties.PreprocessorDirectory);
            Assert.AreEqual(preprocessorDirectory, properties.PreprocessorDirectoryReadOnly);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_CanUsePreprocessorTrue_PropertiesHaveExpectedAttributesValues(bool usePreprocessor)
        {
            // Setup
            var context = new HydraulicBoundaryDatabaseContext(new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    CanUsePreprocessor = true,
                    UsePreprocessor = usePreprocessor,
                    PreprocessorDirectory = "Preprocessor"
                }
            });

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(context);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Hydraulische randvoorwaardendatabase",
                                                                            "Locatie van het hydraulische randvoorwaardendatabase bestand.",
                                                                            true);

            PropertyDescriptor usePreprocessorProperty = dynamicProperties[usePreprocessorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(usePreprocessorProperty,
                                                                            "Algemeen",
                                                                            "Gebruik preprocessor",
                                                                            "Gebruik de preprocessor bij het uitvoeren van een berekening.");

            PropertyDescriptor preprocessorDirectoryProperty = dynamicProperties[preprocessorDirectoryPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(preprocessorDirectoryProperty,
                                                                            "Algemeen",
                                                                            "Locatie preprocessor bestanden",
                                                                            "Locatie waar de preprocessor bestanden opslaat.",
                                                                            !usePreprocessor);
        }

        [Test]
        public void Constructor_CanUsePreprocessorFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var context = new HydraulicBoundaryDatabaseContext(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(context);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Hydraulische randvoorwaardendatabase",
                                                                            "Locatie van het hydraulische randvoorwaardendatabase bestand.",
                                                                            true);
        }

        [Test]
        public void Constructor_HydraulicBoundaryDatabaseNotCoupled_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var context = new HydraulicBoundaryDatabaseContext(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(context);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Hydraulische randvoorwaardendatabase",
                                                                            "Locatie van het hydraulische randvoorwaardendatabase bestand.",
                                                                            true);
        }

        [Test]
        public void UsePreprocessor_SetNewValue_ValueSetToHydraulicBoundaryDatabaseAndObserversNotified([Values(true, false)] bool usePreprocessor)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    CanUsePreprocessor = true,
                    UsePreprocessor = !usePreprocessor,
                    PreprocessorDirectory = "Preprocessor"
                }
            };
            var context = new HydraulicBoundaryDatabaseContext(assessmentSection);
            var properties = new HydraulicBoundaryDatabaseProperties(context);

            assessmentSection.Attach(observer);

            // Call
            properties.UsePreprocessor = usePreprocessor;

            // Assert
            Assert.AreEqual(usePreprocessor, assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor);
            mocks.VerifyAll();
        }

        [Test]
        public void PreprocessorDirectory_SetNewValue_ValueSetToHydraulicBoundaryDatabase()
        {
            // Setup
            const string newPreprocessorDirectory = @"C:/path";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    CanUsePreprocessor = true,
                    UsePreprocessor = true,
                    PreprocessorDirectory = "Preprocessor"
                }
            };
            var context = new HydraulicBoundaryDatabaseContext(assessmentSection);
            var properties = new HydraulicBoundaryDatabaseProperties(context);

            // Call
            properties.PreprocessorDirectory = newPreprocessorDirectory;

            // Assert
            Assert.AreEqual(newPreprocessorDirectory, assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory);
        }

        [Test]
        public void DynamicVisibleValidationMethod_DependingOnCanUsePreprocessorAndUsePreprocessor_ReturnExpectedVisibility(
            [Values(true, false)] bool canUsePreprocessor,
            [Values(true, false)] bool usePreprocessor)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            if (canUsePreprocessor)
            {
                assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
                assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = usePreprocessor;
                assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "Preprocessor";
            }

            var context = new HydraulicBoundaryDatabaseContext(assessmentSection);

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(context);

            // Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.FilePath)));
            Assert.AreEqual(canUsePreprocessor, properties.DynamicVisibleValidationMethod(nameof(properties.UsePreprocessor)));
            Assert.AreEqual(canUsePreprocessor && usePreprocessor, properties.DynamicVisibleValidationMethod(nameof(properties.PreprocessorDirectory)));
            Assert.AreEqual(canUsePreprocessor && !usePreprocessor, properties.DynamicVisibleValidationMethod(nameof(properties.PreprocessorDirectoryReadOnly)));
        }
    }
}