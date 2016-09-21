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
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.Revetment.Forms.UITypeEditors;

namespace Ringtoets.Revetment.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class WaveConditionsInputContextForeshoreProfileEditorTest
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
            var foreshoreProfiles = new[]
            {
                new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                     null, new ForeshoreProfile.ConstructionProperties())
            };

            var grassCoverErosionInwardsInput = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone);

            var inputContext = new TestWaveConditionsInputContext(grassCoverErosionInwardsInput,
                                                                  foreshoreProfiles);

            var properties = new TestWaveConditionsInputContextProperties
            {
                Data = inputContext
            };
            var editor = new WaveConditionsInputContextForeshoreProfileEditor();
            var propertyBag = new DynamicPropertyBag(properties);

            var serviceProviderMock = mockRepository.StrictMock<IServiceProvider>();
            var serviceMock = mockRepository.StrictMock<IWindowsFormsEditorService>();
            var descriptorContextMock = mockRepository.StrictMock<ITypeDescriptorContext>();
            serviceProviderMock.Expect(p => p.GetService(null)).IgnoreArguments().Return(serviceMock);
            serviceMock.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            serviceMock.Expect(s => s.CloseDropDown());
            descriptorContextMock.Expect(c => c.Instance).Return(propertyBag).Repeat.Times(3);
            mockRepository.ReplayAll();

            var someValue = new object();

            // Call
            var result = editor.EditValue(descriptorContextMock, serviceProviderMock, someValue);

            // Assert
            Assert.IsNull(result);

            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties());
            var waveConditionsInput = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone)
            {
                ForeshoreProfile = foreshoreProfile
            };

            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var inputContext = new TestWaveConditionsInputContext(waveConditionsInput,
                                                                  new[]
                                                                  {
                                                                      foreshoreProfile
                                                                  });

            var properties = new TestWaveConditionsInputContextProperties
            {
                Data = inputContext
            };
            var editor = new WaveConditionsInputContextForeshoreProfileEditor();
            var propertyBag = new DynamicPropertyBag(properties);

            var serviceProviderMock = mockRepository.StrictMock<IServiceProvider>();
            var serviceMock = mockRepository.StrictMock<IWindowsFormsEditorService>();
            var descriptorContextMock = mockRepository.StrictMock<ITypeDescriptorContext>();
            serviceProviderMock.Expect(p => p.GetService(null)).IgnoreArguments().Return(serviceMock);
            serviceMock.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            serviceMock.Expect(s => s.CloseDropDown()).IgnoreArguments();
            descriptorContextMock.Expect(c => c.Instance).Return(propertyBag).Repeat.Times(3);
            mockRepository.ReplayAll();

            var someValue = new object();

            // Call
            var result = editor.EditValue(descriptorContextMock, serviceProviderMock, someValue);

            // Assert
            Assert.AreSame(foreshoreProfile, result);

            mockRepository.VerifyAll();
        }

        private class TestWaveConditionsInputContext : WaveConditionsInputContext
        {
            private readonly IEnumerable<ForeshoreProfile> foreshoreProfiles;

            public TestWaveConditionsInputContext(WaveConditionsInput wrappedData,
                                                  IEnumerable<ForeshoreProfile> foreshoreProfiles) : base(wrappedData)
            {
                this.foreshoreProfiles = foreshoreProfiles;
            }

            public override IEnumerable<HydraulicBoundaryLocation> HydraulicBoundaryLocations
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override IEnumerable<ForeshoreProfile> ForeshoreProfiles
            {
                get
                {
                    return foreshoreProfiles;
                }
            }
        }

        private class TestWaveConditionsInputContextProperties : WaveConditionsInputContextProperties<WaveConditionsInputContext>
        {
            public override string RevetmentType
            {
                get
                {
                    return "test";
                }
            }
        }
    }
}