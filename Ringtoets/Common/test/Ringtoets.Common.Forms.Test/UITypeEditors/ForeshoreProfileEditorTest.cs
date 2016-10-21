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
using System.Linq;
using System.Windows.Forms.Design;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.UITypeEditors;

namespace Ringtoets.Common.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class ForeshoreProfileEditorTest
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
            var foreshoreProfile = CreateForeshoreProfile();
            var properties = new ObjectPropertiesWithForeshoreProfile(foreshoreProfile, new ForeshoreProfile[0]);
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new ForeshoreProfileEditor();
            var someValue = new object();
            var serviceProviderMock = mockRepository.Stub<IServiceProvider>();
            var serviceMock = mockRepository.Stub<IWindowsFormsEditorService>();
            var descriptorContextMock = mockRepository.Stub<ITypeDescriptorContext>();
            serviceProviderMock.Stub(p => p.GetService(null)).IgnoreArguments().Return(serviceMock);
            descriptorContextMock.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

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
            var foreshoreProfile = CreateForeshoreProfile();
            var properties = new ObjectPropertiesWithForeshoreProfile(foreshoreProfile, new[]
            {
                foreshoreProfile
            });
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new ForeshoreProfileEditor();
            var someValue = new object();
            var serviceProviderMock = mockRepository.Stub<IServiceProvider>();
            var serviceMock = mockRepository.Stub<IWindowsFormsEditorService>();
            var descriptorContextMock = mockRepository.Stub<ITypeDescriptorContext>();
            serviceProviderMock.Stub(p => p.GetService(null)).IgnoreArguments().Return(serviceMock);
            descriptorContextMock.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            // Call
            var result = editor.EditValue(descriptorContextMock, serviceProviderMock, someValue);

            // Assert
            Assert.AreSame(foreshoreProfile, result);
            mockRepository.VerifyAll();
        }

        private static ForeshoreProfile CreateForeshoreProfile()
        {
            return new ForeshoreProfile(new Point2D(0, 0), Enumerable.Empty<Point2D>(), new BreakWater(BreakWaterType.Caisson, 0.0), new ForeshoreProfile.ConstructionProperties());
        }

        private class ObjectPropertiesWithForeshoreProfile : IHasForeshoreProfileProperty
        {
            private readonly ForeshoreProfile foreshoreProfile;
            private readonly IEnumerable<ForeshoreProfile> availableForeshoreProfiles;

            public ObjectPropertiesWithForeshoreProfile(ForeshoreProfile foreshoreProfile, IEnumerable<ForeshoreProfile> availableForeshoreProfiles)
            {
                this.foreshoreProfile = foreshoreProfile;
                this.availableForeshoreProfiles = availableForeshoreProfiles;
            }

            public object Data { get; set; }

            public ForeshoreProfile ForeshoreProfile
            {
                get
                {
                    return foreshoreProfile;
                }
            }

            public IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
            {
                return availableForeshoreProfiles;
            }
        }
    }
}