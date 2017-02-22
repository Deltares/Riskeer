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
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Forms.UITypeEditors;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputContextDikeProfileEditorTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                DikeProfiles =
                {
                    new TestDikeProfile()
                }
            };
            var grassCoverErosionInwardsInput = new GrassCoverErosionInwardsInput();
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var inputContext = new GrassCoverErosionInwardsInputContext(grassCoverErosionInwardsInput,
                                                                        grassCoverErosionInwardsCalculation,
                                                                        failureMechanism,
                                                                        assessmentSection);

            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);
            var editor = new GrassCoverErosionInwardsInputContextDikeProfileEditor();
            var propertyBag = new DynamicPropertyBag(properties);

            var serviceProviderMock = mockRepository.StrictMock<IServiceProvider>();
            var serviceMock = mockRepository.StrictMock<IWindowsFormsEditorService>();
            var descriptorContextMock = mockRepository.StrictMock<ITypeDescriptorContext>();
            serviceProviderMock.Expect(p => p.GetService(null)).IgnoreArguments().Return(serviceMock);
            serviceMock.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            descriptorContextMock.Expect(c => c.Instance).Return(propertyBag).Repeat.Twice();
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
            DikeProfile dikeProfile = new TestDikeProfile();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                DikeProfiles =
                {
                    dikeProfile
                }
            };
            var grassCoverErosionInwardsInput = new GrassCoverErosionInwardsInput
            {
                DikeProfile = dikeProfile
            };
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var inputContext = new GrassCoverErosionInwardsInputContext(grassCoverErosionInwardsInput,
                                                                        grassCoverErosionInwardsCalculation,
                                                                        failureMechanism,
                                                                        assessmentSection);
            
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            var editor = new GrassCoverErosionInwardsInputContextDikeProfileEditor();
            var propertyBag = new DynamicPropertyBag(properties);

            var serviceProviderMock = mockRepository.StrictMock<IServiceProvider>();
            var serviceMock = mockRepository.StrictMock<IWindowsFormsEditorService>();
            var descriptorContextMock = mockRepository.StrictMock<ITypeDescriptorContext>();
            serviceProviderMock.Expect(p => p.GetService(null)).IgnoreArguments().Return(serviceMock);
            serviceMock.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            serviceMock.Expect(s => s.CloseDropDown()).IgnoreArguments();
            descriptorContextMock.Expect(c => c.Instance).Return(propertyBag).Repeat.Twice();
            mockRepository.ReplayAll();

            var someValue = new object();

            // Call
            var result = editor.EditValue(descriptorContextMock, serviceProviderMock, someValue);

            // Assert
            Assert.AreSame(dikeProfile, result);

            mockRepository.VerifyAll();
        }
    }
}