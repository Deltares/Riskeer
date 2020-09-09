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
using System.Collections;
using System.Collections.Generic;
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Input;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Kernels.UpliftVan.Input
{
    [TestFixture]
    public class LoopComparerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var comparer = new LoopComparer();

            // Assert
            Assert.IsInstanceOf<IComparer>(comparer);
            Assert.IsInstanceOf<IComparer<Loop>>(comparer);
        }

        [Test]
        public void Compare_FirstObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            var firstObject = new object();
            object secondObject = new Loop();

            var comparer = new LoopComparer();

            // Call
            void Call() => comparer.Compare(firstObject, secondObject);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual($"Cannot compare objects other than {typeof(Loop)} with this comparer.", exception.Message);
        }

        [Test]
        public void Compare_SecondObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            object firstObject = new Loop();
            var secondObject = new object();

            var comparer = new LoopComparer();

            // Call
            void Call() => comparer.Compare(firstObject, secondObject);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual($"Cannot compare objects other than {typeof(Loop)} with this comparer.", exception.Message);
        }

        [Test]
        public void Compare_SameInstance_ReturnZero()
        {
            // Setup
            var loop = new Loop();

            // Call
            int result = new LoopComparer().Compare(loop, loop);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_SameCurveInstance_ReturnZero()
        {
            // Setup
            var curve = new Curve
            {
                HeadPoint = new Point2D(1.1, 2.2),
                EndPoint = new Point2D(3.3, 4.4)
            };
            var loop1 = new Loop
            {
                Curves = new List<Curve>
                {
                    curve
                }
            };
            var loop2 = new Loop
            {
                Curves = new List<Curve>
                {
                    curve
                }
            };

            // Call
            int result = new LoopComparer().Compare(loop1, loop2);

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
            var loop1 = new Loop
            {
                Curves = new List<Curve>
                {
                    new Curve
                    {
                        HeadPoint = new Point2D(x1, y1),
                        EndPoint = new Point2D(x2, y2)
                            }
                }
            };
            var loop2 = new Loop
            {
                Curves = new List<Curve>
                {
                    new Curve
                    {
                        HeadPoint = new Point2D(x1, y1),
                        EndPoint = new Point2D(x2, y2)
                    }
                }
            };

            // Call
            int result = new LoopComparer().Compare(loop1, loop2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_DifferentCoordinates_ReturnOne()
        {
            // Setup
            var loop1 = new Loop
            {
                Curves = new List<Curve>
                {
                    new Curve
                    {
                        HeadPoint = new Point2D(0, 0),
                        EndPoint = new Point2D(1, 1)
                    }
                }
            };
            var loop2 = new Loop
            {
                Curves = new List<Curve>
                {
                    new Curve
                    {
                        HeadPoint = new Point2D(2, 2),
                        EndPoint = new Point2D(3, 3)
                    }
                }
            };

            // Call
            int result = new LoopComparer().Compare(loop1, loop2);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}