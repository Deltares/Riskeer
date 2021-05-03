// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Gui.PropertyBag;
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
            var hasStochasticSoilProfile = mockRepository.Stub<IHasStochasticSoilProfile>();

            hasStochasticSoilProfile.Stub(hssp => hssp.StochasticSoilProfile).Return(
                new PipingStochasticSoilProfile(1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile()));
            hasStochasticSoilProfile.Stub(hssp => hssp.GetAvailableStochasticSoilProfiles()).Return(
                new[]
                {
                    new PipingStochasticSoilProfile(0.9, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
                });

            var editor = new PipingInputContextStochasticSoilProfileSelectionEditor<IHasStochasticSoilProfile>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(hasStochasticSoilProfile);

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
            var hasStochasticSoilProfile = mockRepository.Stub<IHasStochasticSoilProfile>();
            var stochasticSoilProfile = new PipingStochasticSoilProfile(1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

            hasStochasticSoilProfile.Stub(hssp => hssp.StochasticSoilProfile).Return(stochasticSoilProfile);
            hasStochasticSoilProfile.Stub(hssp => hssp.GetAvailableStochasticSoilProfiles()).Return(
                new[]
                {
                    stochasticSoilProfile
                });

            var editor = new PipingInputContextStochasticSoilProfileSelectionEditor<IHasStochasticSoilProfile>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(hasStochasticSoilProfile);

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
    }
}