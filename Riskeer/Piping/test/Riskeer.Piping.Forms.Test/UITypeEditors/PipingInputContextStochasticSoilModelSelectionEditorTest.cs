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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Core.Common.Base.Geometry;
using Core.Gui.PropertyBag;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.UITypeEditors;
using Riskeer.Piping.Primitives.TestUtil;

namespace Riskeer.Piping.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class PipingInputContextStochasticSoilModelSelectionEditorTest
    {
        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var provider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IWindowsFormsEditorService>();
            var context = Substitute.For<ITypeDescriptorContext>();
            var hasStochasticSoilModel = Substitute.For<IHasStochasticSoilModel>();

            hasStochasticSoilModel.StochasticSoilModel.Returns(
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("StochasticSoilModelName"));
            hasStochasticSoilModel.GetAvailableStochasticSoilModels().Returns(
                new[]
                {
                    PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("NewStochasticSoilModelName")
                });

            var editor = new PipingInputContextStochasticSoilModelSelectionEditor<IHasStochasticSoilModel>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(hasStochasticSoilModel);

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
            var hasStochasticSoilModel = Substitute.For<IHasStochasticSoilModel>();
            var stochasticSoilModel = new PipingStochasticSoilModel("Model", new[]
            {
                new Point2D(0, 2),
                new Point2D(4, 2)
            }, new[]
            {
                new PipingStochasticSoilProfile(1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
            });

            hasStochasticSoilModel.StochasticSoilModel.Returns(stochasticSoilModel);
            hasStochasticSoilModel.GetAvailableStochasticSoilModels().Returns(
                new[]
                {
                    stochasticSoilModel
                });

            var editor = new PipingInputContextStochasticSoilModelSelectionEditor<IHasStochasticSoilModel>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(hasStochasticSoilModel);

            provider.GetService(Arg.Any<Type>()).Returns(service);
            // service.DropDownControl(Arg.Any<Control>());
            context.Instance.Returns(propertyBag);
            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(stochasticSoilModel, result);
        }
    }
}