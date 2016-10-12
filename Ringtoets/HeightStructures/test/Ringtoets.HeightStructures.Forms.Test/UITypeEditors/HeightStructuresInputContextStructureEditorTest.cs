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
using System.Windows.Forms.Design;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.PropertyClasses;
using Ringtoets.HeightStructures.Forms.UITypeEditors;

namespace Ringtoets.HeightStructures.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class HeightStructuresInputContextStructureEditorTest
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
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            var heightStructure = new TestHeightStructure();
            var inputContext = new HeightStructuresInputContext(
                new HeightStructuresCalculation(), 
                new HeightStructuresFailureMechanism
                {
                    HeightStructures =
                    {
                        heightStructure
                    }
                },
                assessmentSectionMock);

            var properties = new HeightStructuresInputContextProperties
            {
                Data = inputContext
            };

            var propertyBag = new DynamicPropertyBag(properties);

            var serviceProviderMock = mockRepository.Stub<IServiceProvider>();
            var serviceMock = mockRepository.Stub<IWindowsFormsEditorService>();
            var descriptorContextMock = mockRepository.Stub<ITypeDescriptorContext>();
            serviceProviderMock.Stub(p => p.GetService(null)).IgnoreArguments().Return(serviceMock);
            descriptorContextMock.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            var editor = new HeightStructuresInputContextStructureEditor();
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
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            var heightStructure = new TestHeightStructure();
            var inputContext = new HeightStructuresInputContext(
                new HeightStructuresCalculation
                {
                    InputParameters =
                    {
                        HeightStructure = heightStructure
                    }
                }, 
                new HeightStructuresFailureMechanism
                {
                    HeightStructures =
                    {
                        heightStructure
                    }
                },
                assessmentSectionMock);

            var properties = new HeightStructuresInputContextProperties
            {
                Data = inputContext
            };

            var propertyBag = new DynamicPropertyBag(properties);

            var serviceProviderMock = mockRepository.Stub<IServiceProvider>();
            var serviceMock = mockRepository.Stub<IWindowsFormsEditorService>();
            var descriptorContextMock = mockRepository.Stub<ITypeDescriptorContext>();
            serviceProviderMock.Stub(p => p.GetService(null)).IgnoreArguments().Return(serviceMock);
            descriptorContextMock.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            var editor = new HeightStructuresInputContextStructureEditor();
            var someValue = new object();

            // Call
            var result = editor.EditValue(descriptorContextMock, serviceProviderMock, someValue);

            // Assert
            Assert.AreSame(heightStructure, result);
            mockRepository.VerifyAll();
        }
    }
}