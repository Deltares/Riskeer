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

using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Forms.UITypeEditors;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputContextHydraulicBoundaryLocationEditorTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void EditValue_NoCurrentItemInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new TestHydraulicBoundaryLocation());

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var grassCoverErosionInwardsInput = new GrassCoverErosionInwardsInput();
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();

            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            var inputContext = new GrassCoverErosionInwardsInputContext(grassCoverErosionInwardsInput,
                                                                        grassCoverErosionInwardsCalculation,
                                                                        failureMechanism,
                                                                        assessmentSectionMock);

            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = inputContext
            };
            var editor = new GrassCoverErosionInwardsInputContextHydraulicBoundaryLocationEditor();
            var propertyBag = new DynamicPropertyBag(properties);

            var serviceProviderMock = mockRepository.Stub<IServiceProvider>();
            var serviceMock = mockRepository.Stub<IWindowsFormsEditorService>();
            var descriptorContextMock = mockRepository.Stub<ITypeDescriptorContext>();
            serviceProviderMock.Stub(p => p.GetService(null)).IgnoreArguments().Return(serviceMock);
            descriptorContextMock.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            var someValue = new object();

            // Call
            var result = editor.EditValue(descriptorContextMock, serviceProviderMock, someValue);

            // Assert
            Assert.AreSame(someValue, result);

            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var grassCoverErosionInwardsInput = new GrassCoverErosionInwardsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation
            };
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();

            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            var inputContext = new GrassCoverErosionInwardsInputContext(grassCoverErosionInwardsInput,
                                                                        grassCoverErosionInwardsCalculation,
                                                                        failureMechanism,
                                                                        assessmentSectionMock);

            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = inputContext
            };
            var editor = new GrassCoverErosionInwardsInputContextHydraulicBoundaryLocationEditor();
            var propertyBag = new DynamicPropertyBag(properties);

            var serviceProviderMock = mockRepository.Stub<IServiceProvider>();
            var serviceMock = mockRepository.Stub<IWindowsFormsEditorService>();
            var descriptorContextMock = mockRepository.Stub<ITypeDescriptorContext>();
            serviceProviderMock.Stub(p => p.GetService(null)).IgnoreArguments().Return(serviceMock);
            descriptorContextMock.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            var someValue = new object();

            // Call
            var result = editor.EditValue(descriptorContextMock, serviceProviderMock, someValue);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, result);

            mockRepository.VerifyAll();
        }

        private class TestHydraulicBoundaryLocation : HydraulicBoundaryLocation
        {
            public TestHydraulicBoundaryLocation() : base(0, string.Empty, 0, 0) {}
        }
    }
}