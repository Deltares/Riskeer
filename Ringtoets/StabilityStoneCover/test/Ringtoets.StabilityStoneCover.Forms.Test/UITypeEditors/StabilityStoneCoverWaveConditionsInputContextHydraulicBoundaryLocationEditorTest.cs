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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PropertyClasses;
using Ringtoets.StabilityStoneCover.Forms.UITypeEditors;

namespace Ringtoets.StabilityStoneCover.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsInputContextHydraulicBoundaryLocationEditorTest
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

            var grassCoverErosionInwardsInput = new WaveConditionsInput();

            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Expect(asm => asm.HydraulicBoundaryDatabase)
                                 .Return(hydraulicBoundaryDatabase)
                                 .Repeat.AtLeastOnce();
            var inputContext = new WaveConditionsInputContext(grassCoverErosionInwardsInput,
                                                              new ForeshoreProfile[0],
                                                              assessmentSectionMock);

            var properties = new StabilityStoneCoverWaveConditionsCalculationInputContextProperties
            {
                Data = inputContext
            };
            var editor = new StabilityStoneCoverWaveConditionsCalculationInputContextHydraulicBoundaryLocationEditor();
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
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var waveConditionsInput = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation
            };

            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Expect(asm => asm.HydraulicBoundaryDatabase)
                                 .Return(hydraulicBoundaryDatabase)
                                 .Repeat.AtLeastOnce();
            var inputContext = new WaveConditionsInputContext(waveConditionsInput,
                                                              new ForeshoreProfile[0],
                                                              assessmentSectionMock);

            var properties = new StabilityStoneCoverWaveConditionsCalculationInputContextProperties
            {
                Data = inputContext
            };
            var editor = new StabilityStoneCoverWaveConditionsCalculationInputContextHydraulicBoundaryLocationEditor();
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
            Assert.AreSame(hydraulicBoundaryLocation, result);

            mockRepository.VerifyAll();
        }

        private class TestHydraulicBoundaryLocation : HydraulicBoundaryLocation
        {
            public TestHydraulicBoundaryLocation() : base(0, string.Empty, 0, 0) {}
        }
    }
}