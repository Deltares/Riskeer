// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.ComponentModel;
using System.Drawing.Drawing2D;
using Core.Components.DotSpatial.Converter;
using Core.Components.Gis.Style;
using DotSpatial.Symbology;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class MapDataHelperTest
    {
        #region Convert symbol

        [Test]
        public void Convert_Circle_ReturnDefault()
        {
            // Call
            PointShape symbol = MapDataHelper.Convert(PointSymbol.Circle);

            // Assert
            Assert.AreEqual(PointShape.Ellipse, symbol);
        }

        [Test]
        public void Convert_Square_ReturnRectangle()
        {
            // Call
            PointShape symbol = MapDataHelper.Convert(PointSymbol.Square);

            // Assert
            Assert.AreEqual(PointShape.Rectangle, symbol);
        }

        [Test]
        public void Convert_Triangle_ReturnTriangle()
        {
            // Call
            PointShape symbol = MapDataHelper.Convert(PointSymbol.Triangle);

            // Assert
            Assert.AreEqual(PointShape.Triangle, symbol);
        }

        [Test]
        public void Convert_Diamond_ReturnDiamond()
        {
            // Call
            PointShape symbol = MapDataHelper.Convert(PointSymbol.Diamond);

            // Assert
            Assert.AreEqual(PointShape.Diamond, symbol);
        }

        [Test]
        public void Convert_Star_ReturnStar()
        {
            // Call
            PointShape symbol = MapDataHelper.Convert(PointSymbol.Star);

            // Assert
            Assert.AreEqual(PointShape.Star, symbol);
        }

        [Test]
        public void Convert_Hexagon_ReturnStar()
        {
            // Call
            PointShape symbol = MapDataHelper.Convert(PointSymbol.Hexagon);

            // Assert
            Assert.AreEqual(PointShape.Hexagon, symbol);
        }

        [Test]
        public void Convert_Pentagon_ReturnStar()
        {
            // Call
            PointShape symbol = MapDataHelper.Convert(PointSymbol.Pentagon);

            // Assert
            Assert.AreEqual(PointShape.Pentagon, symbol);
        }

        [Test]
        public void Convert_InvalidPointSymbol_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => MapDataHelper.Convert((PointSymbol) 100);

            // Assert
            Assert.Throws<InvalidEnumArgumentException>(test);
        }

        #endregion

        #region Convert DashStyle

        [Test]
        public void Convert_Solid_ReturnsSolid()
        {
            // Call
            DashStyle dashStyle = MapDataHelper.Convert(LineDashStyle.Solid);

            // Assert
            Assert.AreEqual(DashStyle.Solid, dashStyle);
        }

        [Test]
        public void Convert_Dash_ReturnsDash()
        {
            // Call
            DashStyle dashStyle = MapDataHelper.Convert(LineDashStyle.Dash);

            // Assert
            Assert.AreEqual(DashStyle.Dash, dashStyle);
        }

        [Test]
        public void Convert_Dot_ReturnsDot()
        {
            // Call
            DashStyle dashStyle = MapDataHelper.Convert(LineDashStyle.Dot);

            // Assert
            Assert.AreEqual(DashStyle.Dot, dashStyle);
        }

        [Test]
        public void Convert_DashDot_ReturnsDashDot()
        {
            // Call
            DashStyle dashStyle = MapDataHelper.Convert(LineDashStyle.DashDot);

            // Assert
            Assert.AreEqual(DashStyle.DashDot, dashStyle);
        }

        [Test]
        public void Convert_DashDotDot_ReturnsDashDotDot()
        {
            // Call
            DashStyle dashStyle = MapDataHelper.Convert(LineDashStyle.DashDotDot);

            // Assert
            Assert.AreEqual(DashStyle.DashDotDot, dashStyle);
        }

        [Test]
        public void Convert_Unknown_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate call = () => MapDataHelper.Convert((LineDashStyle) 100);

            // Assert
            Assert.Throws<InvalidEnumArgumentException>(call);
        }

        #endregion
    }
}