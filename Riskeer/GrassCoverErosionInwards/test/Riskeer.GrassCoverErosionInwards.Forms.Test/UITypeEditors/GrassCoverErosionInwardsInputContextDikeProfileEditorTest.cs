// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses;
using Riskeer.GrassCoverErosionInwards.Forms.UITypeEditors;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.UITypeEditors
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                DikeProfileTestFactory.CreateDikeProfile()
            }, "path");
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

            var serviceProvider = mockRepository.StrictMock<IServiceProvider>();
            var service = mockRepository.StrictMock<IWindowsFormsEditorService>();
            var descriptorContext = mockRepository.StrictMock<ITypeDescriptorContext>();
            serviceProvider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            descriptorContext.Expect(c => c.Instance).Return(propertyBag).Repeat.Twice();
            mockRepository.ReplayAll();

            var someValue = new object();

            // Call
            object result = editor.EditValue(descriptorContext, serviceProvider, someValue);

            // Assert
            Assert.AreSame(someValue, result);

            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile
            }, "path");

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

            var serviceProvider = mockRepository.StrictMock<IServiceProvider>();
            var service = mockRepository.StrictMock<IWindowsFormsEditorService>();
            var descriptorContext = mockRepository.StrictMock<ITypeDescriptorContext>();
            serviceProvider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            service.Expect(s => s.CloseDropDown()).IgnoreArguments();
            descriptorContext.Expect(c => c.Instance).Return(propertyBag).Repeat.Twice();
            mockRepository.ReplayAll();

            var someValue = new object();

            // Call
            object result = editor.EditValue(descriptorContext, serviceProvider, someValue);

            // Assert
            Assert.AreSame(dikeProfile, result);

            mockRepository.VerifyAll();
        }
    }
}