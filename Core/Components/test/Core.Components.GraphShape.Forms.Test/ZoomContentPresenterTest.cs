// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Threading;
using System.Windows;
using System.Windows.Controls;
using NUnit.Framework;

namespace Core.Components.GraphShape.Forms.Test
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class ZoomContentPresenterTest
    {
        [Test]
        public void ArrangeOverride_GivenContentLargerThanViewport_ThenContentKeepsItsOwnSize()
        {
            // Given
            var child = new Border
            {
                Width = 400,
                Height = 300
            };
            var presenter = new ZoomContentPresenter
            {
                Content = child
            };

            // When
            presenter.Measure(new Size(200, 100));
            presenter.Arrange(new Rect(0, 0, 200, 100));
            presenter.UpdateLayout();

            // Then
            Assert.AreEqual(new Size(400, 300), presenter.ContentSize);
            Assert.AreEqual(new Size(200, 100), presenter.DesiredSize);
            Assert.AreEqual(new Size(400, 300), child.RenderSize);
        }
    }
}
