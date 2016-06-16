// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing.Drawing2D;
using Core.Components.OxyPlot.Converter;
using NUnit.Framework;
using OxyPlot;

namespace Core.Components.OxyPlot.Test.Converter
{
    [TestFixture]
    public class ChartDataHelperTest
    {
        [Test]
        public void Convert_Solid_ReturnsDefault()
        {
            // Call
            var lineStyle = ChartDataHelper.Convert(DashStyle.Solid);

            // Assert
            Assert.AreEqual(LineStyle.Solid, lineStyle);
        }

        [Test]
        public void Convert_Dash_ReturnsDash()
        {
            // Call
            var lineStyle = ChartDataHelper.Convert(DashStyle.Dash);

            // Assert
            Assert.AreEqual(LineStyle.Dash, lineStyle);
        }

        [Test]
        public void Convert_Dot_ReturnsDot()
        {
            // Call
            var lineStyle = ChartDataHelper.Convert(DashStyle.Dot);

            // Assert
            Assert.AreEqual(LineStyle.Dot, lineStyle);
        }

        [Test]
        public void Convert_DashDot_ReturnsDashDot()
        {
            // Call
            var lineStyle = ChartDataHelper.Convert(DashStyle.DashDot);

            // Assert
            Assert.AreEqual(LineStyle.DashDot, lineStyle);
        }

        [Test]
        public void Convert_DashDotDot_ReturnsDashDotDot()
        {
            // Call
            var lineStyle = ChartDataHelper.Convert(DashStyle.DashDotDot);

            // Assert
            Assert.AreEqual(LineStyle.DashDotDot, lineStyle);
        }

        [Test]
        public void Convert_Custom_ThrowsNotSupportedException()
        {
            // Call
            TestDelegate call = () => ChartDataHelper.Convert(DashStyle.Custom);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }
    }
}