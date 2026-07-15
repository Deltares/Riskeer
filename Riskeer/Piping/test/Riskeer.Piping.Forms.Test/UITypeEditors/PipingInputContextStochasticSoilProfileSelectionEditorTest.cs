// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using NSubstitute;
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
            var provider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IWindowsFormsEditorService>();
            var context = Substitute.For<ITypeDescriptorContext>();
            var hasStochasticSoilProfile = Substitute.For<IHasStochasticSoilProfile>();

            hasStochasticSoilProfile.StochasticSoilProfile.Returns(
                new PipingStochasticSoilProfile(1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile()));
            hasStochasticSoilProfile.GetAvailableStochasticSoilProfiles().Returns(
                new[]
                {
                    new PipingStochasticSoilProfile(0.9, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
                });

            var editor = new PipingInputContextStochasticSoilProfileSelectionEditor<IHasStochasticSoilProfile>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(hasStochasticSoilProfile);

            provider.GetService(Arg.Any<Type>()).Returns(service);
            // service.DropDownControl(Arg.Any<Control>());
            context.Instance.Returns(propertyBag);
            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            var provider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IWindowsFormsEditorService>();
            var context = Substitute.For<ITypeDescriptorContext>();
            var hasStochasticSoilProfile = Substitute.For<IHasStochasticSoilProfile>();
            var stochasticSoilProfile = new PipingStochasticSoilProfile(1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

            hasStochasticSoilProfile.StochasticSoilProfile.Returns(stochasticSoilProfile);
            hasStochasticSoilProfile.GetAvailableStochasticSoilProfiles().Returns(
                new[]
                {
                    stochasticSoilProfile
                });

            var editor = new PipingInputContextStochasticSoilProfileSelectionEditor<IHasStochasticSoilProfile>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(hasStochasticSoilProfile);

            provider.GetService(Arg.Any<Type>()).Returns(service);
            // service.DropDownControl(Arg.Any<Control>());
            context.Instance.Returns(propertyBag);
            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(stochasticSoilProfile, result);
        }
    }
}