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

using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Kernels.UpliftVan.Input
{
    [TestFixture]
    public class GeometryLoopComparerTest
    {
        [Test]
        public void Compare_SameInstance_ReturnZero()
        {
            // Setup
            var loop = new GeometryLoop();

            // Call
            int result = new GeometryLoopComparer().Compare(loop, loop);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_SameCurveInstance_ReturnZero()
        {
            // Setup
            var curve = new GeometryCurve(new Point2D(1.1, 2.2), new Point2D(3.3, 4.4));
            var loop1 = new GeometryLoop
            {
                CurveList =
                {
                    curve
                }
            };
            var loop2 = new GeometryLoop
            {
                CurveList =
                {
                    curve
                }
            };

            // Call
            int result = new GeometryLoopComparer().Compare(loop1, loop2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_EqualCoordinates_ReturnZero()
        {
            // Setup
            const double x1 = 1.1;
            const double y1 = 2.2;
            const double x2 = 3.3;
            const double y2 = 4.4;
            var loop1 = new GeometryLoop
            {
                CurveList =
                {
                    new GeometryCurve(new Point2D(x1, y1), new Point2D(x2, y2))
                }
            };
            var loop2 = new GeometryLoop
            {
                CurveList =
                {
                    new GeometryCurve(new Point2D(x1, y1), new Point2D(x2, y2))
                }
            };

            // Call
            int result = new GeometryLoopComparer().Compare(loop1, loop2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_DifferentCoordinates_ReturnOne()
        {
            // Setup
            var loop1 = new GeometryLoop
            {
                CurveList =
                {
                    new GeometryCurve(new Point2D(0, 0), new Point2D(1, 1))
                }
            };
            var loop2 = new GeometryLoop
            {
                CurveList =
                {
                    new GeometryCurve(new Point2D(2, 2), new Point2D(3, 3))
                }
            };

            // Call
            int result = new GeometryLoopComparer().Compare(loop1, loop2);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}