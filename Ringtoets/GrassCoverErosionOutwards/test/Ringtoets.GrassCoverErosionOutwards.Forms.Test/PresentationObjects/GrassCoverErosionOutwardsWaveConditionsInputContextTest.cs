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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.Revetment.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsInputContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        Enumerable.Empty<Point2D>(),
                                                        null,
                                                        new ForeshoreProfile.ConstructionProperties());

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.ForeshoreProfiles.Add(foreshoreProfile);
            failureMechanism.HydraulicBoundaryLocations.Add(hydraulicBoundaryLocation);

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var input = calculation.InputParameters;

            // Call
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(
                input,
                calculation,
                failureMechanism);

            // Assert
            Assert.IsInstanceOf<WrappedObjectContextBase<WaveConditionsInput>>(context);
            Assert.AreSame(input, context.WrappedData);
            CollectionAssert.AreEqual(new[]
            {
                foreshoreProfile
            }, context.ForeshoreProfiles);
            CollectionAssert.AreEqual(new[]
            {
                hydraulicBoundaryLocation
            }, context.HydraulicBoundaryLocations);
        }

        [Test]
        public void Constructor_InputNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsInputContext(
                null, 
                new TestCalculation(), 
                failureMechanism);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("wrappedData", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var input = new WaveConditionsInput();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsInputContext(
                input,
                null,
                failureMechanism);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var input = new WaveConditionsInput();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsInputContext(
                input,
                new TestCalculation(), 
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }
    }
}