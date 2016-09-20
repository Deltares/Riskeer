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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.Revetment.Forms.UITypeEditors;

namespace Ringtoets.Revetment.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class WaveConditionsInputContextHydraulicBoundaryLocationEditorTest
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
            var grassCoverErosionInwardsInput = new WaveConditionsInput(WaveConditionsRevetment.Grass);

            var inputContext = new TestWaveConditionsInputContext(grassCoverErosionInwardsInput,
                                                                  new[]
                                                                  {
                                                                      new TestHydraulicBoundaryLocation()
                                                                  });

            var properties = new WaveConditionsInputContextProperties<WaveConditionsInputContext>
            {
                Data = inputContext
            };
            var editor = new WaveConditionsInputContextHydraulicBoundaryLocationEditor();
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
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var waveConditionsInput = new WaveConditionsInput(WaveConditionsRevetment.Grass)
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation
            };

            var inputContext = new TestWaveConditionsInputContext(waveConditionsInput,
                                                                  new[]
                                                                  {
                                                                      hydraulicBoundaryLocation
                                                                  });

            var properties = new WaveConditionsInputContextProperties<WaveConditionsInputContext>
            {
                Data = inputContext
            };
            var editor = new WaveConditionsInputContextHydraulicBoundaryLocationEditor();
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

        private class TestWaveConditionsInputContext : WaveConditionsInputContext
        {
            private readonly IEnumerable<HydraulicBoundaryLocation> locations;

            public TestWaveConditionsInputContext(WaveConditionsInput wrappedData, IEnumerable<HydraulicBoundaryLocation> locations) : base(wrappedData)
            {
                this.locations = locations;
            }

            public override IEnumerable<HydraulicBoundaryLocation> HydraulicBoundaryLocations
            {
                get
                {
                    return locations;
                }
            }

            public override IEnumerable<ForeshoreProfile> ForeshoreProfiles
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}