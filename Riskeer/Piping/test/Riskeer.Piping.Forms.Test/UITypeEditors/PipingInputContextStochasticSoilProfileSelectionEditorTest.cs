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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.UITypeEditors;
using Riskeer.Piping.Primitives.TestUtil;

namespace Riskeer.Piping.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class PipingInputContextStochasticSoilProfileSelectionEditorTest
    {
        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            var context = mockRepository.DynamicMock<ITypeDescriptorContext>();

            var properties = new TestHasStochasticSoilProfileProperty(new PipingStochasticSoilProfile(1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile()),
                                                                      new[]
                                                                      {
                                                                          new PipingStochasticSoilProfile(0.9, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
                                                                      });

            var editor = new PipingInputContextStochasticSoilProfileSelectionEditor<TestHasStochasticSoilProfileProperty>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            context.Expect(c => c.Instance).Return(propertyBag);

            mockRepository.ReplayAll();

            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);

            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            var context = mockRepository.DynamicMock<ITypeDescriptorContext>();

            var stochasticSoilProfile = new PipingStochasticSoilProfile(1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

            var properties = new TestHasStochasticSoilProfileProperty(stochasticSoilProfile,
                                                                      new[]
                                                                      {
                                                                          stochasticSoilProfile
                                                                      });

            var editor = new PipingInputContextStochasticSoilProfileSelectionEditor<TestHasStochasticSoilProfileProperty>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            context.Expect(c => c.Instance).Return(propertyBag);

            mockRepository.ReplayAll();

            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(stochasticSoilProfile, result);

            mockRepository.VerifyAll();
        }

        private class TestHasStochasticSoilProfileProperty : IHasStochasticSoilProfile
        {
            private readonly IEnumerable<PipingStochasticSoilProfile> availableStochasticSoilProfiles;

            public event EventHandler<EventArgs> RefreshRequired;

            public TestHasStochasticSoilProfileProperty(PipingStochasticSoilProfile stochasticSoilProfile, IEnumerable<PipingStochasticSoilProfile> availableStochasticSoilProfiles)
            {
                this.availableStochasticSoilProfiles = availableStochasticSoilProfiles;
                StochasticSoilProfile = stochasticSoilProfile;
            }

            public object Data { get; set; }

            public PipingStochasticSoilProfile StochasticSoilProfile { get; }

            public IEnumerable<PipingStochasticSoilProfile> GetAvailableStochasticSoilProfiles()
            {
                return availableStochasticSoilProfiles;
            }
        }
    }
}